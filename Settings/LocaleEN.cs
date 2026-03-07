// File: Settings/LocaleEN.cs
// Purpose: English (en-US) localization entries for Zone Tools.
// Notes:
// - Settings UI strings generated via ModSetting helper IDs.
// - React UI strings use fixed keys (ZoneTools.UI.*).

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
            // Dictionary is rebuilt when CS2 requests entries.
            // CS2 manages lifecycle; Unload() can remain empty.
            Dictionary<string, string> d = new Dictionary<string, string>
            {
                // Options title (keep consistent with Mod.cs constants)
                { m_Setting.GetSettingsLocaleID(), Mod.ModName + " " + Mod.ModTag },

                // Tabs
                { m_Setting.GetOptionTabLocaleID(Setting.kActionsTab), "Actions" },
                { m_Setting.GetOptionTabLocaleID(Setting.kAboutTab),   "About"   },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.kActionsGroup),    "Actions"      },
                { m_Setting.GetOptionGroupLocaleID(Setting.kBindingsGroup),   "Key bindings" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutGroup),      "About"        },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutLinksGroup), "Links"        },

                // About fields
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ModName)),    "Mod name" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ModName)),     "Display name of this mod." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.VersionText)), "Version" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.VersionText)),  "Current Zone Tools version." },

                // About links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadox)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadox)),  "Open the author's Paradox Mods page." },

                // Actions toggles
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectOccupiedCells)), "Protect occupied cells (has buildings)" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectOccupiedCells)),
                    "**[ ✓ ] enabled**, Zone Tools does not change zoning depth/area on cells that already have a building.\n" +
                    "**[   ] disabled**, buildings could be condemned when changing the zoning under them."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectZonedCells)), "Protect zoned-but-empty cells" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectZonedCells)),
                    "**[ ✓ ] enabled**, Zone Tools does not change zoning depth/area on cells that are already zoned (even if empty).\n" +
                    "**[   ] disabled**, already zoned cells (painted RCIO) could be overwritten when using the Zone Tools."
                },

                // Keybinding option (Options → Mods)
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.TogglePanelBinding)), "Toggle panel" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.TogglePanelBinding)),
                    "**Keyboard** shortcut to show or hide the Zone Tools panel (same as clicking the top-left menu icon)."
                },

                // Keybinding action name (Options → Keybindings)
                { m_Setting.GetBindingKeyLocaleID(Mod.kTogglePanelActionName), "Zone Tools – Toggle panel" },

                // -----------------------------------------------------------------
                // React UI strings (React panel)
                // -----------------------------------------------------------------
                { "ZoneTools.UI.UpdateRoad", "Update Road" },

                { "ZoneTools.UI.Tooltip.UpdateRoad",
                  "Toggle update panel ON / OFF (for existing roads).\n\n" +
                  "If enabling fails, open new road build tool one time first (i.e, small roads)." },

                { "ZoneTools.UI.Tooltip.ModeDefault", "Both (default)" },
                { "ZoneTools.UI.Tooltip.ModeLeft",    "Left"          },
                { "ZoneTools.UI.Tooltip.ModeRight",   "Right"         },
                { "ZoneTools.UI.Tooltip.ModeNone",    "None"          }
            };

            return d;
        }

        public void Unload( )
        {
            // No cleanup required; CS2 manages locale lifecycle.
        }
    }
}
