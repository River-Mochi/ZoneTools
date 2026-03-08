// File: Mod.cs
// Purpose: Entry point for Zone Tools.
// - Registers settings + localization
// - Registers ECS systems (core logic, UI bridge, tools)
// - Creates the rebindable Shift+X hotkey via CO InputManager

namespace ZoningToolkit
{
    using Colossal;                       // IDictionarySource
    using Colossal.IO.AssetDatabase;      // AssetDatabase.global.LoadSettings
    using Colossal.Localization;          // LocalizationManager
    using Colossal.Logging;              // ILog, LogManager
    using Game;                          // UpdateSystem, SystemUpdatePhase
    using Game.Input;                    // ProxyAction
    using Game.Modding;                  // IMod
    using Game.SceneFlow;                // GameManager
    using System;
    using System.Reflection;
    using System.Threading;             // Interlocked (debug report request counter)
    using ZoningToolkit.Systems;

    public sealed class Mod : IMod
    {
        // ---- Metadata ------ 
        public const string ModName = "Zone Tools";
        public const string ModId = "ZoneTools";
        public const string ModTag = "[ZT]";

        // Assembly version source of truth (3-part) for logs.
        // Controlled by <Version> in ZoneTools.csproj.
        public static readonly string ModVersion =
            Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "1.0.0";

        // CO InputManager action name for the panel toggle keybinding.
        // Must match [SettingsUIKeyboardAction] in Setting.cs and the UI/Keybind system usage.
        public const string kTogglePanelActionName = "ZoneToolsTogglePanel";

        // Prevents double banners when reloading playsets / mod reload events.
        private static bool s_BannerLogged;
        // Debug report request counter (incremented by Options UI button).
        private static int s_DebugReportRequestId;

        // Main mod logger. Writes to mod named file
        public static readonly ILog s_Log = LogManager.GetLogger(ModId);

        // Debug logging gate:
        // - DEBUG: verbose logging enabled by default
        // - RELEASE: verbose logging disabled by default
        // This avoids massive logs for players while keeping deep logs available during development.
        public static bool DebugLoggingEnabled
        {
            get; set;
        } =
#if DEBUG
            true;
#else
            false;
#endif
        // Verbose log helper. Route through one place to control spam.
        public static void Debug(string message)
        {
            if (!DebugLoggingEnabled)
            {
                return;
            }

            s_Log.Info(message);
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

        // Active settings instance (Options UI).
        // Used from systems via Mod.Settings.
        public static Setting? Settings
        {
            get; private set;
        }

        // ProxyAction resolved from settings keybinding registration.
        // ZoneToolSystemKeybind reads this and checks for presses.
        public static ProxyAction? TogglePanelAction
        {
            get; private set;
        }

        static Mod( )
        {
#if DEBUG
            // Show errors as UI popups in Debug builds (dev-friendly).
            s_Log.SetShowsErrorsInUI(true);
#else
            // Avoid UI popups in Release builds (player-friendly).
            s_Log.SetShowsErrorsInUI(false);
#endif
        }

        public void OnLoad(UpdateSystem updateSystem)
        {
            // One-time banner line.
            if (!s_BannerLogged)
            {
                s_BannerLogged = true;
                s_Log.Info($"{ModName} {ModTag} v{ModVersion} OnLoad");
            }

            // ----- Settings + localization -----------------------------------

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

            // Loads saved settings values from disk.
            // Third parameter provides defaults when the file does not exist yet.
            AssetDatabase.global.LoadSettings(ModId, setting, new Setting(this));

            // Registers the Options UI entries (tabs, groups, toggles, keybind UI).
            setting.RegisterInOptionsUI();

            // ----- Keybindings -----------

            // Registers key actions and default keyboard binding (Shift+X by default).
            try
            {
                setting.RegisterKeyBindings();

                // Resolve the action by name so systems can read it without duplicating input code.
                TogglePanelAction = setting.GetAction(kTogglePanelActionName);
                if (TogglePanelAction != null)
                {
                    TogglePanelAction.shouldBeEnabled = true;
                    s_Log.Info($"{ModTag} Keybinding '{kTogglePanelActionName}' enabled (default Shift+X).");
                }
                else
                {
                    s_Log.Warn($"{ModTag} Keybinding action '{kTogglePanelActionName}' not found.");
                }
            }
            catch (Exception ex)
            {
                // Mod should still load even if keybinding registration fails.
                s_Log.Warn($"{ModTag} Keybinding setup skipped: {ex.GetType().Name}: {ex.Message}");
            }

            // ----- ECS systems ------------

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
            s_Log.Info(nameof(OnDispose));

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
            if (string.IsNullOrEmpty(localeId))
            {
                return;
            }

            LocalizationManager? lm = GameManager.instance?.localizationManager;
            if (lm == null)
            {
                s_Log.Warn($"AddLocaleSource: No LocalizationManager; cannot add source for '{localeId}'.");
                return;
            }

            try
            {
                lm.AddSource(localeId, source);
            }
            catch (Exception ex)
            {
                s_Log.Warn($"AddLocaleSource: AddSource for '{localeId}' failed: {ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
