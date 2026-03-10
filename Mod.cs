// File: Mod.cs
// Purpose: Entry point for Zone Tools.
// - Registers settings + localization
// - Registers ECS systems (core logic, UI bridge, tools)
// - Creates the rebindable Shift+X hotkey via CO InputManager
// - Keeps release logs clean; verbose logs stay in DEBUG-only paths

namespace ZoningToolkit
{
    using Colossal;                       // IDictionarySource
    using Colossal.IO.AssetDatabase;      // AssetDatabase.global.LoadSettings
    using Colossal.Localization;          // LocalizationManager
    using Colossal.Logging;               // ILog, LogManager, Level
    using CS2HonuShared;                  // LogUtils
    using Game;                           // UpdateSystem, SystemUpdatePhase
    using Game.Input;                     // ProxyAction
    using Game.Modding;                   // IMod
    using Game.SceneFlow;                 // GameManager
    using System;                         // Exception
    using System.Reflection;              // Assembly
    using System.Threading;               // Interlocked, Volatile
    using ZoningToolkit.Systems;

    public sealed class Mod : IMod
    {
        // ---- Metadata ----
        public const string ModName = "Zone Tools";
        public const string ModId = "ZoneTools";
        public const string ModTag = "[ZT]";

        // CO InputManager action name for the panel toggle keybinding.
        // Must match [SettingsUIKeyboardAction] in Setting.cs.
        public const string kTogglePanelActionName = "ZoneToolsTogglePanel";

        // Main mod logger. Writes to Logs/ZoneTools.log
        public static readonly ILog s_Log = LogManager.GetLogger(ModId);

        // Assembly version source of truth (3-part) for logs.
        // Controlled by <Version> in ZoneTools.csproj.
        public static readonly string ModVersion =
            Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "1.0.0";

#if DEBUG
        private const string BuildTag = "[DEBUG]";
#else
        private const string BuildTag = "[RELEASE]";
#endif

        // Prevents double banners when reloading playsets / mod reload events.
        private static bool s_BannerLogged;

        // Debug report request counter (incremented by Options UI button).
        private static int s_DebugReportRequestId;

        // Active settings instance (Options UI).
        // Used from systems via Mod.Settings.
        public static Setting? Settings
        {
            get;
            private set;
        }

        // ProxyAction resolved from settings keybinding registration.
        // ZoneToolSystemKeybind reads this and checks for presses.
        public static ProxyAction? TogglePanelAction
        {
            get;
            private set;
        }

        static Mod( )
        {
#if DEBUG
            // Dev builds: surface errors in UI.
            s_Log.SetShowsErrorsInUI(true);
#else
            // Release builds: keep UI popups quiet.
            s_Log.SetShowsErrorsInUI(false);
#endif
        }

        // DEBUG-only info log helper.
        // Keeps release logs free of tool enable/disable chatter.
        internal static void DebugLog(string message)
        {
#if DEBUG
            s_Log.Info(message);
#else
            _ = message;
#endif
        }

        /// <summary>
        /// Called by Setting.cs button. Uses a counter so multiple clicks can be detected safely.
        /// </summary>
        public static void RequestDebugReport( )
        {
            Interlocked.Increment(ref s_DebugReportRequestId);
        }

        /// <summary>
        /// Read by ZoneToolBridgeUI (UIUpdate) to run the dump once per click.
        /// </summary>
        internal static int DebugReportRequestId => Volatile.Read(ref s_DebugReportRequestId);

        public void OnLoad(UpdateSystem updateSystem)
        {
            // One-time banner line.
            if (!s_BannerLogged)
            {
                s_BannerLogged = true;
                LogUtils.TryLog(
                    s_Log,
                    Level.Info,
                    ( ) => $"{ModName} {ModTag} v{ModVersion} OnLoad {BuildTag}");
            }

            // ----- Settings + localization -----

            // Create settings object (holds user choices + keybinds).
            Setting setting = new Setting(this);
            Settings = setting;

            // Register locale sources. Failures are caught inside AddLocaleSource.
            AddLocaleSource("en-US", new LocaleEN(setting));
            AddLocaleSource("fr-FR", new LocaleFR(setting));
            AddLocaleSource("es-ES", new LocaleES(setting));
            AddLocaleSource("de-DE", new LocaleDE(setting));
            AddLocaleSource("it-IT", new LocaleIT(setting));
            AddLocaleSource("ja-JP", new LocaleJA(setting));
            AddLocaleSource("ko-KR", new LocaleKO(setting));
            AddLocaleSource("pl-PL", new LocalePL(setting));
            AddLocaleSource("pt-BR", new LocalePT_BR(setting));
            AddLocaleSource("zh-HANS", new LocaleZH_CN(setting));   // Simplified Chinese
            AddLocaleSource("zh-HANT", new LocaleZH_HANT(setting)); // Traditional Chinese

            // Load saved settings values from disk.
            // Third parameter provides defaults when the file does not exist yet.
            AssetDatabase.global.LoadSettings(ModId, setting, new Setting(this));

            // Register the Options UI entries (tabs, groups, toggles, keybind UI).
            setting.RegisterInOptionsUI();

            // ----- Keybindings -----

            try
            {
                // Register key actions and default keyboard binding (Shift+X by default).
                setting.RegisterKeyBindings();

                // Resolve the action by name so systems can read it without duplicating input code.
                TogglePanelAction = setting.GetAction(kTogglePanelActionName);
                if (TogglePanelAction != null)
                {
                    TogglePanelAction.shouldBeEnabled = true;
                    LogUtils.TryLog(
                        s_Log,
                        Level.Info,
                        ( ) => $"{ModTag} Keybinding '{kTogglePanelActionName}' enabled (default Shift+X).");
                }
                else
                {
                    LogUtils.TryLog(
                        s_Log,
                        Level.Warn,
                        ( ) => $"{ModTag} Keybinding action '{kTogglePanelActionName}' not found.");
                }
            }
            catch (Exception ex)
            {
                // Mod should still load even if keybinding registration fails.
                LogUtils.TryLog(
                    s_Log,
                    Level.Warn,
                    ( ) => $"{ModTag} Keybinding setup skipped: {ex.GetType().Name}: {ex.Message}");
            }

            // ----- ECS systems -----

            // Existing roads update tool (click/drag on roads to apply zoning mode).
            updateSystem.UpdateAt<ZoneToolSystemExistingRoads>(SystemUpdatePhase.ToolUpdate);

            // Core zoning logic for new roads (Net tool / block changes).
            updateSystem.UpdateAt<ZoneToolSystemCore>(SystemUpdatePhase.Modification4B);

            // UI bridge (C# <-> React panel): bindings, tool toggle, panel visibility.
            updateSystem.UpdateAt<ZoneToolBridgeUI>(SystemUpdatePhase.UIUpdate);

            // Hotkey listener (Shift+X): toggles panel visibility.
            updateSystem.UpdateAt<ZoneToolSystemKeybind>(SystemUpdatePhase.ToolUpdate);
        }

        public void OnDispose( )
        {
            DebugLog("OnDispose");

            // Disable action so it stops consuming input when mod unloads.
            if (TogglePanelAction != null)
            {
                TogglePanelAction.shouldBeEnabled = false;
                TogglePanelAction = null;
            }

            // Unregister Options UI and release settings reference.
            if (Settings != null)
            {
                Settings.UnregisterInOptionsUI();
                Settings = null;
            }
        }

        // ------------------------------------------------------------------
        // Localization helper
        // ------------------------------------------------------------------

        // Wrapper for LocalizationManager.AddSource that catches exceptions.
        // Prevents locale issues from breaking mod load.
        private static void AddLocaleSource(string localeId, IDictionarySource source)
        {
            if (string.IsNullOrEmpty(localeId) || source == null)
            {
                return;
            }

            LocalizationManager? lm = GameManager.instance?.localizationManager;
            if (lm == null)
            {
                LogUtils.WarnOnce(
                    s_Log,
                    "LocalizationManagerMissing",
                    ( ) => $"{ModTag} AddLocaleSource skipped: no LocalizationManager.");
                return;
            }

            try
            {
                lm.AddSource(localeId, source);
            }
            catch (Exception ex)
            {
                LogUtils.TryLog(
                    s_Log,
                    Level.Warn,
                    ( ) => $"{ModTag} AddLocaleSource failed for '{localeId}': {ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
