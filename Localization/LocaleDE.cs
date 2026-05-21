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
                { m_Setting.GetOptionTabLocaleID(Setting.kAboutTab),   "Info" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.kActionsGrp),        "Schutz" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kBindingsGrp),       "Tastenbelegung" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kCompatibilityGrp),  "Kompatibilität" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kUiGrp),             "UI" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kUsageGrp),          "NUTZUNG" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutGrp),          "Info" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutLinksGrp),     "Links" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kDebugGrp),          "DEBUG" },

                // About fields
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ModName)), "Mod-Name" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ModName)),  "Angezeigter Name dieses Mods." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.VersionText)), "Version" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.VersionText)),  "Aktuelle Zone Tools-Version." },

                // About links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadox)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadox)),  "Öffnet die Paradox Mods-Seite des Autors." },

                // Actions toggles
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectOccupiedCells)), "● Belegte Zellen schützen (mit Gebäuden)" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectOccupiedCells)),
                    "**[ ✓ ] aktiviert**, Zone Tools entfernt keine Zonenzellen auf der Straßenseite mit Gebäuden.\n" +
                    "**[   ] deaktiviert**, Gebäude könnten durch Änderungen darunter abbruchreif werden."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectZonedCells)), "● Bemalte Zonen-Zellen schützen" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectZonedCells)),
                    "**[ ✓ ] aktiviert**, Zone Tools ändert bemalte RCIO-Zellen nicht (leer oder belegt).\n" +
                    "**[   ] deaktiviert**, bemalte Zonenblöcke können entfernt werden."
                },

                // Compatibility
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ContourIconText)), "Höhenlinien" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ContourIconText)), "" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ShowContourButton)), "● Kontur-Schaltfläche" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ShowContourButton)),
                    "**[ ✓ ] aktiviert**, zeigt die Contour-Schaltfläche im Zone Tools-Panel.\n\n" +
                    "● Damit können Höhenlinien aktiviert werden, auch wenn kein vanilla Straßenwerkzeug offen ist.\n" +
                    "● **Update Road**: aktiviert, die vanilla Topography-Schaltfläche ist unten links sichtbar.\n" +
                    "[ ] deaktivieren für ein kleineres Panel oder wenn ein anderer Mod das Gelände übernimmt.\n" +
                    "Wenn deaktiviert, ist Contour nur verfügbar, wenn **Update Road** ON ist."
                },

                // UI
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.UseGlassPanel)), "◉ Glas-Panel-Stil" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.UseGlassPanel)),
                    "**[ ✓ ] aktiviert**, verwendet den klareren, transluzenten Panelstil.\n" +
                    "**[   ] deaktiviert**, verwendet das graue Panel im Vanilla-Stil (dunkler).\n" +
                    "Beide Stile vermeiden Unschärfe; das ist nur eine optische Einstellung."
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DefaultPanelLocation)), "Standardposition des Panels" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.DefaultPanelLocation)),
                    "Wähle, wo das Zone Tools-Panel erscheint, wenn es über das Symbol oben links oder Shift+X geöffnet wird.\n" +
                    "Das Panel kann danach weiterhin verschoben werden."
                },
                { m_Setting.GetEnumValueLocaleID(Setting.PanelLocation.ScreenTopLeft), "Bildschirm oben links" },
                { m_Setting.GetEnumValueLocaleID(Setting.PanelLocation.ScreenBottomLeft), "Bildschirm unten links" },
                { m_Setting.GetEnumValueLocaleID(Setting.PanelLocation.ScreenBottomRight), "Bildschirm unten rechts" },

                // Keybinding option (Options → Mods)
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.TogglePanelBinding)), "Panel umschalten" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.TogglePanelBinding)),
                    "**Tastenkürzel**, um das Zone Tools-Panel ein- oder auszublenden (wie das Symbol oben links)."
                },

                // Usage toggle + multiline block
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ShowUsage)), "Show Instructions" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ShowUsage)), "Show or hide the usage instructions below." },
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
                    "The Contour button shows terrain elevation lines from the Zone Tools panel." },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.UsageText)), "" },

                // Keybinding action name (Options → Keybindings)
                { m_Setting.GetBindingKeyLocaleID(Mod.kTogglePanelActionName), "Zone Tools – Panel umschalten" },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpDebugReport)), "Ausführlichen Debug-Bericht ins Log schreiben" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpDebugReport)),
                    "Schreibt einen längeren Debug-Bericht nach Logs/ZoneTools.log (nur für Debug).\n" +
                    "**Für normales Spielen nicht nötig**; erstellt ein riesiges Log (kann gelöscht werden)."
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenLog)), "Log öffnen" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenLog)),
                    "Öffnet **ZoneTools.log**, wenn sie vorhanden ist.\n" +
                    "Wenn die Log-Datei noch nicht existiert, wird stattdessen der **Logs**-Ordner geöffnet."
                },

                // -----------------------------------------------------------------
                // React UI strings
                // -----------------------------------------------------------------
                { "ZoneTools.UI.Tooltip.TitleBar", "Panel an der Titelleiste ziehen." },

                { "ZoneTools.UI.UpdateRoad", "Update Road" },
                { "ZoneTools.UI.Tooltip.UpdateRoad", "Bestehende Straßen bearbeiten ON / OFF" },

                { "ZoneTools.UI.Contour", "Contour" },
                { "ZoneTools.UI.Tooltip.Contour", "Höhenlinien anzeigen." },

                { "ZoneTools.UI.Tooltip.ModeDefault", "Beide Seiten" },
                { "ZoneTools.UI.Tooltip.ModeLeft",    "Nur links" },
                { "ZoneTools.UI.Tooltip.ModeRight",   "Nur rechts" },
                { "ZoneTools.UI.Tooltip.ModeNone",    "Keine" },

                // GameTopLeft button tooltip
                { "ZoneTools.UI.Fab.Title", "Zone Tools" },
                { "ZoneTools.UI.Fab.Desc",  "Zonen entlang von Straßen ändern.\nShortcut: Shift+X (in den Optionen einstellbar)\nPanel ist verschiebbar." },
            };

            return d;
        }

        public void Unload( )
        {
        }
    }
}
