// File: Localization/LocaleIT.cs
// Purpose: Italian (it-IT) localization entries for Zone Tools.
// Notes:
// - Settings UI strings generated via ModSetting helper IDs.
// - React UI strings use fixed keys.

namespace ZoningToolkit
{
    using Colossal;
    using System.Collections.Generic;

    public sealed class LocaleIT : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleIT(Setting setting)
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
                { m_Setting.GetOptionTabLocaleID(Setting.kActionsTab), "Azioni" },
                { m_Setting.GetOptionTabLocaleID(Setting.kAboutTab),   "Info" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.kActionsGrp),        "Protezione" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kBindingsGrp),       "Scorciatoie da tastiera" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kCompatibilityGrp),  "Compatibilità" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kUiGrp),             "UI" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kUsageGrp),          "USO" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutGrp),          "Info" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutLinksGrp),     "Link" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kDebugGrp),          "DEBUG" },

                // About fields
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ModName)), "Nome mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ModName)),  "Nome visualizzato di questa mod." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.VersionText)), "Versione" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.VersionText)),  "Versione attuale di Zone Tools." },

                // About links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadox)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadox)),  "Apri la pagina Paradox Mods dell'autore." },

                // Actions toggles
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectOccupiedCells)), "● Proteggi celle occupate (con edifici)" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectOccupiedCells)),
                    "**[ ✓ ] attivo**, Zone Tools non rimuove celle di zona sul lato strada con edifici.\n" +
                    "**[   ] disattivo**, gli edifici potrebbero essere condannati cambiando la zona sotto."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectZonedCells)), "● Proteggi celle zonizzate dipinte" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectZonedCells)),
                    "**[ ✓ ] attivo**, Zone Tools non cambia celle RCIO dipinte (vuote o occupate).\n" +
                    "**[   ] disattivo**, i blocchi di zona dipinti potrebbero essere rimossi."
                },

                // Compatibility
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ContourIconText)), "Linee di contorno" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ContourIconText)), "" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ShowContourButton)), "● Pulsante contour" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ShowContourButton)),
                    "**[ ✓ ] attivo**, mostra il pulsante Contour nel pannello Zone Tools.\n\n" +
                    "● Permette di attivare le linee del terreno anche senza uno strumento strada vanilla aperto.\n" +
                    "● **Update Road**: quando attivo, il pulsante vanilla Topography è visibile in basso a sinistra.\n" +
                    "[ ] disattivalo se preferisci un pannello più piccolo o se un'altra mod gestisce il terreno.\n" +
                    "Quando disattivato, Contour è disponibile solo mentre **Update Road** è ON."
                },

                // UI
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.UseGlassPanel)), "◉ Stile pannello vetro" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.UseGlassPanel)),
                    "**[ ✓ ] attivo**, usa il pannello traslucido più chiaro.\n" +
                    "**[   ] disattivo**, usa il pannello grigio stile vanilla (più scuro).\n" +
                    "Entrambi gli stili evitano il blur; è solo una preferenza visiva."
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DefaultPanelLocation)), "Posizione predefinita del pannello" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.DefaultPanelLocation)),
                    "Scegli dove appare il pannello Zone Tools quando viene aperto dall'icona in alto a sinistra o con Shift+X.\n" +
                    "Il pannello può ancora essere trascinato dopo l'apertura."
                },
                { m_Setting.GetEnumValueLocaleID(Setting.PanelLocation.ScreenTopLeft), "Schermo in alto a sinistra" },
                { m_Setting.GetEnumValueLocaleID(Setting.PanelLocation.ScreenBottomLeft), "Schermo in basso a sinistra" },
                { m_Setting.GetEnumValueLocaleID(Setting.PanelLocation.ScreenBottomRight), "Schermo in basso a destra" },

                // Keybinding option (Options → Mods)
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.TogglePanelBinding)), "Mostra/Nascondi pannello" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.TogglePanelBinding)),
                    "**Scorciatoia da tastiera** per mostrare o nascondere il pannello Zone Tools (come cliccare l'icona in alto a sinistra)."
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
                { m_Setting.GetBindingKeyLocaleID(Mod.kTogglePanelActionName), "Zone Tools – Mostra/Nascondi pannello" },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpDebugReport)), "Scrivi report debug dettagliato nel log" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpDebugReport)),
                    "Scrive un report debug più lungo in Logs/ZoneTools.log (solo debug).\n" +
                    "**Non serve per giocare normalmente**; crea un log enorme (si può cancellare)."
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenLog)), "Apri log" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenLog)),
                    "Apre **ZoneTools.log** se esiste.\n" +
                    "Se il file di log non esiste ancora, apre invece la cartella **Logs**."
                },

                // -----------------------------------------------------------------
                // React UI strings
                // -----------------------------------------------------------------
                { "ZoneTools.UI.Tooltip.TitleBar", "Trascina il pannello dalla barra del titolo." },

                { "ZoneTools.UI.UpdateRoad", "Update Road" },
                { "ZoneTools.UI.Tooltip.UpdateRoad", "Modifica strade esistenti ON / OFF" },

                { "ZoneTools.UI.Contour", "Contour" },
                { "ZoneTools.UI.Tooltip.Contour", "Mostra le linee del terreno." },

                { "ZoneTools.UI.Tooltip.ModeDefault", "Entrambi i lati" },
                { "ZoneTools.UI.Tooltip.ModeLeft",    "Solo sinistra" },
                { "ZoneTools.UI.Tooltip.ModeRight",   "Solo destra" },
                { "ZoneTools.UI.Tooltip.ModeNone",    "Nessuno" },

                // GameTopLeft button tooltip
                { "ZoneTools.UI.Fab.Title", "Zone Tools" },
                { "ZoneTools.UI.Fab.Desc",  "Modifica le zone lungo le strade.\nScorciatoia: Shift+X (impostabile nelle Opzioni)\nIl pannello si può spostare." },
            };

            return d;
        }

        public void Unload( )
        {
        }
    }
}
