// File: Systems/ZoneToolSystem.BridgeUI.cs
// Purpose: Bridges Zone Tools ECS state with the in-game UI (panel, menu button, and tool enable/disable).
// Notes:
// - Adds ContourEnabled + ToggleContourLines bindings for the Existing Roads tool.
// - Contour toggle is tool-local (selectedSnap flag) and shown only when the update tool is active.

namespace ZoningToolkit.Systems
{
    using Colossal.UI.Binding;       // GetterValueBinding, TriggerBinding
    using Game;                      // GameMode
    using Game.Prefabs;              // RoadPrefab
    using Game.Rendering;            // PhotoModeRenderSystem
    using Game.Tools;                // ToolSystem, ToolBaseSystem, NetToolSystem
    using Game.UI;                   // UISystemBase
    using System;                    // Action, Delegate, Enum
    using Unity.Entities;            // World
    using ZoningToolkit.Components;  // ZoningMode

    internal struct UIState
    {
        public bool visible;
        public ZoningMode zoningMode;
        public bool applyToNewRoads; // reserved; keep only if UI uses it
        public bool toolEnabled;
    }

    internal sealed partial class ZoneToolBridgeUI : UISystemBase
    {
        private const string kGroup = "zoning_adjuster_ui_namespace";

        private ZoneToolSystemCore? m_ZoningSystem;
        private ToolSystem? m_ToolSystem;
        private ZoneToolSystemExistingRoads? m_Tool;
        private PhotoModeRenderSystem? m_PhotoMode;

        private UIState m_UIState;

        // Tracks clicks on the Options “dump report” button (works in Release too).
        private int m_LastDebugReportRequestId;

        public override GameMode gameMode => GameMode.Game;

        protected override void OnCreate( )
        {
            base.OnCreate();

            m_ZoningSystem = World.GetOrCreateSystemManaged<ZoneToolSystemCore>();
            m_ToolSystem = World.GetOrCreateSystemManaged<ToolSystem>();
            m_PhotoMode = World.GetOrCreateSystemManaged<PhotoModeRenderSystem>();
            m_Tool = World.GetOrCreateSystemManaged<ZoneToolSystemExistingRoads>();

            m_UIState = new UIState
            {
                visible = false,
                zoningMode = ZoningMode.Default,
                applyToNewRoads = false,
                toolEnabled = false
            };

            // Tool switches: disable update tool when selecting a road build tool.
            if (m_ToolSystem != null)
            {
                m_ToolSystem.EventToolChanged =
                    (Action<ToolBaseSystem>) Delegate.Combine(
                        m_ToolSystem.EventToolChanged,
                        new Action<ToolBaseSystem>(OnToolChanged));
            }

            // ----- C# -> UI bindings (polling) -----

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
                ( ) => m_PhotoMode?.Enabled ?? false));

            // Contour state for the Existing Roads tool icon.
            // Returns false when tool is not active so the UI never shows "selected" while inactive.
            AddUpdateBinding(new GetterValueBinding<bool>(
                kGroup,
                "contour_enabled",
                ( ) => m_Tool != null && m_Tool.toolEnabled && m_Tool.ContourEnabled));

            // ----- UI -> C# bindings (events/triggers) -----

            AddBinding(new TriggerBinding<string>(
                kGroup,
                "zoning_mode_update",
                zoningModeString =>
                {
                    if (Enum.TryParse<ZoningMode>(zoningModeString, out ZoningMode mode))
                    {
                        Mod.Debug($"{Mod.ModTag} UI zoning mode updated to {mode}");
                        m_UIState.zoningMode = mode;
                    }
                }));

            AddBinding(new TriggerBinding<bool>(
                kGroup,
                "tool_enabled",
                enabled =>
                {
                    Mod.Debug($"{Mod.ModTag} UI tool_enabled requested: {enabled}");

                    ToggleTool(enabled);

                    // Reflect reality back to UI after attempting the toggle.
                    m_UIState.toolEnabled = m_Tool != null && m_Tool.toolEnabled;
                }));

            AddBinding(new TriggerBinding<bool>(
                kGroup,
                "toggle_panel",
                _ =>
                {
                    Mod.Debug($"{Mod.ModTag} UI toggle_panel trigger");
                    TogglePanelFromHotkey();
                }));

            // Contour toggle (EZ-style): flips Snap.ContourLines on the active tool.
            // UI triggers: engine.trigger("zoning_adjuster_ui_namespace.ToggleContourLines", true)
            AddBinding(new TriggerBinding<bool>(
                kGroup,
                "ToggleContourLines",
                _ =>
                {
                    if (m_Tool == null || !m_Tool.toolEnabled)
                    {
                        return;
                    }

                    m_Tool.ToggleContourLines();
                }));
        }

        protected override void OnDestroy( )
        {
            if (m_ToolSystem != null)
            {
                m_ToolSystem.EventToolChanged =
                    (Action<ToolBaseSystem>) Delegate.Remove(
                        m_ToolSystem.EventToolChanged,
                        new Action<ToolBaseSystem>(OnToolChanged));
            }

            base.OnDestroy();
        }

        internal void TogglePanelFromHotkey( )
        {
            bool newVisible = !m_UIState.visible;
            m_UIState.visible = newVisible;

            Mod.Debug($"{Mod.ModTag} Panel visible = {m_UIState.visible}");

            // Hide panel => disable update tool.
            if (!newVisible && m_Tool != null && m_Tool.toolEnabled)
            {
                ToggleTool(false);
                m_UIState.toolEnabled = false;
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

        private void ToggleTool(bool enable)
        {
            if (m_Tool == null)
            {
                return;
            }

            if (enable)
            {
                bool enabledOk = m_Tool.EnableTool();
                if (!enabledOk)
                {
                    Mod.Debug($"{Mod.ModTag} EnableTool refused (safe prefab not ready)");
                    m_UIState.toolEnabled = false;
                    return;
                }

                m_UIState.toolEnabled = true;
            }
            else
            {
                m_Tool.DisableTool();
                m_UIState.toolEnabled = false;
            }
        }

        private void OnToolChanged(ToolBaseSystem tool)
        {
            if (m_ToolSystem == null || m_Tool == null)
            {
                return;
            }

            // If a zonable road build tool is selected, disable Update Existing Roads helper.
            if (IsZonableRoadTool(tool) && m_Tool.toolEnabled)
            {
                Mod.Debug($"{Mod.ModTag} Road build tool selected -> disabling update tool");
                m_Tool.DisableTool();
                m_UIState.toolEnabled = false;
            }
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

            // Photo mode or panel hidden => tool must not stay active.
            if ((!m_UIState.visible || m_PhotoMode.Enabled) && m_Tool.toolEnabled)
            {
                ToggleTool(false);
                m_UIState.toolEnabled = false;
            }

            // UI -> core mode.
            if (m_UIState.zoningMode != m_ZoningSystem.zoningMode)
            {
                m_ZoningSystem.zoningMode = m_UIState.zoningMode;
            }

            // Tool -> UI state (reflect refusal or external disables).
            if (m_UIState.toolEnabled != m_Tool.toolEnabled)
            {
                m_UIState.toolEnabled = m_Tool.toolEnabled;
            }
        }

        private static bool IsZonableRoadTool(ToolBaseSystem tool)
        {
            if (tool is not NetToolSystem netTool)
            {
                return false;
            }

            if (netTool.GetPrefab() is not RoadPrefab roadPrefab)
            {
                return false;
            }

            return roadPrefab.m_ZoneBlock != null;
        }
    }
}
