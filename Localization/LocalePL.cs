// File: Localization/LocalePL.cs
// Purpose: Polish (pl-PL) localization entries for Zone Tools.
// Notes:
// - Settings UI strings generated via ModSetting helper IDs.
// - React UI strings use fixed keys.

namespace ZoningToolkit
{
    using Colossal;
    using System.Collections.Generic;

    public sealed class LocalePL : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocalePL(Setting setting)
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
                { m_Setting.GetOptionTabLocaleID(Setting.kActionsTab), "Akcje" },
                { m_Setting.GetOptionTabLocaleID(Setting.kAboutTab),   "Informacje" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.kActionsGrp),        "Akcje" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kCompatibilityGrp), "Zgodność" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kBindingsGrp),       "Skróty klawiszowe" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutGrp),          "Informacje" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutLinksGrp),     "Linki" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kDebugGrp),          "Tylko debug" },

                // About fields
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ModName)), "Nazwa moda" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ModName)),  "Wyświetlana nazwa tego moda." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.VersionText)), "Wersja" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.VersionText)),  "Aktualna wersja Zone Tools." },

                // About links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadox)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadox)),  "Otwórz stronę autora na Paradox Mods." },

                // Actions toggles
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectOccupiedCells)), "Chroń zajęte komórki (są budynki)" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectOccupiedCells)),
                    "**[ ✓ ] włączone**, Zone Tools nie zmienia głębokości/obszaru strefowania na komórkach, na których już stoi budynek.\n" +
                    "**[   ] wyłączone**, budynki mogą zostać skazane do wyburzenia przy zmianie strefowania pod nimi."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectZonedCells)), "Chroń ostrefowane, ale puste komórki" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectZonedCells)),
                    "**[ ✓ ] włączone**, Zone Tools nie zmienia głębokości/obszaru strefowania na komórkach, które są już ostrefowane (nawet jeśli puste).\n" +
                    "**[   ] wyłączone**, już ostrefowane komórki (pomalowane RCIO) mogą zostać nadpisane podczas używania Zone Tools."
                },

                // Compatibility (Phase 1: manual user control only)
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ShowContourButton)), "Przycisk konturów" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ShowContourButton)),
                    "**[ ✓ ] włączone**, pokazuje przycisk linii terenu w panelu Zone Tools.\n" +
                    "Daje opcję tam, gdzie gra na to pozwala w narzędziach.\n" +
                    "Wyłącz, jeśli wolisz innego moda do linii konturowych.\n" +
                    "Uwaga: nawet jeśli nie wyłączysz tego narzędzia konturów, prawdopodobnie nadal jest OK.\n" +
                    "Inny mod po prostu będzie bossem, przejmie kontrolę i będzie działał jako przycisk konturów (nasz przycisk staje się nieszkodliwy)."
                },

                // Keybinding option (Options → Mods)
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.TogglePanelBinding)), "Przełącz panel" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.TogglePanelBinding)),
                    "Skrót **klawiaturowy** do pokazania lub ukrycia panelu Zone Tools (to samo co kliknięcie ikony menu w lewym górnym rogu)."
                },

                // Keybinding action name (Options → Keybindings)
                { m_Setting.GetBindingKeyLocaleID(Mod.kTogglePanelActionName), "Zone Tools – Przełącz panel" },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpDebugReport)), "Szczegółowy raport debug do logu" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpDebugReport)),
                    "Zapisz jednorazowy długi raport debug do Logs/ZoneTools.log (tylko do debugowania).\n" +
                    "**Niepotrzebne w normalnej rozgrywce**; tworzy ogromny log, który można usunąć."
                },

                // -----------------------------------------------------------------
                // React UI strings
                // -----------------------------------------------------------------
                { "ZoneTools.UI.UpdateRoad", "Aktualizuj drogę" },
                { "ZoneTools.UI.Tooltip.UpdateRoad",
                    "Panel Zone Tools WŁ. / WYŁ. (przesuwalny)"
                },

                { "ZoneTools.UI.Contour", "Kontury" },
                { "ZoneTools.UI.Tooltip.Contour", "Przełącz linie terenu." },

                { "ZoneTools.UI.Tooltip.ModeDefault", "Obie strony (domyślnie)" },
                { "ZoneTools.UI.Tooltip.ModeLeft",    "Tylko lewa" },
                { "ZoneTools.UI.Tooltip.ModeRight",   "Tylko prawa" },
                { "ZoneTools.UI.Tooltip.ModeNone",    "Brak" },

                // GameTopLeft button tooltip
                { "ZoneTools.UI.Fab.Title", "Zone Tools" },
                { "ZoneTools.UI.Fab.Desc",  "Modyfikuj strefowanie wzdłuż dróg.\nSkrót: Shift+X (ustaw w Opcjach)." },
            };

            return d;
        }

        public void Unload( )
        {
        }
    }
}
