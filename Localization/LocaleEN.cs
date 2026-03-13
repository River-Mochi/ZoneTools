// File: Localization/LocaleEN.cs
// Purpose: English (en-US) localization entries for Zone Tools.
// Notes:
// - Settings UI strings generated via ModSetting helper IDs.
// - React UI strings use fixed keys.

namespace ZoningToolkit
{
    using Colossal;
    using System.Collections.Generic;

    public sealed class LocaleEN : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleEN(Setting setting)
        {
            m_Setting = setting;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(
            IList<IDictionaryEntryError> errors,
            Dictionary<string, int> indexCounts)
        {
            Dictionary<string, string> d = new Dictionary<string, string>
            {
                // Options title
                { m_Setting.GetSettingsLocaleID(), Mod.ModName + " " + Mod.ModTag },

                // Tabs
                { m_Setting.GetOptionTabLocaleID(Setting.kActionsTab), "Actions" },
                { m_Setting.GetOptionTabLocaleID(Setting.kAboutTab),   "About" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.kActionsGrp),        "Actions" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kBindingsGrp),       "Key bindings" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kCompatibilityGrp),  "Compatibility" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kUiGrp),             "UI" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutGrp),          "About" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutLinksGrp),     "Links" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kDebugGrp),          "Debug only" },

                // About fields
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ModName)), "Mod name" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ModName)),  "Display name of this mod." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.VersionText)), "Version" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.VersionText)),  "Current Zone Tools version." },

                // About links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadox)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadox)),  "Open the author's Paradox Mods page." },

                // Actions toggles
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectOccupiedCells)), "● Protect occupied cells (has buildings)" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectOccupiedCells)),
                    "**[ ✓ ] enabled**, Zone Tools does not change zoning depth/area on cells that already have a building.\n" +
                    "**[   ] disabled**, buildings could be condemned when changing the zoning under them."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectZonedCells)), "● Protect zoned-but-empty cells" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectZonedCells)),
                    "**[ ✓ ] enabled**, Zone Tools does not change zoning depth/area on cells that are already zoned (even if empty).\n" +
                    "**[   ] disabled**, already zoned cells (painted RCIO) could be overwritten when using Zone Tools."
                },

                // Compatibility
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ShowContourButton)), "◉ Contour button" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ShowContourButton)),
                    "**[ ✓ ] enabled**, show the Contour button in the Zone Tools panel.\n\n" +
                    "● This lets contour lines be enabled even when no vanilla road tool is open.\n" +
                    "● **Update Road**: when enabled, the vanilla Topography button is visible in the bottom-left vanilla location.\n" +
                    "[ ] disable this if a smaller panel is preferred or another mod is used for terrain lines.\n" +
                    "When disabled, contour is only available while **Update Road** is ON."
                },

                // UI
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.UseGlassPanel)), "◉ Glass panel style" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.UseGlassPanel)),
                    "**[ ✓ ] enabled**, use the clearer translucent panel style.\n" +
                    "**[   ] disabled**, use the vanilla-style gray panel (darker).\n" +
                    "Both styles avoid blur; this is only a visual preference toggle."
                },

                // Keybinding option (Options → Mods)
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.TogglePanelBinding)), "Toggle panel" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.TogglePanelBinding)),
                    "**Keyboard** shortcut to show or hide the Zone Tools panel (same as clicking the top-left menu icon)."
                },

                // Keybinding action name (Options → Keybindings)
                { m_Setting.GetBindingKeyLocaleID(Mod.kTogglePanelActionName), "Zone Tools – Toggle panel" },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpDebugReport)), "Verbose debug report to log" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpDebugReport)),
                    "Write a one-time longer debug report to Logs/ZoneTools.log (debug use only).\n" +
                    "**Not needed for normal game play**; creates a huge log (you can delete)."
                },

                // -----------------------------------------------------------------
                // React UI strings
                // -----------------------------------------------------------------
                { "ZoneTools.UI.Tooltip.TitleBar", "Drag panel from the title bar." },

                { "ZoneTools.UI.UpdateRoad", "Update Road" },
                { "ZoneTools.UI.Tooltip.UpdateRoad", "Edit Existing roads ON / OFF" },

                { "ZoneTools.UI.Contour", "Contour" },
                { "ZoneTools.UI.Tooltip.Contour", "Show Terrain lines." },

                { "ZoneTools.UI.Tooltip.ModeDefault", "Both sides" },
                { "ZoneTools.UI.Tooltip.ModeLeft",    "Left only" },
                { "ZoneTools.UI.Tooltip.ModeRight",   "Right only" },
                { "ZoneTools.UI.Tooltip.ModeNone",    "None" },

                // GameTopLeft button tooltip
                { "ZoneTools.UI.Fab.Title", "Zone Tools" },
                { "ZoneTools.UI.Fab.Desc",  "Modify zones along roads.\nShortcut: Shift+X (set in Options)\nPanel can move." },
            };

            return d;
        }

        public void Unload( )
        {
        }
    }
}
