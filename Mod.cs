// File: Mod.cs
// Entry point for Zone Tools – logging, settings, localization, systems, and Shift+Z panel hotkey.

namespace ZoningToolkit
{
    using System;
    using System.Reflection;
    using Colossal;
    using Colossal.IO.AssetDatabase;
    using Colossal.Localization;
    using Colossal.Logging;
    using Game;
    using Game.Input;
    using Game.Modding;
    using Game.SceneFlow;
    using ZoningToolkit.Systems;

    public sealed class Mod : IMod
    {
        public const string ModName = "Zone Tools";
        public const string ModId = "ZoneTools";
        public const string ModTag = "[ZT]";

        public static readonly string ModVersion =
            Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "1.0.0";

        public const string kTogglePanelActionName = "ZoneToolsTogglePanel";

        private static bool s_BannerLogged;

        public static readonly ILog s_Log = LogManager.GetLogger(ModId);

        public static bool DebugLoggingEnabled
        {
            get; set;
        } =
#if DEBUG
            true;
#else
            false;
#endif

        public static void Debug(string message)
        {
            if (!DebugLoggingEnabled)
            {
                return;
            }

            s_Log.Info(message);
        }

        public static Setting? Settings
        {
            get; private set;
        }

        public static ProxyAction? TogglePanelAction
        {
            get; private set;
        }

        static Mod()
        {
#if DEBUG
            s_Log.SetShowsErrorsInUI(true);
#else
            s_Log.SetShowsErrorsInUI(false);
#endif
        }

        public void OnLoad(UpdateSystem updateSystem)
        {
            if (!s_BannerLogged)
            {
                s_BannerLogged = true;
                s_Log.Info($"{ModName} {ModTag} v{ModVersion} OnLoad");
            }

            var setting = new Setting(this);
            Settings = setting;

            AddLocaleSource("en-US", new LocaleEN(setting));
            AddLocaleSource("fr-FR", new LocaleFR(setting));
            AddLocaleSource("es-ES", new LocaleES(setting));
            AddLocaleSource("de-DE", new LocaleDE(setting));
            AddLocaleSource("it-IT", new LocaleIT(setting));
            AddLocaleSource("ja-JP", new LocaleJA(setting));
            AddLocaleSource("ko-KR", new LocaleKO(setting));
            AddLocaleSource("pl-PL", new LocalePL(setting));
            AddLocaleSource("pt-BR", new LocalePT_BR(setting));
            AddLocaleSource("zh-HANS", new LocaleZH_CN(setting));
            AddLocaleSource("zh-HANT", new LocaleZH_HANT(setting));

            AssetDatabase.global.LoadSettings(ModId, setting, new Setting(this));
            setting.RegisterInOptionsUI();

            try
            {
                setting.RegisterKeyBindings();

                TogglePanelAction = setting.GetAction(kTogglePanelActionName);
                if (TogglePanelAction != null)
                {
                    TogglePanelAction.shouldBeEnabled = true;
                    s_Log.Info($"{ModTag} Keybinding '{kTogglePanelActionName}' enabled (default Shift+Z).");
                }
                else
                {
                    s_Log.Warn($"{ModTag} Keybinding action '{kTogglePanelActionName}' not found.");
                }
            }
            catch (Exception ex)
            {
                s_Log.Warn($"{ModTag} Keybinding setup skipped: {ex.GetType().Name}: {ex.Message}");
            }

            // ----------------------------------------------------------------
            // ECS systems
            // ----------------------------------------------------------------

            // Hotkey system – listens to Shift+Z and toggles the panel.
            updateSystem.UpdateAt<ZoneToolSystemKeybind>(SystemUpdatePhase.ToolUpdate);

            // Existing-road update tool (hover/select/apply).
            updateSystem.UpdateAt<ZoneToolSystemExistingRoads>(SystemUpdatePhase.ToolUpdate);

            // Highlight apply/cleanup (runs after tool logic to reduce frame-lag).
            updateSystem.UpdateAt<ZoneToolSystemHighlight>(SystemUpdatePhase.ToolUpdate);

            // Cleanup for TempZoning (safety net; harmless even if TempZoning is rarely used).
            updateSystem.UpdateAt<ClearTempZoningWhenToolInactiveSystem>(SystemUpdatePhase.ToolUpdate);

            // Core zoning logic that applies the selected zoning mode to blocks.
            updateSystem.UpdateAt<ZoneToolSystemCore>(SystemUpdatePhase.Modification4B);

            // Cohtml UI bridge (C# <-> React panel).
            updateSystem.UpdateAt<ZoneToolBridgeUI>(SystemUpdatePhase.UIUpdate);
        }

        public void OnDispose()
        {
            s_Log.Info(nameof(OnDispose));

            if (TogglePanelAction != null)
            {
                TogglePanelAction.shouldBeEnabled = false;
                TogglePanelAction = null;
            }

            if (Settings != null)
            {
                Settings.UnregisterInOptionsUI();
                Settings = null;
            }
        }

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
