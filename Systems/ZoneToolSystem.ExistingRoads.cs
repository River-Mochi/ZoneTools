// File: Systems/ZoneToolSystem.ExistingRoads.cs
// Purpose: Update Existing Roads tool (hover + select + apply zoning mode to existing networks).
// Notes: Preview logic moved to Preview.cs and Highlight logic moved to Highlight.cs.

namespace ZoningToolkit.Systems
{
    using Colossal.Entities;
    using Game.Common;
    using Game.Net;
    using Game.Tools;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Jobs;
    using ZoningToolkit.Components;

    internal sealed partial class ZoneToolSystemExistingRoads : ToolBaseSystem
    {
        private ToolSystem m_ZTToolSystem = null!;
        private DefaultToolSystem m_DefaultTool = null!;
        private NetToolSystem m_NetTool = null!;
        private ToolOutputBarrier m_ToolOutputBarrier = null!;
        private ZoneToolBridgeUI m_UISystem = null!;
        private ZoneToolSystemHighlight m_HighlightSystem = null!;
        private ZoneToolSystemPreview m_PreviewSystem = null!; // Add Preview system reference

        private NativeHashSet<Entity> m_Selected;
        private int m_SelectedCount;
        private Entity m_Hovered;

        private ToolBaseSystem? m_PreviousTool;

        internal bool toolEnabled
        {
            get; private set;
        }

        public override string toolID => "Zone Tools Zoning Tool";

        protected override void OnCreate()
        {
            base.OnCreate();

            m_ZTToolSystem = World.GetOrCreateSystemManaged<ToolSystem>();
            m_DefaultTool = World.GetOrCreateSystemManaged<DefaultToolSystem>();
            m_NetTool = World.GetOrCreateSystemManaged<NetToolSystem>();
            m_ToolOutputBarrier = World.GetOrCreateSystemManaged<ToolOutputBarrier>();
            m_UISystem = World.GetOrCreateSystemManaged<ZoneToolBridgeUI>();
            m_HighlightSystem = World.GetOrCreateSystemManaged<ZoneToolSystemHighlight>();
            m_PreviewSystem = World.GetOrCreateSystemManaged<ZoneToolSystemPreview>(); // Initialize Preview system

            m_Selected = new NativeHashSet<Entity>(128, Allocator.Persistent);
            m_SelectedCount = 0;
            m_Hovered = Entity.Null;

            toolEnabled = false;
        }

        protected override void OnDestroy()
        {
            if (m_Selected.IsCreated)
            {
                m_Selected.Dispose();
            }

            base.OnDestroy();
        }

        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            toolEnabled = true;

            applyAction.shouldBeEnabled = true;
            secondaryApplyAction.shouldBeEnabled = true;

            requireNet = Layer.Road;
            requireZones = true;
            allowUnderground = true;
        }

        protected override void OnStopRunning()
        {
            // Restore preview and highlight cleanup
            m_PreviewSystem.RestoreAndClearPreview();  // Delegate restoring preview logic
            ClearSelectionAndHover();

            toolEnabled = false;

            base.OnStopRunning();
        }

        public override void InitializeRaycast()
        {
            base.InitializeRaycast();

            m_ToolRaycastSystem.typeMask = TypeMask.Net;
            m_ToolRaycastSystem.netLayerMask = Layer.Road;
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (m_ZTToolSystem.activeTool != this)
            {
                return inputDeps;
            }

            applyMode = ApplyMode.None;

            if (cancelAction.WasPressedThisFrame())
            {
                m_PreviewSystem.RestoreAndClearPreview();  // Delegate restore preview on cancel
                ClearSelectionAndHover();
                return inputDeps;
            }

            if (secondaryApplyAction.WasPressedThisFrame())
            {
                CycleZoningMode();
                m_PreviewSystem.MarkPreviewDirty();  // Mark preview dirty on mode change
            }

            m_PreviewSystem.UpdatePreview();  // Update preview for hover or selection

            if (applyAction.WasPressedThisFrame() || applyAction.IsPressed())
            {
                AddHoveredToSelection();
            }

            if (applyAction.WasReleasedThisFrame())
            {
                EntityCommandBuffer ecb = m_ToolOutputBarrier.CreateCommandBuffer();
                ApplySelection(ecb);

                // Discard preview backups after apply
                m_PreviewSystem.DiscardPreviewBackups();

                m_PreviewSystem.UpdatePreview();  // Update preview after apply
            }

            return inputDeps;
        }

        private void AddHoveredToSelection()
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

                m_PreviewSystem.MarkPreviewDirty();  // Refresh preview when selection changes
            }
        }

        private void ClearSelectionAndHover()
        {
            if (m_Selected.IsCreated && m_SelectedCount > 0)
            {
                foreach (Entity e in m_Selected)
                {
                    m_HighlightSystem.RequestHighlight(e, false);
                }
            }

            if (m_Selected.IsCreated)
            {
                m_Selected.Clear();
            }

            m_SelectedCount = 0;

            if (m_Hovered != Entity.Null)
            {
                m_HighlightSystem.RequestHighlight(m_Hovered, false);
                m_Hovered = Entity.Null;
            }
        }

        private void ApplySelection(EntityCommandBuffer ecb)
        {
            if (m_SelectedCount == 0)
            {
                return;
            }

            ZoningMode mode = m_UISystem.CurrentZoningMode;

            foreach (Entity roadEntity in m_Selected)
            {
                if (!EntityManager.Exists(roadEntity))
                {
                    continue;
                }

                if (!WouldChange(roadEntity))
                {
                    continue;
                }

                AddOrSetZoningInfo(ecb, roadEntity, mode);
                TagSubBlocksForUpdate(ecb, roadEntity);
            }

            m_Selected.Clear();
            m_SelectedCount = 0;
        }

        private bool WouldChange(Entity roadEntity)
        {
            ZoningMode desired = m_UISystem.CurrentZoningMode;
            ZoningMode current = ZoningMode.Default;
            if (EntityManager.TryGetComponent<ZoningInfo>(roadEntity, out var zi))
            {
                current = zi.zoningMode;
            }

            return current != desired;
        }
    }
}
