// File: Settings/Setting.cs
// Purpose: Options UI for Zone Tools (Actions + About + Keybinding).
// Notes:
// - Defaults are controlled by property initializers + SetDefaults().
// - FileLocation controls where settings are stored in LocalLow.

namespace ZoningToolkit
{
    using Colossal.IO.AssetDatabase;
    using Game.Input;        // ProxyBinding, BindingKeyboard
    using Game.Modding;       // IMod
    using Game.Settings;      // ModSetting + Settings UI attributes
    using System;             // Exception
    using UnityEngine;        // Application.OpenURL

    [FileLocation("ModsSettings/ZoneTools/ZoneTools")] // Saved settings path key (not a disk path).
    [SettingsUITabOrder(kActionsTab, kAboutTab)]
    [SettingsUIGroupOrder(kActionsGroup, kBindingsGroup, kAboutGroup, kAboutLinksGroup)]
    [SettingsUIShowGroupName(kAboutLinksGroup)] // Only show “Links” header on About tab.
    [SettingsUIKeyboardAction(Mod.kTogglePanelActionName, ActionType.Button, usages: new[] { "Game" })]
    public sealed class Setting : ModSetting
    {
        // Tabs (tab IDs must match locale keys)
        public const string kActionsTab = "Actions";
        public const string kAboutTab = "About";

        // Groups (group IDs must match locale keys)
        public const string kActionsGroup = "Actions";
        public const string kBindingsGroup = "Key bindings";
        public const string kAboutGroup = "About";
        public const string kAboutLinksGroup = "Links";

        // Defaults (first install + “Reset to defaults”)
        // NOTE: Keep defaults TRUE protects cities from accidental damage.
        private const bool kDefaultProtectOccupiedCells = true;
        private const bool kDefaultProtectZonedCells = true;

        // External links
        private const string kUrlParadox =
            "https://mods.paradoxplaza.com/authors/River-mochi/cities_skylines_2?games=cities_skylines_2&orderBy=desc&sortBy=best&time=alltime";

        public Setting(IMod mod)
            : base(mod)
        {
        }

        public override void SetDefaults( )
        {
            // Called by the game when resetting settings to defaults.
            ProtectOccupiedCells = kDefaultProtectOccupiedCells;
            ProtectZonedCells = kDefaultProtectZonedCells;

            // Ensure binding has a concrete value when resetting.
            TogglePanelBinding = new ProxyBinding();
        }

        // ----- ABOUT TAB -----

        [SettingsUISection(kAboutTab, kAboutGroup)]
        public string ModName => Mod.ModName;

        [SettingsUISection(kAboutTab, kAboutGroup)]
        public string VersionText =>
#if DEBUG
            Mod.ModVersion + " (DEBUG)";
#else
            Mod.ModVersion;
#endif

        [SettingsUIButtonGroup(kAboutLinksGroup)]
        [SettingsUIButton]
        [SettingsUISection(kAboutTab, kAboutLinksGroup)]
        public bool OpenParadox
        {
            set
            {
                if (!value)
                {
                    return;
                }

                TryOpenUrl(kUrlParadox);
            }
        }

        // ----- ACTIONS TAB -----

        [SettingsUISection(kActionsTab, kActionsGroup)]
        public bool ProtectOccupiedCells { get; set; } = kDefaultProtectOccupiedCells;

        [SettingsUISection(kActionsTab, kActionsGroup)]
        public bool ProtectZonedCells { get; set; } = kDefaultProtectZonedCells;

        // ----- KEYBINDINGS (Actions tab) -----

        [SettingsUISection(kActionsTab, kBindingsGroup)]
        [SettingsUIKeyboardBinding(BindingKeyboard.X, Mod.kTogglePanelActionName, shift: true)]
        public ProxyBinding TogglePanelBinding { get; set; } = new ProxyBinding();

        // ----- Helpers -----

        private static void TryOpenUrl(string url)
        {
            try
            {
                Application.OpenURL(url);
            }
            catch (Exception)
            {
                // Ignore: failing to open a browser must not break the Options UI.
            }
        }
    }
}
