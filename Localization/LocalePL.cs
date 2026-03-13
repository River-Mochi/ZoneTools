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
                { m_Setting.GetOptionTabLocaleID(Setting.kAboutTab),   "O modzie" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.kActionsGrp),        "Akcje" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kBindingsGrp),       "Skróty klawiszowe" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kCompatibilityGrp),  "Kompatybilność" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kUiGrp),             "UI" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutGrp),          "O modzie" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutLinksGrp),     "Linki" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kDebugGrp),          "Debug only" },

                // About fields
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ModName)), "Nazwa moda" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ModName)),  "Wyświetlana nazwa tego moda." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.VersionText)), "Wersja" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.VersionText)),  "Aktualna wersja Zone Tools." },

                // About links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadox)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadox)),  "Otwórz stronę autora na Paradox Mods." },

                // Actions toggles
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectOccupiedCells)), "● Chroń zajęte komórki (z budynkami)" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectOccupiedCells)),
                    "**[ ✓ ] włączone**, Zone Tools nie zmienia głębokości/obszaru strefy w komórkach, które mają już budynek.\n" +
                    "**[   ] wyłączone**, budynki mogą zostać wyburzone po zmianie strefy pod nimi."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectZonedCells)), "● Chroń już wyznaczone, ale puste komórki" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectZonedCells)),
                    "**[ ✓ ] włączone**, Zone Tools nie zmienia głębokości/obszaru już wyznaczonych komórek (nawet pustych).\n" +
                    "**[   ] wyłączone**, już wyznaczone komórki (namalowane RCIO) mogą zostać nadpisane przez Zone Tools."
                },

                // Compatibility
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ShowContourButton)), "◉ Przycisk Contour" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ShowContourButton)),
                    "**[ ✓ ] włączone**, pokazuje przycisk Contour w panelu Zone Tools.\n\n" +
                    "● Umożliwia włączenie linii terenu nawet wtedy, gdy nie jest otwarte żadne vanilla narzędzie drogowe.\n" +
                    "● **Update Road**: po włączeniu vanilla przycisk Topography jest widoczny w lewym dolnym rogu.\n" +
                    "[ ] wyłącz to, jeśli wolisz mniejszy panel albo inny mod obsługuje linie terenu.\n" +
                    "Po wyłączeniu Contour jest dostępny tylko wtedy, gdy **Update Road** jest ON."
                },

                // UI
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.UseGlassPanel)), "◉ Szklany styl panelu" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.UseGlassPanel)),
                    "**[ ✓ ] włączone**, używa jaśniejszego półprzezroczystego panelu.\n" +
                    "**[   ] wyłączone**, używa szarego panelu w stylu vanilla (ciemniejszego).\n" +
                    "Oba style nie używają blur; to tylko wybór wyglądu."
                },

                // Keybinding option (Options → Mods)
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.TogglePanelBinding)), "Przełącz panel" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.TogglePanelBinding)),
                    "**Skrót klawiaturowy** do pokazywania lub ukrywania panelu Zone Tools (to samo co ikona w lewym górnym rogu)."
                },

                // Keybinding action name (Options → Keybindings)
                { m_Setting.GetBindingKeyLocaleID(Mod.kTogglePanelActionName), "Zone Tools – Przełącz panel" },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpDebugReport)), "Zapisz szczegółowy raport debug do logu" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpDebugReport)),
                    "Zapisuje jednorazowo dłuższy raport debug do Logs/ZoneTools.log (tylko do debugowania).\n" +
                    "**Niepotrzebne w normalnej grze**; tworzy ogromny log (można usunąć)."
                },

                // -----------------------------------------------------------------
                // React UI strings
                // -----------------------------------------------------------------
                { "ZoneTools.UI.Tooltip.TitleBar", "Przeciągnij panel za pasek tytułu." },

                { "ZoneTools.UI.UpdateRoad", "Update Road" },
                { "ZoneTools.UI.Tooltip.UpdateRoad", "Edycja istniejących dróg ON / OFF" },

                { "ZoneTools.UI.Contour", "Contour" },
                { "ZoneTools.UI.Tooltip.Contour", "Pokaż linie terenu." },

                { "ZoneTools.UI.Tooltip.ModeDefault", "Obie strony" },
                { "ZoneTools.UI.Tooltip.ModeLeft",    "Tylko lewa" },
                { "ZoneTools.UI.Tooltip.ModeRight",   "Tylko prawa" },
                { "ZoneTools.UI.Tooltip.ModeNone",    "Brak" },

                // GameTopLeft button tooltip
                { "ZoneTools.UI.Fab.Title", "Zone Tools" },
                { "ZoneTools.UI.Fab.Desc",  "Modyfikuje strefy wzdłuż dróg.\nSkrót: Shift+X (ustawiany w Opcjach)\nPanel można przesuwać." },
            };

            return d;
        }

        public void Unload( )
        {
        }
    }
}
