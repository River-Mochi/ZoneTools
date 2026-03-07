// File: Systems/ZoneToolSystem.BridgeUI.cs
// Purpose: Bridges Zone Tools ECS state with the in-game UI (panel, menu button, and tool enable/disable).

namespace ZoningToolkit.Systems
{
    using Colossal.UI.Binding;
    using Game;
    using Game.Prefabs;
    using Game.Rendering;
    using Game.Tools;
    using Game.UI;
    using System;
    using Unity.Entities;
    using ZoningToolkit.Components;

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

            // React to tool changes.
            if (m_ToolSystem != null)
            {
                m_ToolSystem.EventToolChanged =
                    (Action<ToolBaseSystem>) Delegate.Combine(
                        m_ToolSystem.EventToolChanged,
                        new Action<ToolBaseSystem>(OnToolChanged));
            }

            // C# -> UI bindings.
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

            // UI -> C# bindings.
            AddBinding(new TriggerBinding<string>(
                kGroup,
                "zoning_mode_update",
                zoningModeString =>
                {
                    if (Enum.TryParse<ZoningMode>(zoningModeString, out ZoningMode mode))
                    {
                        Mod.Debug($"{Mod.ModTag} Zone Tools UI: zoning mode updated to {mode}");
                        m_UIState.zoningMode = mode;
                    }
                }));

            AddBinding(new TriggerBinding<bool>(
                kGroup,
                "tool_enabled",
                enabled =>
                {
                    Mod.Debug($"{Mod.ModTag} Zone Tools UI: tool_enabled set to {enabled}");

                    // If tool refuses to enable (e.g., null-prefab safety), reflect reality back to UI.
                    ToggleTool(enabled);

                    if (m_Tool != null)
                    {
                        m_UIState.toolEnabled = m_Tool.toolEnabled;
                    }
                    else
                    {
                        m_UIState.toolEnabled = false;
                    }
                }));

            // FAB button toggle: same behaviour as keybind.
            AddBinding(new TriggerBinding<bool>(
                kGroup,
                "toggle_panel",
                _ =>
                {
                    Mod.Debug($"{Mod.ModTag} Zone Tools UI: toggle_panel trigger.");
                    TogglePanelFromHotkey();
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

        // Called from ZoneToolSystemKeybind and from UI via toggle_panel trigger.
        // Only toggles panel visibility; does not change the active tool.
        internal void TogglePanelFromHotkey( )
        {
            bool newVisible = !m_UIState.visible;
            m_UIState.visible = newVisible;

            Mod.Debug($"{Mod.ModTag} TogglePanelFromHotkey: m_UIState.visible = {m_UIState.visible}");

            // If panel is hidden, disable the update tool so input/overlays stop.
            if (!newVisible && m_Tool != null && m_Tool.toolEnabled)
            {
                Mod.Debug($"{Mod.ModTag} TogglePanelFromHotkey: panel hidden -> disabling update tool.");
                ToggleTool(false);
                m_UIState.toolEnabled = false;
            }
        }

        // Used by the zoning tool to read / change the active mode.
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
                m_Tool.EnableTool();
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

            // Disable the existing-road helper when a zonable road building tool is selected.
            bool isZonableRoadTool = IsZonableRoadTool(tool);

            if (isZonableRoadTool)
            {
                if (m_Tool.toolEnabled)
                {
                    Mod.Debug($"{Mod.ModTag} OnToolChanged: Net road tool selected -> disabling Zone Tool helper.");
                    m_Tool.DisableTool();
                }
            }
        }

        protected override void OnUpdate( )
        {
            base.OnUpdate();

            if (m_ToolSystem == null || m_Tool == null || m_PhotoMode == null || m_ZoningSystem == null)
            {
                return;
            }

            // If panel is not visible (or photo mode is on), the update tool must not remain active.
            if ((!m_UIState.visible || m_PhotoMode.Enabled) && m_Tool.toolEnabled)
            {
                Mod.Debug($"{Mod.ModTag} OnUpdate: panel hidden/photomode -> disabling update tool.");
                ToggleTool(false);
                m_UIState.toolEnabled = false;
            }

            // Sync UI -> system.
            if (m_UIState.zoningMode != m_ZoningSystem.zoningMode)
            {
                m_ZoningSystem.zoningMode = m_UIState.zoningMode;
            }

            // Sync tool -> UI.
            if (m_UIState.toolEnabled != m_Tool.toolEnabled)
            {
                m_UIState.toolEnabled = m_Tool.toolEnabled;
            }
        }

        // ----- Helpers -----

        private static bool IsZonableRoadTool(ToolBaseSystem tool)
        {
            if (tool == null)
            {
                return false;
            }

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
