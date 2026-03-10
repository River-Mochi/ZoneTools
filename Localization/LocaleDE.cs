// File: Localization/LocaleDE.cs
// Purpose: German (de-DE) localization entries for Zone Tools.
// Notes:
// - Settings UI strings generated via ModSetting helper IDs.
// - React UI strings use fixed keys.

namespace ZoningToolkit
{
    using Colossal;
    using System.Collections.Generic;

    public sealed class LocaleDE : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleDE(Setting setting)
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
                { m_Setting.GetOptionTabLocaleID(Setting.kActionsTab), "Aktionen" },
                { m_Setting.GetOptionTabLocaleID(Setting.kAboutTab),   "Über" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.kActionsGrp),        "Aktionen" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kCompatibilityGrp), "Kompatibilität" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kBindingsGrp),       "Tastenkürzel" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutGrp),          "Über" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutLinksGrp),     "Links" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kDebugGrp),          "Nur Debug" },

                // About fields
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ModName)), "Mod-Name" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ModName)),  "Anzeigename dieses Mods." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.VersionText)), "Version" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.VersionText)),  "Aktuelle Zone-Tools-Version." },

                // About links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadox)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadox)),  "Öffnet die Paradox-Mods-Seite des Autors." },

                // Actions toggles
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectOccupiedCells)), "Belegte Zellen schützen (mit Gebäuden)" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectOccupiedCells)),
                    "**[ ✓ ] aktiviert**, Zone Tools ändert die Zonierungstiefe/-fläche nicht bei Zellen, auf denen bereits ein Gebäude steht.\n" +
                    "**[   ] deaktiviert**, Gebäude könnten abgerissen werden, wenn die Zonierung darunter geändert wird."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectZonedCells)), "Zonierte, aber leere Zellen schützen" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectZonedCells)),
                    "**[ ✓ ] aktiviert**, Zone Tools ändert die Zonierungstiefe/-fläche nicht bei Zellen, die bereits zoniert sind (auch wenn sie leer sind).\n" +
                    "**[   ] deaktiviert**, bereits zonierte Zellen (gemalt RCIO) könnten beim Verwenden von Zone Tools überschrieben werden."
                },

                // Compatibility (Phase 1: manual user control only)
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ShowContourButton)), "Höhenlinien-Schaltfläche" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ShowContourButton)),
                    "**[ ✓ ] aktiviert**, zeigt die Gelände-/Höhenlinien-Schaltfläche im Zone-Tools-Panel.\n" +
                    "Bietet eine Option dort, wo das Spiel es in Tools erlaubt.\n" +
                    "Deaktivieren, wenn ein anderer Mod für Höhenlinien bevorzugt wird.\n" +
                    "Hinweis: selbst wenn dieses Höhenlinien-Tool nicht deaktiviert wird, ist das wahrscheinlich weiterhin ok.\n" +
                    "Der andere Mod übernimmt einfach und ist die funktionierende Höhenlinien-Schaltfläche (unsere Schaltfläche wird harmlos)."
                },

                // Keybinding option (Options → Mods)
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.TogglePanelBinding)), "Panel umschalten" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.TogglePanelBinding)),
                    "**Tastatur**-Kürzel zum Ein- oder Ausblenden des Zone-Tools-Panels (wie das Klicken auf das Menü-Icon oben links)."
                },

                // Keybinding action name (Options → Keybindings)
                { m_Setting.GetBindingKeyLocaleID(Mod.kTogglePanelActionName), "Zone Tools – Panel umschalten" },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpDebugReport)), "Ausführlicher Debug-Bericht ins Log" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpDebugReport)),
                    "Schreibt einen langen Debug-Bericht (einmalig) nach Logs/ZoneTools.log (nur für Debug).\n" +
                    "**Nicht nötig fürs normale Spielen**; erstellt ein riesiges Log, das gelöscht werden kann."
                },

                // -----------------------------------------------------------------
                // React UI strings
                // -----------------------------------------------------------------
                { "ZoneTools.UI.UpdateRoad", "Straße aktualisieren" },
                { "ZoneTools.UI.Tooltip.UpdateRoad",
                    "Zone-Tools-Panel ON / OFF (verschiebbar)"
                },

                { "ZoneTools.UI.Contour", "Höhenlinien" },
                { "ZoneTools.UI.Tooltip.Contour", "Geländelinien umschalten." },

                { "ZoneTools.UI.Tooltip.ModeDefault", "Beide (Standard)" },
                { "ZoneTools.UI.Tooltip.ModeLeft",    "Nur links" },
                { "ZoneTools.UI.Tooltip.ModeRight",   "Nur rechts" },
                { "ZoneTools.UI.Tooltip.ModeNone",    "Keine" },

                // GameTopLeft button tooltip
                { "ZoneTools.UI.Fab.Title", "Zone Tools" },
                { "ZoneTools.UI.Fab.Desc",  "Zonierung entlang von Straßen ändern.\nTastenkürzel: Shift+X (in den Optionen festlegen)." },
            };

            return d;
        }

        public void Unload( )
        {
        }
    }
}
