// File: Systems/ZoneToolSystem.Keybind.cs
// Purpose: Handles Zone Tools keybinding (Shift+X by default) via CO InputManager.
// Notes:
//   - actual keybind is defined in Setting.cs.
//   - this system gets/enables the ProxyAction because this is where it is used.
//   - release-edge handling acts like a normal click and avoids held-key repeats.

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
        private ProxyAction? m_TogglePanelAction;
        private bool m_LoggedMissingAction;
        private bool m_LoggedMissingUISystem;

        protected override void OnCreate( )
        {
            base.OnCreate();

            m_UISystem = World.GetOrCreateSystemManaged<ZoneToolBridgeUI>();
            m_TogglePanelAction = GetTogglePanelAction();

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

            if (m_TogglePanelAction == null)
            {
                m_TogglePanelAction = GetTogglePanelAction();
            }

            ProxyAction? togglePanelAction = m_TogglePanelAction;
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

            if (togglePanelAction.WasReleasedThisFrame())
            {
#if DEBUG
                LogUtils.TryLog(Mod.s_Log, Colossal.Logging.Level.Info,
                    () => $"{Mod.ModTag} ZoneToolSystemKeybind: toggle released -> toggling panel.");
#endif
                m_UISystem.TogglePanelFromHotkey();
            }
        }

        private static ProxyAction? GetTogglePanelAction( )
        {
            Setting? settings = Mod.Settings;
            if (settings == null)
            {
                return null;
            }

            ProxyAction? action = settings.GetAction(Mod.kTogglePanelActionName);
            if (action != null)
            {
                action.shouldBeEnabled = true;
            }

            return action;
        }
    }
}
