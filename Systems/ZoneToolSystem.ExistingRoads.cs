// File: Systems/ZoneToolSystem.ExistingRoads.cs
// Purpose: Update Existing Roads tool (hover + select + apply zoning mode to existing networks).
// Notes:
// - Enabled/disabled by UI panel.
// - Disabling returns to DefaultToolSystem.
// - Tool refuses to activate unless GetPrefab is guaranteed non-null.

namespace ZoningToolkit.Systems
{
    using Colossal.Serialization.Entities; // Purpose (OnGameLoadingComplete signature)
    using Game;                            // GameMode, ToolBaseSystem
    using Game.Common;                     // Updated
    using Game.Net;                        // Edge, SubBlock
    using Game.Prefabs;                    // PrefabBase, PrefabSystem
    using Game.Tools;                      // ToolSystem, DefaultToolSystem, NetToolSystem, ToolOutputBarrier
    using Game.Zones;                      // Layer
    using Unity.Collections;               // NativeHashSet, Allocator
    using Unity.Entities;                  // Entity, EntityCommandBuffer, DynamicBuffer
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
        private bool m_ContourHostActive;

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

            InitializeAudio();

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

            // Contour-host mode: keep tool active so contour can render,
            // but do not enable actions or tool behavior.
            if (m_ContourHostActive)
            {
                toolEnabled = false;

                applyAction.shouldBeEnabled = false;
                secondaryApplyAction.shouldBeEnabled = false;

                requireNet = Layer.None;
                requireZones = false;
                allowUnderground = false;

                // Seed snap so contour has a place to live.
                Snap snap = m_NetToolSystem.selectedSnap;
                if (snap == default)
                {
                    snap = Snap.All;
                }
                selectedSnap = snap;

                return;
            }

            toolEnabled = true;

            applyAction.shouldBeEnabled = true;
            secondaryApplyAction.shouldBeEnabled = true;

            requireNet = Layer.Road;
            requireZones = true;

            allowUnderground = false;

            // Seed snap from vanilla so contour overlay can render.
            Snap s = selectedSnap;
            if (s == default)
            {
                s = m_NetToolSystem.selectedSnap;
                if (s == default)
                {
                    s = Snap.All;
                }
                selectedSnap = s;
            }
        }

        protected override void OnStopRunning( )
        {
            base.OnStopRunning();

            toolEnabled = false;

            applyAction.shouldBeEnabled = false;
            secondaryApplyAction.shouldBeEnabled = false;

            ClearSelection();
            m_Hovered = Entity.Null;

            // ToolOutputBarrier.CreateCommandBuffer() is not allowed in OnStopRunning().
            // Highlight cleanup uses immediate EntityManager structural changes.
            ClearHoverHighlightImmediate();
        }

        public override void InitializeRaycast( )
        {
            base.InitializeRaycast();

            // Contour-host mode: do not interact with the world.
            if (m_ContourHostActive)
            {
                m_ToolRaycastSystem.typeMask = TypeMask.None;
                m_ToolRaycastSystem.netLayerMask = Layer.None;
                return;
            }

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
            // Contour-host mode: do nothing except remain active.
            if (m_ContourHostActive)
            {
                return inputDeps;
            }

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

        public override void GetAvailableSnapMask(out Snap onMask, out Snap offMask)
        {
            base.GetAvailableSnapMask(out onMask, out offMask);

            // Contour must be present in BOTH masks so selectedSnap can turn it on/off.
            // Putting it only in onMask forces it on (no toggle).
            onMask |= Snap.ContourLines;
            offMask |= Snap.ContourLines;
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

        internal bool EnableContourHost( )
        {
            if (m_ContourHostActive)
            {
                // Host flag already set, but tool may no longer be active.
                Enabled = true;
                if (m_ZTToolSystem.activeTool != this)
                {
                    m_ZTToolSystem.activeTool = this;
                }

                return true;
            }


            // Do not hijack when full Update Road tool is enabled.
            if (toolEnabled)
            {
                return true;
            }

            if (!TryResolveSafePrefabForUI(out _))
            {
                Enabled = false;
                Mod.s_Log.Warn($"{Mod.ModTag} ContourHost enable refused: safe prefab not ready.");
                return false;
            }

            m_ContourHostActive = true;
            Enabled = true;
            m_ZTToolSystem.activeTool = this;

            Snap snap = m_NetToolSystem.selectedSnap;
            if (snap == default)
            {
                snap = Snap.All;
            }
            selectedSnap = snap;

            // when host is active, snap state (contour) pushed into Net tool's snap state for road-tab sync.
            m_NetToolSystem.selectedSnap = selectedSnap;

            return true;
        }

        internal void DisableContourHost( )
        {
            if (!m_ContourHostActive)
            {
                return;
            }

            // If Update Road enabled, this tool stays active as a real tool.
            if (toolEnabled)
            {
                m_ContourHostActive = false;
                return;
            }

            m_ContourHostActive = false;

            m_NetToolSystem.selectedSnap = selectedSnap;  // last known snap handed back to vanlilla before we drop to defaultTool.

            // Return to vanilla default tool.
            if (m_ZTToolSystem.activeTool == this)
            {
                m_ZTToolSystem.activeTool = m_ZTDefaultToolSystem;
            }

            Enabled = false;
        }


        internal bool EnableTool( )
        {
            m_ContourHostActive = false;

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
            m_ContourHostActive = false;

            toolEnabled = false;

            ClearSelection();
            m_Hovered = Entity.Null;

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

            ZoningMode desired = m_UISystem.CurrentZoningMode;

            // Only create an ECB if at least one entity actually needs changes.
            bool didWork = false;

            EntityCommandBuffer ecb = default;

            foreach (Entity roadEntity in m_Selected)
            {
                if (roadEntity == Entity.Null || !EntityManager.Exists(roadEntity))
                {
                    continue;
                }

                ZoningMode current = EntityManager.HasComponent<ZoningInfo>(roadEntity)
                    ? EntityManager.GetComponentData<ZoningInfo>(roadEntity).zoningMode
                    : ZoningMode.Default;

                if (current == desired)
                {
                    continue;
                }

                if (!didWork)
                {
                    didWork = true;
                    ecb = m_ToolOutputBarrier.CreateCommandBuffer();
                }

                AddOrSetZoningInfo(ecb, roadEntity, desired);
                TagSubBlocksForUpdate(ecb, roadEntity);
            }

            ClearSelection();

            if (didWork)
            {
                PlayBuildSound();
            }
            else
            {
                // “No-op” feedback: quieter than build thud.
                PlaySelectSound();
            }
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
    }
}
