// File: Systems/ZoneToolSystem.Keybind.cs
// Purpose: Handles Zone Tools keybinding (Shift+X by default) via CO InputManager.

namespace ZoningToolkit.Systems
{
    using CS2HonuShared;    // LogUtils
    using Game;
    using Game.Input;       // ProxyAction
    using Unity.Entities;   // GameSystemBase

    /// <summary>
    /// Runs in ToolUpdate and listens to the CO ProxyAction registered in Setting.RegisterKeyBindings().
    /// When the action is pressed, it toggles the Zone Tools UI panel
    /// (same behavior as clicking the GameTopLeft button).
    /// </summary>
    public sealed partial class ZoneToolSystemKeybind : GameSystemBase
    {
        private ZoneToolBridgeUI? m_UISystem;
        private bool m_LoggedMissingAction;
        private bool m_LoggedMissingUISystem;

        protected override void OnCreate( )
        {
            base.OnCreate();

            m_UISystem = World.GetOrCreateSystemManaged<ZoneToolBridgeUI>();

#if DEBUG
            LogUtils.TryLog(Mod.s_Log, Colossal.Logging.Level.Info,
                () => $"{Mod.ModTag} ZoneToolSystemKeybind created.");
#endif
        }

        protected override void OnUpdate( )
        {
            if (m_UISystem == null)
            {
                if (!m_LoggedMissingUISystem)
                {
                    m_LoggedMissingUISystem = true;
                    LogUtils.WarnOnce(
                        Mod.s_Log,
                        "ZoneToolSystemKeybind.MissingUISystem",
                        ( ) => $"{Mod.ModTag} ZoneToolSystemKeybind: UI system is null in OnUpdate (unexpected).");
                }

                return;
            }

            ProxyAction? togglePanelAction = Mod.TogglePanelAction;
            if (togglePanelAction == null)
            {
                if (!m_LoggedMissingAction)
                {
                    m_LoggedMissingAction = true;
                    LogUtils.WarnOnce(
                        Mod.s_Log,
                        "ZoneToolSystemKeybind.MissingTogglePanelAction",
                        ( ) => $"{Mod.ModTag} ZoneToolSystemKeybind: TogglePanelAction is null in OnUpdate.");
                }

                return;
            }

            if (togglePanelAction.WasPressedThisFrame())
            {
#if DEBUG
                LogUtils.TryLog(Mod.s_Log, Colossal.Logging.Level.Info,
                    () => $"{Mod.ModTag} ZoneToolSystemKeybind: toggle pressed -> toggling panel.");
#endif
                m_UISystem.TogglePanelFromHotkey();
            }
        }
    }
}
