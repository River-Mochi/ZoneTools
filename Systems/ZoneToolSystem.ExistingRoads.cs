// File: Systems/ZoneToolSystem.ExistingRoads.cs
// Purpose: Update Existing Roads tool (hover + select + apply zoning mode to existing networks).
// Notes:
// - Tool is enabled/disabled by the UI panel ("Update Road").
// - Selected road entities are tagged; sub-blocks are marked for ZoneToolSystemCore to re-apply sizing.

namespace ZoningToolkit.Systems
{
    using Colossal.Entities;               // Debug build (listEntityComponents) in DEBUG
    using Colossal.Serialization.Entities; // Purpose (OnGameLoadingComplete signature)
    using Game;                            // GameMode, ToolBaseSystem
    using Game.Common;                     // Updated tag
    using Game.Net;                        // Edge, SubBlock, Owner
    using Game.Prefabs;                    // PrefabBase, PrefabSystem
    using Game.Tools;                      // ToolSystem, DefaultToolSystem, NetToolSystem, raycast
    using Game.Zones;                      // Zoning blocks / zone-relevant nets
    using Unity.Collections;               // NativeHashSet
    using Unity.Entities;                  // Entity, EntityCommandBuffer
    using Unity.Jobs;                      // JobHandle
    using ZoningToolkit.Components;        // ZoningInfo, ZoningInfoUpdated, ZoningMode
    using ZoningToolkit.Utils;             // DebugDumpPrefabIds in DEBUG build

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

        // Tool to restore when disabling Update Existing Roads.
        // This is why clicking update twice returns to the prior tool (bulldozer, net tool, etc.).
        private ToolBaseSystem? m_PreviousTool;

        public override string toolID => "Zone Tools Zoning Tool";

        protected override void OnCreate( )
        {
            base.OnCreate();

            // Tool stays disabled until explicitly enabled from UI.
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

            // Prefab safety is required so the CS2 tool panel does not crash when asking for GetPrefab().
            EnsureSafePrefabForUI();
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

            // Raycast requirements: roads only, zones required.
            requireNet = Layer.Road;
            requireZones = true;
            allowUnderground = false;

            EnsureSafePrefabForUI();
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

#if DEBUG
            DebugDumpPrefabIds("Crosswalk");
            DebugDumpPrefabIds("Wide");
            DebugDumpPrefabIds("Sidewalk");
            DebugDumpPrefabIds("Fence");
#endif
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            // Prefab safety is checked every update because other mods/game updates can invalidate prior UI prefabs.
            EnsureSafePrefabForUI();

            if (!toolEnabled)
            {
                return inputDeps;
            }

            // Clear means selection overlay only; actual apply is handled on release.
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
            // Tool panel queries this for icon/UX; must always return a non-null prefab.
            return GetSafePrefabForUI();
        }

        public override bool TrySetPrefab(PrefabBase prefab)
        {
            // Tool does not support changing prefab from the tool panel.
            return false;
        }

        internal void EnableTool( )
        {
            EnsureSafePrefabForUI();

            // Save current tool so disabling Update Existing Roads returns to it.
            m_PreviousTool = m_ZTToolSystem.activeTool;
            m_ZTToolSystem.activeTool = this;

            toolEnabled = true;

            Mod.s_Log.Info($"{Mod.ModTag} ExistingRoads enabled");
        }

        internal void DisableTool( )
        {
            toolEnabled = false;

            ClearSelection();
            m_Hovered = Entity.Null;

            // Return to prior tool when available; otherwise fall back to NetToolSystem.
            ToolBaseSystem? returnTool = m_PreviousTool;
            if (returnTool == null || returnTool == this)
            {
                returnTool = m_NetToolSystem;
            }

            m_ZTToolSystem.activeTool = returnTool;

            m_PreviousTool = null;

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

            // Writes into the UI system so the panel reflects the change immediately.
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

            // ECB is used so changes apply safely in tool output barrier timing.
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

            // Edge indicates a net segment-like entity (roads are net entities).
            if (!EntityManager.HasComponent<Edge>(hit))
            {
                return false;
            }

            // SubBlock buffer is required so blocks can be tagged for core update.
            if (!EntityManager.HasBuffer<SubBlock>(hit))
            {
                return false;
            }

#if DEBUG
            // Debug aid: dumps ECS components on the hit entity and prefab entity.
            this.listEntityComponents(hit);

            if (EntityManager.TryGetComponent<PrefabRef>(hit, out var pr))
            {
                Mod.s_Log.Debug($"{Mod.ModTag} Hit PrefabRef entity: {pr.m_Prefab}");
                this.listEntityComponents(pr.m_Prefab);
            }
#endif

            entity = hit;
            return true;
        }

        private void AddOrSetZoningInfo(EntityCommandBuffer ecb, Entity owner, ZoningMode mode)
        {
            // ZoningInfo is stored on the road entity so future block edits inherit it.
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
            // Sub-blocks are zone blocks under the road; they must be tagged so core re-applies sizing.
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

                // Marker triggers ZoneToolSystemCore update pass (one-shot).
                if (!EntityManager.HasComponent<ZoningInfoUpdated>(blockEntity))
                {
                    ecb.AddComponent<ZoningInfoUpdated>(blockEntity);
                }

                // Updated helps other systems recognize a change occurred.
                if (!EntityManager.HasComponent<Updated>(blockEntity))
                {
                    ecb.AddComponent<Updated>(blockEntity);
                }
            }
        }
    }
}
