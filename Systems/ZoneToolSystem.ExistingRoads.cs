// File: Systems/ZoneToolSystem.ExistingRoads.cs
// Purpose: Update Existing Roads tool (hover + select + apply zoning mode to existing networks).
// Notes:
// - Enabled/disabled by UI panel.
// - Disabling returns to DefaultToolSystem.
// - Tool refuses to activate unless GetPrefab is guaranteed non-null.
// - Hover uses vanilla highlight outline (Highlighted + BatchesUpdated) for visual feedback.
// - Click/apply uses vanilla tool sounds (ToolUXSoundSettingsData) for audio feedback.

namespace ZoningToolkit.Systems
{
    using Colossal.Serialization.Entities; // Purpose (OnGameLoadingComplete signature)
    using Game;                            // GameMode, ToolBaseSystem
    using Game.Audio;                      // AudioManager
    using Game.Common;                     // Updated, Highlighted, BatchesUpdated
    using Game.Net;                        // Edge, SubBlock
    using Game.Prefabs;                    // PrefabBase, PrefabSystem
    using Game.Tools;                      // ToolSystem, DefaultToolSystem, NetToolSystem, ToolUXSoundSettingsData
    using Game.Zones;
    using Unity.Collections;               // NativeHashSet, Allocator
    using Unity.Entities;                  // Entity, EntityCommandBuffer, EntityQuery, EntityQueryBuilder
    using Unity.Jobs;                      // JobHandle
    using ZoningToolkit.Components;        // ZoningInfo, ZoningInfoUpdated, ZoningMode

    internal sealed partial class ZoneToolSystemExistingRoads : ToolBaseSystem
    {
        private ToolSystem m_ZTToolSystem = null!;
        private DefaultToolSystem m_ZTDefaultToolSystem = null!;
        private NetToolSystem m_NetToolSystem = null!;
        private ToolOutputBarrier m_ToolOutputBarrier = null!;
        private PrefabSystem m_ZTPrefabSystem = null!;
        private ZoneToolBridgeUI m_UISystem = null!;

        private EntityQuery m_SoundbankQuery;

        // Selected road entities (click/drag selection).
        private NativeHashSet<Entity> m_Selected;
        private int m_SelectedCount;

        // Currently hovered road entity (from raycast).
        private Entity m_Hovered;

        // Currently highlighted road entity (vanilla outline).
        private Entity m_Highlighted;

        internal bool toolEnabled
        {
            get;
            private set;
        }

        // Unique tool identifier. UI and tool infrastructure can reference tools by this ID.
        public override string toolID => "ZoningToolkit.ExistingRoads";

        protected override void OnCreate( )
        {
            base.OnCreate();

            Enabled = false;

            m_ZTToolSystem = World.GetOrCreateSystemManaged<ToolSystem>();
            m_ZTDefaultToolSystem = World.GetOrCreateSystemManaged<DefaultToolSystem>();
            m_NetToolSystem = World.GetOrCreateSystemManaged<NetToolSystem>();
            m_ToolOutputBarrier = World.GetOrCreateSystemManaged<ToolOutputBarrier>();
            m_ZTPrefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();
            m_UISystem = World.GetOrCreateSystemManaged<ZoneToolBridgeUI>();

            m_SoundbankQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<ToolUXSoundSettingsData>()
                .Build(this);

            m_Selected = new NativeHashSet<Entity>(128, Allocator.Persistent);
            m_SelectedCount = 0;

            m_Hovered = Entity.Null;
            m_Highlighted = Entity.Null;

            toolEnabled = false;

            ResetSafePrefabForUI();
        }

        protected override void OnDestroy( )
        {
            if (m_Selected.IsCreated)
            {
                m_Selected.Dispose();
            }

            base.OnDestroy();
        }

        protected override void OnStartRunning( )
        {
            base.OnStartRunning();

            toolEnabled = true;

            applyAction.shouldBeEnabled = true;
            secondaryApplyAction.shouldBeEnabled = true;

            requireNet = Layer.Road;
            requireZones = true;

            allowUnderground = false;
        }

        protected override void OnStopRunning( )
        {
            base.OnStopRunning();

            toolEnabled = false;

            applyAction.shouldBeEnabled = false;
            secondaryApplyAction.shouldBeEnabled = false;

            ClearSelection();
            m_Hovered = Entity.Null;

            // IMPORTANT:
            // ToolOutputBarrier.CreateCommandBuffer() is not allowed in OnStopRunning().
            // Highlight cleanup uses immediate EntityManager structural changes.
            ClearHoverHighlightImmediate();
        }

        public override void InitializeRaycast( )
        {
            base.InitializeRaycast();

            m_ToolRaycastSystem.typeMask = TypeMask.Net | TypeMask.Lanes;
            m_ToolRaycastSystem.netLayerMask = Layer.Road;
        }

        protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
        {
            base.OnGameLoadingComplete(purpose, mode);

            ResetSafePrefabForUI();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (!toolEnabled)
            {
                return inputDeps;
            }

            applyMode = ApplyMode.Clear;

            EntityCommandBuffer ecb = m_ToolOutputBarrier.CreateCommandBuffer();

            if (secondaryApplyAction.WasPressedThisFrame())
            {
                CycleZoningMode();
                PlaySnapSound();
            }

            UpdateHover();
            UpdateHoverHighlight(ecb);

            if (applyAction.WasPressedThisFrame() || applyAction.IsPressed())
            {
                AddHoveredToSelection();
            }

            if (applyAction.WasReleasedThisFrame())
            {
                ApplySelection();
            }

            return inputDeps;
        }

        public override PrefabBase GetPrefab( )
        {
            return GetSafePrefabForUI();
        }

        public override bool TrySetPrefab(PrefabBase prefab)
        {
            return false;
        }

        // Contour icon state for UI.
        internal bool ContourEnabled => (selectedSnap & Snap.ContourLines) != 0;

        // Called by BridgeUI trigger.
        internal void ToggleContourLines( )
        {
            Snap snap = selectedSnap;

            bool next = (snap & Snap.ContourLines) == 0;
            if (next)
            {
                snap |= Snap.ContourLines;
            }
            else
            {
                snap &= ~Snap.ContourLines;
            }

            selectedSnap = snap;

            // Keep vanilla NetToolSystem snap state consistent to avoid UI mismatch.
            m_NetToolSystem.selectedSnap = selectedSnap;

            PlaySnapSound();
        }

        internal bool EnableTool( )
        {
            if (!TryResolveSafePrefabForUI(out _))
            {
                toolEnabled = false;
                Enabled = false;
                Mod.s_Log.Warn($"{Mod.ModTag} ExistingRoads enable refused: safe prefab not ready.");
                return false;
            }

            Enabled = true;
            m_ZTToolSystem.activeTool = this;

            selectedSnap = m_NetToolSystem.selectedSnap;

            toolEnabled = true;

            Mod.s_Log.Info($"{Mod.ModTag} ExistingRoads enabled");
            return true;
        }

        internal void DisableTool( )
        {
            toolEnabled = false;

            ClearSelection();
            m_Hovered = Entity.Null;

            // IMPORTANT:
            // ToolOutputBarrier.CreateCommandBuffer() is not allowed from toolbar/tool-change paths.
            // Highlight cleanup uses immediate EntityManager structural changes.
            ClearHoverHighlightImmediate();

            // Persist snap state back to vanilla for consistency.
            m_NetToolSystem.selectedSnap = selectedSnap;

            m_ZTToolSystem.activeTool = m_ZTDefaultToolSystem;

            Enabled = false;

            PlayCancelSound();
            Mod.s_Log.Info($"{Mod.ModTag} ExistingRoads disabled");
        }

        private void CycleZoningMode( )
        {
            ZoningMode current = m_UISystem.CurrentZoningMode;
            ZoningMode next = current switch
            {
                ZoningMode.Default => ZoningMode.Left,
                ZoningMode.Left => ZoningMode.Right,
                ZoningMode.Right => ZoningMode.None,
                ZoningMode.None => ZoningMode.Default,
                _ => ZoningMode.Default
            };

            m_UISystem.SetZoningModeFromTool(next);
        }

        private void UpdateHover( )
        {
            Entity newHovered = TryGetRaycastRoad(out Entity e) ? e : Entity.Null;
            if (newHovered == m_Hovered)
            {
                return;
            }

            m_Hovered = newHovered;
        }

        private void AddHoveredToSelection( )
        {
            if (m_Hovered == Entity.Null)
            {
                return;
            }

            if (!EntityManager.Exists(m_Hovered))
            {
                return;
            }

            if (!m_Selected.Contains(m_Hovered))
            {
                m_Selected.Add(m_Hovered);
                m_SelectedCount++;

                PlaySelectSound();
            }
        }

        private void ClearSelection( )
        {
            m_Selected.Clear();
            m_SelectedCount = 0;
        }

        private void ApplySelection( )
        {
            if (m_SelectedCount == 0)
            {
                return;
            }

            ZoningMode mode = m_UISystem.CurrentZoningMode;

            EntityCommandBuffer ecb = m_ToolOutputBarrier.CreateCommandBuffer();

            foreach (Entity roadEntity in m_Selected)
            {
                AddOrSetZoningInfo(ecb, roadEntity, mode);
                TagSubBlocksForUpdate(ecb, roadEntity);
            }

            ClearSelection();

            PlayBuildSound();
        }

        private bool TryGetRaycastRoad(out Entity entity)
        {
            entity = Entity.Null;

            if (!base.GetRaycastResult(out Entity hit, out RaycastHit _))
            {
                return false;
            }

            if (!EntityManager.HasComponent<Edge>(hit))
            {
                return false;
            }

            if (!EntityManager.HasBuffer<SubBlock>(hit))
            {
                return false;
            }

            entity = hit;
            return true;
        }

        private void AddOrSetZoningInfo(EntityCommandBuffer ecb, Entity owner, ZoningMode mode)
        {
            ZoningInfo zi = new ZoningInfo { zoningMode = mode };

            if (EntityManager.HasComponent<ZoningInfo>(owner))
            {
                ecb.SetComponent(owner, zi);
            }
            else
            {
                ecb.AddComponent(owner, zi);
            }
        }

        private void TagSubBlocksForUpdate(EntityCommandBuffer ecb, Entity roadEntity)
        {
            if (!EntityManager.HasBuffer<SubBlock>(roadEntity))
            {
                return;
            }

            DynamicBuffer<SubBlock> subBlocks = EntityManager.GetBuffer<SubBlock>(roadEntity, isReadOnly: true);

            for (int i = 0; i < subBlocks.Length; i++)
            {
                Entity blockEntity = subBlocks[i].m_SubBlock;
                if (blockEntity == Entity.Null)
                {
                    continue;
                }

                if (!EntityManager.Exists(blockEntity))
                {
                    continue;
                }

                if (!EntityManager.HasComponent<ZoningInfoUpdated>(blockEntity))
                {
                    ecb.AddComponent<ZoningInfoUpdated>(blockEntity);
                }

                if (!EntityManager.HasComponent<Updated>(blockEntity))
                {
                    ecb.AddComponent<Updated>(blockEntity);
                }
            }
        }

        private void UpdateHoverHighlight(EntityCommandBuffer ecb)
        {
            Entity target = Entity.Null;

            if (m_Hovered != Entity.Null && EntityManager.Exists(m_Hovered))
            {
                ZoningMode desired = m_UISystem.CurrentZoningMode;
                ZoningMode current = EntityManager.HasComponent<ZoningInfo>(m_Hovered)
                    ? EntityManager.GetComponentData<ZoningInfo>(m_Hovered).zoningMode
                    : ZoningMode.Default;

                if (current != desired)
                {
                    target = m_Hovered;
                }
            }

            if (target == m_Highlighted)
            {
                return;
            }

            if (m_Highlighted != Entity.Null)
            {
                SetHighlighted(ecb, m_Highlighted, value: false);
            }

            m_Highlighted = target;

            if (m_Highlighted != Entity.Null)
            {
                SetHighlighted(ecb, m_Highlighted, value: true);
            }
        }

        private void ClearHoverHighlight(EntityCommandBuffer ecb)
        {
            if (m_Highlighted == Entity.Null)
            {
                return;
            }

            SetHighlighted(ecb, m_Highlighted, value: false);
            m_Highlighted = Entity.Null;
        }

        // Immediate cleanup path (no ECB).
        private void ClearHoverHighlightImmediate( )
        {
            if (m_Highlighted == Entity.Null)
            {
                return;
            }

            if (!EntityManager.Exists(m_Highlighted))
            {
                m_Highlighted = Entity.Null;
                return;
            }

            SetHighlightedImmediate(m_Highlighted, value: false);
            m_Highlighted = Entity.Null;
        }

        private void SetHighlighted(EntityCommandBuffer ecb, Entity entity, bool value)
        {
            if (value)
            {
                if (!EntityManager.HasComponent<Highlighted>(entity))
                {
                    ecb.AddComponent<Highlighted>(entity);
                }

                if (!EntityManager.HasComponent<Updated>(entity))
                {
                    ecb.AddComponent<Updated>(entity);
                }
            }
            else
            {
                if (EntityManager.HasComponent<Highlighted>(entity))
                {
                    ecb.RemoveComponent<Highlighted>(entity);
                }

                if (!EntityManager.HasComponent<Updated>(entity))
                {
                    ecb.AddComponent<Updated>(entity);
                }
            }

            if (EntityManager.HasComponent<Edge>(entity))
            {
                Edge edge = EntityManager.GetComponentData<Edge>(entity);

                if (edge.m_Start != Entity.Null)
                {
                    ecb.AddComponent<BatchesUpdated>(edge.m_Start);
                }

                if (edge.m_End != Entity.Null)
                {
                    ecb.AddComponent<BatchesUpdated>(edge.m_End);
                }
            }
        }

        // Immediate version (no ECB).
        private void SetHighlightedImmediate(Entity entity, bool value)
        {
            if (value)
            {
                if (!EntityManager.HasComponent<Highlighted>(entity))
                {
                    EntityManager.AddComponent<Highlighted>(entity);
                }

                if (!EntityManager.HasComponent<Updated>(entity))
                {
                    EntityManager.AddComponent<Updated>(entity);
                }
            }
            else
            {
                if (EntityManager.HasComponent<Highlighted>(entity))
                {
                    EntityManager.RemoveComponent<Highlighted>(entity);
                }

                if (!EntityManager.HasComponent<Updated>(entity))
                {
                    EntityManager.AddComponent<Updated>(entity);
                }
            }

            if (EntityManager.HasComponent<Edge>(entity))
            {
                Edge edge = EntityManager.GetComponentData<Edge>(entity);

                AddBatchesUpdatedImmediate(edge.m_Start);
                AddBatchesUpdatedImmediate(edge.m_End);
            }
        }

        private void AddBatchesUpdatedImmediate(Entity node)
        {
            if (node == Entity.Null)
            {
                return;
            }

            if (!EntityManager.Exists(node))
            {
                return;
            }

            if (!EntityManager.HasComponent<BatchesUpdated>(node))
            {
                EntityManager.AddComponent<BatchesUpdated>(node);
            }
        }

        private bool TryGetSoundbank(out ToolUXSoundSettingsData soundbank)
        {
            if (m_SoundbankQuery.IsEmptyIgnoreFilter)
            {
                soundbank = default;
                return false;
            }

            soundbank = m_SoundbankQuery.GetSingleton<ToolUXSoundSettingsData>();
            return true;
        }

        private void PlaySelectSound( )
        {
            if (!TryGetSoundbank(out ToolUXSoundSettingsData soundbank))
            {
                return;
            }

            AudioManager.instance.PlayUISound(soundbank.m_SelectEntitySound);
        }

        private void PlayBuildSound( )
        {
            if (!TryGetSoundbank(out ToolUXSoundSettingsData soundbank))
            {
                return;
            }

            AudioManager.instance.PlayUISound(soundbank.m_NetBuildSound);
        }

        private void PlayCancelSound( )
        {
            if (!TryGetSoundbank(out ToolUXSoundSettingsData soundbank))
            {
                return;
            }

            AudioManager.instance.PlayUISound(soundbank.m_NetCancelSound);
        }

        private void PlaySnapSound( )
        {
            if (!TryGetSoundbank(out ToolUXSoundSettingsData soundbank))
            {
                return;
            }

            AudioManager.instance.PlayUISound(soundbank.m_SnapSound);
        }
    }
}
