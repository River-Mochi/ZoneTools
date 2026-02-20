// File: Systems/ZoneToolSystem.BridgeUI.cs
// Purpose: Bridges Zone Tools ECS state with the in-game UI (panel + tool enable/disable + mode sync).

namespace ZoningToolkit.Systems
{
    using System;
    using Colossal.UI.Binding;
    using Game;
    using Game.Prefabs;
    using Game.Rendering;
    using Game.Tools;
    using Game.UI;
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
        private bool m_AutoOpenedForRoadTools;

        public override GameMode gameMode => GameMode.Game;

        protected override void OnCreate()
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
                toolEnabled = false,
            };

            m_AutoOpenedForRoadTools = false;

            if (m_ToolSystem != null)
            {
                m_ToolSystem.EventPrefabChanged =
                    (Action<PrefabBase>)Delegate.Combine(
                        m_ToolSystem.EventPrefabChanged,
                        new Action<PrefabBase>(OnPrefabChanged));

                m_ToolSystem.EventToolChanged =
                    (Action<ToolBaseSystem>)Delegate.Combine(
                        m_ToolSystem.EventToolChanged,
                        new Action<ToolBaseSystem>(OnToolChanged));
            }

            // C# -> UI
            AddUpdateBinding(new GetterValueBinding<string>(
                kGroup,
                "zoning_mode",
                () => m_UIState.zoningMode.ToString()));

            AddUpdateBinding(new GetterValueBinding<bool>(
                kGroup,
                "apply_to_new_roads",
                () => m_UIState.applyToNewRoads));

            AddUpdateBinding(new GetterValueBinding<bool>(
                kGroup,
                "tool_enabled",
                () => m_UIState.toolEnabled));

            AddUpdateBinding(new GetterValueBinding<bool>(
                kGroup,
                "visible",
                () => m_UIState.visible));

            AddUpdateBinding(new GetterValueBinding<bool>(
                kGroup,
                "photomode",
                () => m_PhotoMode != null && m_PhotoMode.Enabled));

            // UI -> C#
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
                "apply_to_new_roads",
                value =>
                {
                    m_UIState.applyToNewRoads = value;
                }));

            AddBinding(new TriggerBinding<bool>(
                kGroup,
                "tool_enabled",
                enabled =>
                {
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

            AddBinding(new TriggerBinding<bool>(
                kGroup,
                "toggle_panel",
                _ =>
                {
                    TogglePanelFromHotkey();
                }));
        }

        internal void TogglePanelFromHotkey()
        {
            bool newVisible = !m_UIState.visible;
            m_UIState.visible = newVisible;

            m_AutoOpenedForRoadTools = false;

            if (!newVisible && m_Tool != null && m_Tool.toolEnabled)
            {
                ToggleTool(false);
                m_UIState.toolEnabled = false;
            }
        }

        internal ZoningMode CurrentZoningMode => m_UIState.zoningMode;

        internal bool ApplyToNewRoads => m_UIState.applyToNewRoads;

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

            bool isZonableRoadTool = IsZonableRoadTool(tool);

            if (isZonableRoadTool && m_Tool.toolEnabled)
            {
                m_Tool.DisableTool();
            }

            HandleAutoPanelForRoadTools(isZonableRoadTool, "tool change");
        }

        private void OnPrefabChanged(PrefabBase prefab)
        {
            if (m_ToolSystem == null)
            {
                return;
            }

            if (m_ToolSystem.activeTool is not NetToolSystem)
            {
                return;
            }

            bool isZonableRoadPrefab =
                prefab is RoadPrefab roadPrefab &&
                roadPrefab.m_ZoneBlock != null;

            HandleAutoPanelForRoadTools(isZonableRoadPrefab, "prefab change");
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (m_ToolSystem == null || m_Tool == null || m_PhotoMode == null || m_ZoningSystem == null)
            {
                return;
            }

            if ((!m_UIState.visible || m_PhotoMode.Enabled) && m_Tool.toolEnabled)
            {
                ToggleTool(false);
                m_UIState.toolEnabled = false;
            }

            bool autoOpen = Mod.Settings?.AutoOpenPanelForRoadTools ?? true;
            if (autoOpen && m_AutoOpenedForRoadTools)
            {
                bool stillZonable = IsZonableRoadTool(m_ToolSystem.activeTool);
                if (!stillZonable)
                {
                    HandleAutoPanelForRoadTools(false, "update check");
                }
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

        private void HandleAutoPanelForRoadTools(bool isZonableRoadTool, string reason)
        {
            bool autoOpen = Mod.Settings?.AutoOpenPanelForRoadTools ?? true;
            if (!autoOpen)
            {
                m_AutoOpenedForRoadTools = false;
                return;
            }

            if (isZonableRoadTool)
            {
                if (!m_UIState.visible)
                {
                    m_UIState.visible = true;
                    m_AutoOpenedForRoadTools = true;
                }
            }
            else
            {
                if (m_AutoOpenedForRoadTools && m_UIState.visible)
                {
                    m_UIState.visible = false;
                    m_AutoOpenedForRoadTools = false;

                    if (m_Tool != null && m_Tool.toolEnabled)
                    {
                        ToggleTool(false);
                        m_UIState.toolEnabled = false;
                    }
                }
            }
        }
    }
}
