// File: Systems/ZoneToolSystem.BridgeUI.cs
// Purpose: Bridges Zone Tools ECS state with the in-game UI (panel, menu button, and tool enable/disable).

namespace ZoningToolkit.Systems
{
    using Colossal.UI.Binding;       // ValueBinding, TriggerBinding
    using Game;                      // GameMode
    using Game.Prefabs;              // RoadPrefab (tool-type checks)
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
        public bool applyToNewRoads;
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

            // ECS systems used by the UI bridge.
            m_ZoningSystem = World.GetOrCreateSystemManaged<ZoneToolSystemCore>();
            m_ToolSystem = World.GetOrCreateSystemManaged<ToolSystem>();
            m_PhotoMode = World.GetOrCreateSystemManaged<PhotoModeRenderSystem>();
            m_Tool = World.GetOrCreateSystemManaged<ZoneToolSystemExistingRoads>();

            // Local UI mirror state (C# side).
            m_UIState = new UIState
            {
                visible = false,
                zoningMode = ZoningMode.Default,
                applyToNewRoads = false,
                toolEnabled = false
            };

            // Listen for tool switches so the update tool can be disabled when a road build tool is selected.
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

            // ----- UI -> C# bindings (events/triggers) -----

            AddBinding(new TriggerBinding<string>(
                kGroup,
                "zoning_mode_update",
                zoningModeString =>
                {
                    if (Enum.TryParse<ZoningMode>(zoningModeString, out ZoningMode mode))
                    {
                        // Updates the shared state; core system is synced during OnUpdate().
                        Mod.Debug($"{Mod.ModTag} UI zoning mode updated to {mode}");
                        m_UIState.zoningMode = mode;
                    }
                }));

            AddBinding(new TriggerBinding<bool>(
                kGroup,
                "tool_enabled",
                enabled =>
                {
                    // User toggled “Update Road” in the panel.
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
                    // FAB button uses this same trigger.
                    Mod.Debug($"{Mod.ModTag} UI toggle_panel trigger");
                    TogglePanelFromHotkey();
                }));
        }

        protected override void OnDestroy( )
        {
            // Unhook tool-change event to avoid dangling delegates during unload/reload.
            if (m_ToolSystem != null)
            {
                m_ToolSystem.EventToolChanged =
                    (Action<ToolBaseSystem>) Delegate.Remove(
                        m_ToolSystem.EventToolChanged,
                        new Action<ToolBaseSystem>(OnToolChanged));
            }

            base.OnDestroy();
        }

        // Called from hotkey system and from UI trigger.
        // Only controls panel visibility; tool enable/disable is handled separately.
        internal void TogglePanelFromHotkey( )
        {
            bool newVisible = !m_UIState.visible;
            m_UIState.visible = newVisible;

            Mod.Debug($"{Mod.ModTag} Panel visible = {m_UIState.visible}");

            // When panel is hidden, disable the update tool to stop input and overlays.
            if (!newVisible && m_Tool != null && m_Tool.toolEnabled)
            {
                ToggleTool(false);
                m_UIState.toolEnabled = false;
            }
        }

        // Read by the tool to apply/reflect the current mode.
        internal ZoningMode CurrentZoningMode => m_UIState.zoningMode;

        // Called by the tool to sync mode changes (right-click cycle) back into UI state.
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
                // EnableTool can refuse when safe prefab is not ready (early load / odd playset).
                bool enabled = m_Tool.EnableTool();
                if (!enabled)
                {
                    // Panel stays open; UI toggle is forced back off by state sync.
                    Mod.Debug($"{Mod.ModTag} EnableTool refused (safe prefab not ready)");
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
            if (m_ToolSystem == null || m_Tool == null)
            {
                return;
            }

            // If a zonable road build tool is selected, disable Update Existing Roads helper.
            if (IsZonableRoadTool(tool) && m_Tool.toolEnabled)
            {
                Mod.Debug($"{Mod.ModTag} Road build tool selected -> disabling update tool");
                m_Tool.DisableTool();
            }
        }

        protected override void OnUpdate( )
        {
            base.OnUpdate();

            if (m_ToolSystem == null || m_Tool == null || m_PhotoMode == null || m_ZoningSystem == null)
            {
                return;
            }

            // Options “dump report” button: run once per click.
            int req = Mod.DebugReportRequestId;
            if (req != m_LastDebugReportRequestId)
            {
                m_LastDebugReportRequestId = req;
                m_Tool.DumpDebugReportOnDemand();
            }

            // Photo mode or panel hidden: tool must not stay active.
            if ((!m_UIState.visible || m_PhotoMode.Enabled) && m_Tool.toolEnabled)
            {
                ToggleTool(false);
                m_UIState.toolEnabled = false;
            }

            // Sync UI -> core system (mode used by core zoning logic).
            if (m_UIState.zoningMode != m_ZoningSystem.zoningMode)
            {
                m_ZoningSystem.zoningMode = m_UIState.zoningMode;
            }

            // Sync tool -> UI state (reflect refusal or external disables).
            if (m_UIState.toolEnabled != m_Tool.toolEnabled)
            {
                m_UIState.toolEnabled = m_Tool.toolEnabled;
            }
        }

        // Tool check: Net tool + road prefab with zone blocks.
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
