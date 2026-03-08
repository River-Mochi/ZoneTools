// File: Systems/ZoneToolSystem.ExistingRoads.cs
// Purpose: Update Existing Roads tool (hover + select + apply zoning mode to existing networks).
// Notes:
// - Tool is enabled/disabled by the UI panel ("Update Road").
// - Disabling returns to DefaultToolSystem (predictable; avoids returning to an unrelated prior tool).
// - Tool refuses to activate unless a non-null prefab is guaranteed (prevents null-prefab crashes in other mods).

namespace ZoningToolkit.Systems
{
    using Colossal.Serialization.Entities; // Purpose (OnGameLoadingComplete signature)
    using Game;                            // GameMode, ToolBaseSystem
    using Game.Common;                     // Updated tag
    using Game.Net;                        // Edge, SubBlock
    using Game.Prefabs;                    // PrefabBase, PrefabSystem
    using Game.Tools;                      // ToolSystem, DefaultToolSystem, NetToolSystem, raycast
    using Game.Zones;
    using Unity.Collections;               // NativeHashSet
    using Unity.Entities;                  // Entity, EntityCommandBuffer
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

        // Selected road entities (click/drag selection).
        private NativeHashSet<Entity> m_Selected;
        private int m_SelectedCount;

        // Currently hovered road entity (from raycast).
        private Entity m_Hovered;

        internal bool toolEnabled
        {
            get;
            private set;
        }

        // unique toolID string. Tool/UI infrastructure can reference tools by ID.
        public override string toolID => "ZoningToolkit.ExistingRoads";

        protected override void OnCreate( )
        {
            base.OnCreate();

            // Tool system should not run until explicitly enabled from UI.
            Enabled = false;

            m_ZTToolSystem = World.GetOrCreateSystemManaged<ToolSystem>();
            m_ZTDefaultToolSystem = World.GetOrCreateSystemManaged<DefaultToolSystem>();
            m_NetToolSystem = World.GetOrCreateSystemManaged<NetToolSystem>();
            m_ToolOutputBarrier = World.GetOrCreateSystemManaged<ToolOutputBarrier>();
            m_ZTPrefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();
            m_UISystem = World.GetOrCreateSystemManaged<ZoneToolBridgeUI>();

            m_Selected = new NativeHashSet<Entity>(128, Allocator.Persistent);
            m_SelectedCount = 0;
            m_Hovered = Entity.Null;

            toolEnabled = false;

            // Clear cached safe prefab on startup.
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

            // ToolBaseSystem actions drive selection behavior.
            applyAction.shouldBeEnabled = true;
            secondaryApplyAction.shouldBeEnabled = true;

            // Raycast requirements: roads only.
            requireNet = Layer.Road;
            requireZones = true;

            // Zoning does not apply underground; hide underground toggle.
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
        }

        public override void InitializeRaycast( )
        {
            base.InitializeRaycast();

            // Roads and lanes are sufficient to hit most road network entities.
            m_ToolRaycastSystem.typeMask = TypeMask.Net | TypeMask.Lanes;
            m_ToolRaycastSystem.netLayerMask = Layer.Road;
        }

        protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
        {
            base.OnGameLoadingComplete(purpose, mode);

            // Clear cached safe prefab across loads (prefabs can differ per session/playset).
            ResetSafePrefabForUI();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (!toolEnabled)
            {
                return inputDeps;
            }

            // Clear means selection overlay only; apply is performed on release.
            applyMode = ApplyMode.Clear;

            // Secondary action cycles mode (right-click by default).
            if (secondaryApplyAction.WasPressedThisFrame())
            {
                CycleZoningMode();
            }

            UpdateHover();

            // Primary action selects while pressed (supports click-and-drag selection).
            if (applyAction.WasPressedThisFrame() || applyAction.IsPressed())
            {
                AddHoveredToSelection();
            }

            // Apply once on release to avoid repeated writes while dragging.
            if (applyAction.WasReleasedThisFrame())
            {
                ApplySelection();
            }

            return inputDeps;
        }

        public override PrefabBase GetPrefab( )
        {
            // Tool panel queries this for icon/UX.
            // Non-null required when tool is active (some mods assume non-null and crash otherwise).
            return GetSafePrefabForUI();
        }

        public override bool TrySetPrefab(PrefabBase prefab)
        {
            // Prefab is not selectable from the tool panel for this tool.
            return false;
        }

        internal bool EnableTool( )
        {
            // Hard rule: do not activate tool unless GetPrefab can be guaranteed non-null.
            if (!TryResolveSafePrefabForUI(out _))
            {
                toolEnabled = false;
                Enabled = false;
                Mod.s_Log.Warn($"{Mod.ModTag} ExistingRoads enable refused: safe prefab not ready.");
                return false;
            }

            Enabled = true;
            m_ZTToolSystem.activeTool = this;
            toolEnabled = true;

            Mod.s_Log.Info($"{Mod.ModTag} ExistingRoads enabled");
            return true;
        }

        internal void DisableTool( )
        {
            toolEnabled = false;

            ClearSelection();
            m_Hovered = Entity.Null;

            // Predictable: always return to DefaultToolSystem.
            m_ZTToolSystem.activeTool = m_ZTDefaultToolSystem;

            // Stop tool updates when not active.
            Enabled = false;

            Mod.s_Log.Info($"{Mod.ModTag} ExistingRoads disabled");
        }

        private void CycleZoningMode( )
        {
            // Cycle order matches UI expectations.
            ZoningMode current = m_UISystem.CurrentZoningMode;
            ZoningMode next = current switch
            {
                ZoningMode.Default => ZoningMode.Left,
                ZoningMode.Left => ZoningMode.Right,
                ZoningMode.Right => ZoningMode.None,
                ZoningMode.None => ZoningMode.Default,
                _ => ZoningMode.Default
            };

            // Write into UI system so panel reflects the change immediately.
            m_UISystem.SetZoningModeFromTool(next);
        }

        private void UpdateHover( )
        {
            // Hover comes from a tool raycast result.
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

            // ECB used so changes apply safely at tool output barrier timing.
            EntityCommandBuffer ecb = m_ToolOutputBarrier.CreateCommandBuffer();

            foreach (Entity roadEntity in m_Selected)
            {
                AddOrSetZoningInfo(ecb, roadEntity, mode);
                TagSubBlocksForUpdate(ecb, roadEntity);
            }

            ClearSelection();
        }

        private bool TryGetRaycastRoad(out Entity entity)
        {
            entity = Entity.Null;

            // GetRaycastResult is provided by ToolBaseSystem.
            if (!base.GetRaycastResult(out Entity hit, out RaycastHit _))
            {
                return false;
            }

            // Edge indicates a net segment entity (roads are net entities).
            if (!EntityManager.HasComponent<Edge>(hit))
            {
                return false;
            }

            // SubBlock buffer required so blocks can be tagged for core update.
            if (!EntityManager.HasBuffer<SubBlock>(hit))
            {
                return false;
            }

            entity = hit;
            return true;
        }

        private void AddOrSetZoningInfo(EntityCommandBuffer ecb, Entity owner, ZoningMode mode)
        {
            // ZoningInfo stored on the road entity so future block edits inherit it.
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
            // Sub-blocks are zone blocks under the road; tag so core re-applies sizing once.
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

                // Marker triggers ZoneToolSystemCore update pass.
                if (!EntityManager.HasComponent<ZoningInfoUpdated>(blockEntity))
                {
                    ecb.AddComponent<ZoningInfoUpdated>(blockEntity);
                }

                // Updated tag helps other systems recognize a change occurred.
                if (!EntityManager.HasComponent<Updated>(blockEntity))
                {
                    ecb.AddComponent<Updated>(blockEntity);
                }
            }
        }
    }
}
