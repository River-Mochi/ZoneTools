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
                { m_Setting.GetOptionGroupLocaleID(Setting.kActionsGrp),        "Protection" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kBindingsGrp),       "Key bindings" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kCompatibilityGrp),  "Compatibility" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kUiGrp),             "Visual Options" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kUsageGrp),          "USAGE" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutGrp),          "About" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutLinksGrp),     "Links" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kDebugGrp),          "DEBUG" },

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
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectOccupiedCells)),
                    "**[ ✓ ] enabled**, zone tools does not remove zoning cells on side of the road segment that already has a building.\n" +
                    "**[   ] disabled**, buildings could be condemned when changing the zoning under them."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectZonedCells)), "● Protect painted zoned cells" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectZonedCells)),
                    "**[ ✓ ] enabled**, Zone Tools does not change zoning depth/area on painted RCIO cells (empty or occupied).\n" +
                    "**[   ] disabled**, painted zone blocks could be removed."
                },

                // Compatibility
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ContourIconText)), "Contour Lines" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ContourIconText)), "" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ShowContourButton)), "Show button" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ShowContourButton)),
                    "● Contour terrain lines are viewable even when no road tool is open.\n" +
                    "Compatibility option\n" +
                    "**[ ] disable** this if another mod is used to show terrain lines (or to make the panel smaller).\n" +
                    "**[ ✓ ] enabled** shows the Contour button in the Zone Tools panel box.\n\n" +
                    "● Note: when you enable the **Update Road** icon, then the Topography button appears in the bottom-left vanilla location.\n" +
                    "   - When this [ ] option is disabled, then contour is still available while **Update Road** is ON since that activates the game's own vanilla tool for terrain lines view."
                },

                // UI
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.UseGlassPanel)), "◉ Glass panel style" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.UseGlassPanel)),
                    "**[ ✓ ] enabled**, use the clearer translucent panel style.\n" +
                    "**[   ] disabled**, use the vanilla style gray panel (darker).\n" +
                    "This is only a visual preference choice."
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DefaultPanelLocation)), "Panel default location" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DefaultPanelLocation)),
                    "Choose the **default location** of the Zone Tools panel (when opened from either top-left button or hotkey Shift+X).\n" +
                    "● Panel can be dragged after it opens by grabbing it in the title bar."
                },
                { m_Setting.GetEnumValueLocaleID(Setting.PanelLocation.ScreenTopLeft), "Top Left" },
                { m_Setting.GetEnumValueLocaleID(Setting.PanelLocation.ScreenBottomLeft), "Bottom Left" },
                { m_Setting.GetEnumValueLocaleID(Setting.PanelLocation.ScreenBottomRight), "Bottom Right" },
                
                // Keybinding option (Options → Mods)
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.TogglePanelBinding)), "Open/Close panel" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.TogglePanelBinding)),
                    "**Keyboard** shortcut to show or hide the Zone Tools panel\n" +
                    "This is the same as clicking the top-left menu icon to open the ZT panel.\n"+
                    "Reset to what you prefer."
                },

                // Usage toggle + multiline block
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ShowUsage)), "Show Instructions" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ShowUsage)),
                    "Show or hide the **usage instructions** below." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.UsageText)),
                    "<Open panel>\n" +
                    "1. Click the Zone Tools button in the top-left game menu, or press <Shift+X>, to open or close the panel.\n" +
                    "2. Drag the panel by its title bar if you want it somewhere else.\n\n" +
                    "<Existing roads>\n" +
                    "1. Turn <Update Road> on in the Zone Tools panel.\n" +
                    "2. Choose the zoning mode: Both, Left, Right, or None.\n" +
                    "3. Hover a road to preview which zoning cells will change.\n" +
                    "4. <LMB> applies the change. Hold and drag <LMB> across road sections, then release to apply.\n" +
                    "5. <RMB> cycles modes quickly while using the tool.\n\n" +
                    "<Protection>\n" +
                    "Protection options help avoid removing zoning under buildings or already painted zone cells.\n\n" +
                    "<Contour Lines>\n" +
                    "The Contour button shows terrain elevation lines from the Zone Tools panel."
                },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.UsageText)), "" },

                // Keybinding action name (Options → Keybindings)
                { m_Setting.GetBindingKeyLocaleID(Mod.kTogglePanelActionName), "Zone Tools – Toggle panel" },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpDebugReport)), "One-time snapshot report to log" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpDebugReport)),
                    "Writes a one-time long debug report to \n" +
                    "<Logs/ZoneTools.log> (debug use only).\n" +
                    "**Not needed for normal game play**;\n" +
                    "creates a large log (you can delete later)."
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenLog)), "Open Log" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenLog)),
                    "Opens <Logs/ZoneTools.log> if it exists.\n" +
                    "If the log file is not there, it opens the **Logs/** folder instead."
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
