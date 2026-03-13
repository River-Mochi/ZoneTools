// File: Systems/ZoneToolSystem.BridgeUI.cs
// Purpose: Bridges Zone Tools ECS state with the in-game UI (panel, menu button, and tool enable/disable).
// Notes:
// - Contour lines are derived from ToolSystem.activeTool.selectedSnap (game code).
// - When activeTool is null (or DefaultToolSystem), game resolves contour OFF each frame.
// - Contour toggle succeeds only when the active tool supports Snap.ContourLines.

namespace ZoningToolkit.Systems
{
    using Colossal.UI.Binding;       // GetterValueBinding, TriggerBinding
    using Game;                      // GameMode
    using Game.Prefabs;              // RoadPrefab
    using Game.Rendering;            // PhotoModeRenderSystem
    using Game.Tools;                // ToolSystem, ToolBaseSystem, DefaultToolSystem, NetToolSystem, Snap
    using Game.UI;                   // UISystemBase
    using System;                    // Action, Delegate, Enum
    using Unity.Entities;            // World
    using ZoningToolkit.Components;  // ZoningMode

    internal struct UIState
    {
        public bool visible;
        public ZoningMode zoningMode;
        public bool applyToNewRoads;
        public bool toolEnabled;
    }

    internal sealed partial class ZoneToolBridgeUI : UISystemBase
    {
        private const string kGroup = "zoning_adjuster_ui_namespace";

        private ZoneToolSystemCore? m_ZoningSystem;
        private ToolSystem? m_ToolSystem;
        private NetToolSystem? m_NetToolSystem;
        private ZoneToolSystemExistingRoads? m_Tool;
        private PhotoModeRenderSystem? m_PhotoMode;

        private UIState m_UIState;
        private int m_LastDebugReportRequestId;
        private Action<ToolBaseSystem>? m_ToolChangedHandler;

        public override GameMode gameMode => GameMode.Game;

        protected override void OnCreate( )
        {
            base.OnCreate();

            m_ZoningSystem = World.GetOrCreateSystemManaged<ZoneToolSystemCore>();
            m_ToolSystem = World.GetOrCreateSystemManaged<ToolSystem>();
            m_NetToolSystem = World.GetOrCreateSystemManaged<NetToolSystem>();
            m_PhotoMode = World.GetOrCreateSystemManaged<PhotoModeRenderSystem>();
            m_Tool = World.GetOrCreateSystemManaged<ZoneToolSystemExistingRoads>();

            m_UIState = new UIState
            {
                visible = false,
                zoningMode = ZoningMode.Default,
                applyToNewRoads = false,
                toolEnabled = false
            };

            if (m_ToolSystem != null)
            {
                m_ToolChangedHandler = OnToolChanged;
                m_ToolSystem.EventToolChanged =
                    (Action<ToolBaseSystem>) Delegate.Combine(
                        m_ToolSystem.EventToolChanged,
                        m_ToolChangedHandler);
            }

            AddUpdateBinding(new GetterValueBinding<string>(
                kGroup,
                "zoning_mode",
                ( ) => m_UIState.zoningMode.ToString()));

            AddUpdateBinding(new GetterValueBinding<bool>(
                kGroup,
                "tool_enabled",
                ( ) => m_UIState.toolEnabled));

            AddUpdateBinding(new GetterValueBinding<bool>(
                kGroup,
                "visible",
                ( ) => m_UIState.visible));

            AddUpdateBinding(new GetterValueBinding<bool>(
                kGroup,
                "photomode",
                ( ) => m_PhotoMode != null && m_PhotoMode.Enabled));

            AddUpdateBinding(new GetterValueBinding<bool>(
                kGroup,
                "contour_enabled",
                ( ) => GetContourEnabledFromActiveTool()));

            AddUpdateBinding(new GetterValueBinding<bool>(
                kGroup,
                "contour_button_visible",
                ( ) => Mod.Settings != null && Mod.Settings.ShowContourButton));

            AddUpdateBinding(new GetterValueBinding<bool>(
                kGroup,
                "contour_tooloptions_visible",
                ( ) => m_Tool != null && m_UIState.visible && !m_PhotoMode!.Enabled && (m_Tool.toolEnabled || m_Tool.HasPendingEnableAfterContourHostStop)));

            AddUpdateBinding(new GetterValueBinding<bool>(
                kGroup,
                "use_glass_panel",
                ( ) => Mod.Settings == null || Mod.Settings.UseGlassPanel));

            AddBinding(new TriggerBinding<string>(
                kGroup,
                "zoning_mode_update",
                zoningModeString =>
                {
                    if (Enum.TryParse<ZoningMode>(zoningModeString, out ZoningMode mode))
                    {
                        m_UIState.zoningMode = mode;
                    }
                }));

            AddBinding(new TriggerBinding<bool>(
                kGroup,
                "tool_enabled",
                enabled =>
                {
                    ToggleUpdateRoadTool(enabled);
                    m_UIState.toolEnabled = m_Tool != null && m_Tool.toolEnabled;
                }));

            AddBinding(new TriggerBinding<bool>(
                kGroup,
                "toggle_panel",
                _ =>
                {
                    TogglePanelFromHotkey();
                }));

            AddBinding(new TriggerBinding<bool>(
                kGroup,
                "ToggleContourLines",
                _ =>
                {
                    if (Mod.Settings == null || !Mod.Settings.ShowContourButton)
                    {
                        return;
                    }

                    ToggleContourLinesFromUI();
                }));
        }

        protected override void OnDestroy( )
        {
            if (m_ToolSystem != null && m_ToolChangedHandler != null)
            {
                m_ToolSystem.EventToolChanged =
                    (Action<ToolBaseSystem>) Delegate.Remove(
                        m_ToolSystem.EventToolChanged,
                        m_ToolChangedHandler);
            }

            base.OnDestroy();
        }

        protected override void OnUpdate( )
        {
            base.OnUpdate();

            if (m_ToolSystem == null || m_Tool == null || m_PhotoMode == null || m_ZoningSystem == null)
            {
                return;
            }

            int req = Mod.DebugReportRequestId;
            if (req != m_LastDebugReportRequestId)
            {
                m_LastDebugReportRequestId = req;
                m_Tool.DumpDebugReportOnDemand();
            }

            // Hidden panel or Photo Mode must cancel both:
            // - active real tool
            // - pending host -> real tool promotion
            if ((!m_UIState.visible || m_PhotoMode.Enabled) &&
                (m_Tool.toolEnabled || m_Tool.HasPendingEnableAfterContourHostStop))
            {
                ToggleUpdateRoadTool(false);
                m_UIState.toolEnabled = false;
            }

            // Complete the second half of host -> real tool promotion.
            // Wait until ToolSystem has processed the intermediate return to default tool.
            if (m_UIState.visible &&
                !m_PhotoMode.Enabled &&
                m_Tool.HasPendingEnableAfterContourHostStop &&
                !m_ToolSystem.fullUpdateRequired)
            {
                m_Tool.ContinuePendingEnableAfterContourHostStop();
            }

            if (m_UIState.zoningMode != m_ZoningSystem.zoningMode)
            {
                m_ZoningSystem.zoningMode = m_UIState.zoningMode;
            }

            if (m_UIState.toolEnabled != m_Tool.toolEnabled)
            {
                m_UIState.toolEnabled = m_Tool.toolEnabled;
            }
        }

        internal ZoningMode CurrentZoningMode => m_UIState.zoningMode;

        internal void SetZoningModeFromTool(ZoningMode mode)
        {
            if (m_UIState.zoningMode != mode)
            {
                m_UIState.zoningMode = mode;
            }
        }

        internal void TogglePanelFromHotkey( )
        {
            bool newVisible = !m_UIState.visible;
            m_UIState.visible = newVisible;

            if (!newVisible && m_Tool != null &&
                (m_Tool.toolEnabled || m_Tool.HasPendingEnableAfterContourHostStop))
            {
                ToggleUpdateRoadTool(false);
                m_UIState.toolEnabled = false;
            }
        }

        private void ToggleUpdateRoadTool(bool enable)
        {
            if (m_Tool == null)
            {
                return;
            }

            if (enable)
            {
                bool ok = m_Tool.EnableTool();
                if (!ok)
                {
                    m_UIState.toolEnabled = false;
                }
            }
            else
            {
                m_Tool.DisableTool();
            }
        }

        private void OnToolChanged(ToolBaseSystem tool)
        {
            // Road build tool active => Update Road tool must not stay enabled.
            if (m_Tool != null && m_UIState.toolEnabled && IsRoadBuildTool(tool))
            {
                ToggleUpdateRoadTool(false);
                m_UIState.toolEnabled = false;
            }
        }

        private static bool IsRoadBuildTool(ToolBaseSystem tool)
        {
            if (tool is not NetToolSystem netTool)
            {
                return false;
            }

            return netTool.GetPrefab() is RoadPrefab;
        }

        private void ToggleContourLinesFromUI( )
        {
            bool actual = GetContourEnabledFromActiveTool();
            bool next = !actual;

            // No active tool or DefaultToolSystem => enable inert host tool when enabling contour.
            if (next)
            {
                if (EnsureContourHostIfNoTool())
                {
                    TrySetContourOnActiveTool(true);
                }

                return;
            }

            TrySetContourOnActiveTool(false);
            ReleaseContourHostIfOnlyHost();
        }

        private bool EnsureContourHostIfNoTool( )
        {
            if (m_ToolSystem == null || m_Tool == null)
            {
                return false;
            }

            ToolBaseSystem? active = m_ToolSystem.activeTool;
            if (active != null && active is not DefaultToolSystem)
            {
                return true;
            }

            return m_Tool.EnableContourHost();
        }

        private void ReleaseContourHostIfOnlyHost( )
        {
            if (m_ToolSystem == null || m_Tool == null)
            {
                return;
            }

            // Contour host should not remain active as the only reason for an active tool.
            // Update Road mode is controlled separately; host disable is a no-op if Update Road is enabled.
            ToolBaseSystem? active = m_ToolSystem.activeTool;
            if (active == m_Tool)
            {
                m_Tool.DisableContourHost();
            }
        }

        private bool TrySetContourOnActiveTool(bool contourOn)
        {
            if (m_ToolSystem == null)
            {
                return false;
            }

            ToolBaseSystem? active = m_ToolSystem.activeTool;
            if (active == null)
            {
                return false;
            }

            active.GetAvailableSnapMask(out Snap onMask, out Snap offMask);
            if (contourOn && (onMask & Snap.ContourLines) == 0)
            {
                return false;
            }

            Snap snap = active.selectedSnap;
            if (contourOn)
            {
                snap |= Snap.ContourLines;
            }
            else
            {
                snap &= ~Snap.ContourLines;
            }

            active.selectedSnap = snap;

            // Sync vanilla road-tool snap with the ZT panel state.
            if (m_NetToolSystem != null && (active is NetToolSystem || active == m_Tool))
            {
                m_NetToolSystem.selectedSnap = snap;
            }

            Snap actual = ToolBaseSystem.GetActualSnap(active.selectedSnap, onMask, offMask);
            if (contourOn)
            {
                return (actual & Snap.ContourLines) != 0;
            }

            return (actual & Snap.ContourLines) == 0;
        }

        private bool GetContourEnabledFromActiveTool( )
        {
            if (m_ToolSystem == null)
            {
                return false;
            }

            ToolBaseSystem? active = m_ToolSystem.activeTool;
            if (active == null)
            {
                return false;
            }

            active.GetAvailableSnapMask(out Snap onMask, out Snap offMask);
            Snap actual = ToolBaseSystem.GetActualSnap(active.selectedSnap, onMask, offMask);
            return (actual & Snap.ContourLines) != 0;
        }
    }
}
