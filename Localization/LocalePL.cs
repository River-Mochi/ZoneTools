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
                { m_Setting.GetOptionGroupLocaleID(Setting.kActionsGrp),        "Ochrona" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kBindingsGrp),       "Skróty klawiszowe" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kCompatibilityGrp),  "Kompatybilność" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kUiGrp),             "UI" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kUsageGrp),          "UŻYCIE" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutGrp),          "O modzie" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutLinksGrp),     "Linki" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kDebugGrp),          "DEBUG" },

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
                    "**[ ✓ ] włączone**, Zone Tools nie usuwa komórek strefy po stronie drogi z budynkami.\n" +
                    "**[   ] wyłączone**, budynki mogą zostać skazane po zmianie strefy pod nimi."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectZonedCells)), "● Chroń pomalowane komórki stref" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectZonedCells)),
                    "**[ ✓ ] włączone**, Zone Tools nie zmienia pomalowanych komórek RCIO (pustych lub zajętych).\n" +
                    "**[   ] wyłączone**, pomalowane bloki stref mogą zostać usunięte."
                },

                // Compatibility
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ContourIconText)), "Warstwice" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ContourIconText)), "" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ShowContourButton)), "● Przycisk Contour" },
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
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DefaultPanelLocation)), "Domyślne położenie panelu" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.DefaultPanelLocation)),
                    "Wybierz, gdzie pojawia się panel Zone Tools po otwarciu ikoną w lewym górnym rogu lub Shift+X.\n" +
                    "Panel nadal można przeciągać po otwarciu."
                },
                { m_Setting.GetEnumValueLocaleID(Setting.PanelLocation.ScreenTopLeft), "Ekran: lewy górny róg" },
                { m_Setting.GetEnumValueLocaleID(Setting.PanelLocation.ScreenBottomLeft), "Ekran: lewy dolny róg" },
                { m_Setting.GetEnumValueLocaleID(Setting.PanelLocation.ScreenBottomRight), "Ekran: prawy dolny róg" },

                // Keybinding option (Options → Mods)
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.TogglePanelBinding)), "Przełącz panel" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.TogglePanelBinding)),
                    "**Skrót klawiaturowy** do pokazywania lub ukrywania panelu Zone Tools (to samo co ikona w lewym górnym rogu)."
                },

                // Usage toggle + multiline block
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ShowUsage)), "Pokaż instrukcje" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ShowUsage)),
                    "Pokaż lub ukryj poniższe **instrukcje użycia**." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.UsageText)),
                    "<Otwórz panel>\n" +
                    "1. Kliknij przycisk Zone Tools w lewym górnym rogu miasta albo naciśnij <Shift+X>, aby otworzyć lub zamknąć panel.\n" +
                    "2. Przeciągnij panel za pasek tytułu, jeśli chcesz przenieść go w inne miejsce.\n\n" +
                    "<Istniejące drogi>\n" +
                    "1. Włącz ikonę <Update Road> w panelu.\n" +
                    "2. Wybierz: Obie strony, Lewa, Prawa albo Brak.\n" +
                    "3. Najedź na drogę, aby zobaczyć podgląd komórek stref, które zostaną zmienione.\n" +
                    "4. <LMB> stosuje zmianę. Przytrzymaj i przeciągnij <LMB> po odcinkach drogi, a potem puść, aby zastosować.\n" +
                    "5. <RMB> szybko przełącza tryby podczas używania narzędzia.\n\n" +
                    "<Ochrona>\n" +
                    "Opcje ochrony pomagają uniknąć usuwania stref pod budynkami lub już pomalowanymi komórkami stref.\n\n" +
                    "<Linie konturowe>\n" +
                    "Przycisk Contour pokazuje linie wysokości terenu z tego samego panelu Zone Tools."
                },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.UsageText)), "" },

                // Keybinding action name (Options → Keybindings)
                { m_Setting.GetBindingKeyLocaleID(Mod.kTogglePanelActionName), "Zone Tools – Przełącz panel" },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpDebugReport)), "Zapisz szczegółowy raport debug do logu" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpDebugReport)),
                    "Zapisuje jednorazowo dłuższy raport debug do Logs/ZoneTools.log (tylko do debugowania).\n" +
                    "**Niepotrzebne w normalnej grze**; tworzy ogromny log (można usunąć)."
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenLog)), "Otwórz log" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenLog)),
                    "Otwiera **ZoneTools.log**, jeśli istnieje.\n" +
                    "Jeśli plik logu jeszcze nie istnieje, zamiast tego otwiera folder **Logs**."
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
