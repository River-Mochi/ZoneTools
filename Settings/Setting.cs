// File: Settings/Setting.cs
// Purpose: Options UI for Zone Tools – Actions + About + keybinding + debug report + compatibility toggles.

namespace ZoningToolkit
{
    using Colossal.IO.AssetDatabase; // FileLocation
    using Game.Input;                // ProxyBinding, keybinding attributes
    using Game.Modding;              // IMod
    using Game.Settings;             // ModSetting + SettingsUI* attributes
    using System;                    // Exception
    using UnityEngine;               // Application.OpenURL

    [FileLocation("ModsSettings/ZoneTools/ZoneTools")]
    [SettingsUITabOrder(kActionsTab, kAboutTab)]
    [SettingsUIGroupOrder(kActionsGrp, kBindingsGrp, kCompatibilityGrp, kUiGrp, kAboutGrp, kAboutLinksGrp, kDebugGrp)]
    [SettingsUIShowGroupName(kUiGrp, kAboutLinksGrp, kDebugGrp)]
    [SettingsUIKeyboardAction(Mod.kTogglePanelActionName, ActionType.Button, usages: new[] { "Game" })]
    public sealed class Setting : ModSetting
    {
        public enum PanelLocation
        {
            ScreenTopLeft,
            ScreenBottomLeft,
            ScreenBottomRight
        }

        // Tabs
        public const string kActionsTab = "Actions";
        public const string kAboutTab = "About";

        // Groups
        public const string kActionsGrp = "Actions";
        public const string kBindingsGrp = "Key bindings";
        public const string kCompatibilityGrp = "Compatibility";
        public const string kUiGrp = "VisualOptions";
        public const string kAboutGrp = "About";
        public const string kAboutLinksGrp = "Links";
        public const string kDebugGrp = "Debug only";
        public const string kDebugButtonsRow = "DebugButtonsRow";

        private const string kUrlParadox =
            "https://mods.paradoxplaza.com/authors/River-mochi/cities_skylines_2?games=cities_skylines_2&orderBy=desc&sortBy=best&time=alltime";

        public Setting(IMod mod)
            : base(mod)
        {
        }

        public override void SetDefaults( )
        {
            ProtectOccupiedCells = true;
            ProtectZonedCells = true;
            ShowContourButton = true;
            UseGlassPanel = true;
            DefaultPanelLocation = PanelLocation.ScreenBottomLeft;

            TogglePanelBinding = new ProxyBinding { };
        }

        // ----- ACTIONS TAB -----

        [SettingsUISection(kActionsTab, kActionsGrp)]
        public bool ProtectOccupiedCells { get; set; } = true;

        [SettingsUISection(kActionsTab, kActionsGrp)]
        public bool ProtectZonedCells { get; set; } = true;

        // Keybindings
        [SettingsUISection(kActionsTab, kBindingsGrp)]
        [SettingsUIKeyboardBinding(BindingKeyboard.X, Mod.kTogglePanelActionName, shift: true)]
        public ProxyBinding TogglePanelBinding { get; set; } = new ProxyBinding { };

        // Compatibility
        [SettingsUISection(kActionsTab, kCompatibilityGrp)]
        public bool ShowContourButton { get; set; } = true;

        // UI
        [SettingsUISection(kActionsTab, kUiGrp)]
        public bool UseGlassPanel { get; set; } = true;

        [SettingsUISection(kActionsTab, kUiGrp)]
        public PanelLocation DefaultPanelLocation { get; set; } = PanelLocation.ScreenBottomLeft;

        // ----- ABOUT TAB -----

        [SettingsUISection(kAboutTab, kAboutGrp)]
        public string ModName => Mod.ModName;

        [SettingsUISection(kAboutTab, kAboutGrp)]
        public string VersionText =>
#if DEBUG
            Mod.ModVersion + " (DEBUG)";
#else
            Mod.ModVersion;
#endif

        [SettingsUIButtonGroup(kAboutLinksGrp)]
        [SettingsUIButton]
        [SettingsUISection(kAboutTab, kAboutLinksGrp)]
        public bool OpenParadox
        {
            set
            {
                if (!value)
                    return;

                TryOpenUrl(kUrlParadox);
            }
        }

        [SettingsUISection(kAboutTab, kDebugGrp)]
        [SettingsUIButtonGroup(kDebugButtonsRow)]
        [SettingsUIButton]
        public bool DumpDebugReport
        {
            set
            {
                if (!value)
                    return;

                Mod.RequestDebugReport();
            }
        }

        [SettingsUISection(kAboutTab, kDebugGrp)]
        [SettingsUIButtonGroup(kDebugButtonsRow)]
        [SettingsUIButton]
        public bool OpenLog
        {
            set
            {
                if (!value)
                    return;

                ShellOpen.OpenModLogOrLogsFolder();
            }
        }

        // ----- HELPERS -----

        private static void TryOpenUrl(string url)
        {
            try
            {
                Application.OpenURL(url);
            }
            catch (Exception)
            {
            }
        }
    }
}
