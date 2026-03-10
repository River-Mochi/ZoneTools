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
                { m_Setting.GetOptionTabLocaleID(Setting.kAboutTab),   "Informazioni" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.kActionsGrp),        "Azioni" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kCompatibilityGrp), "Compatibilità" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kBindingsGrp),       "Scorciatoie da tastiera" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutGrp),          "Informazioni" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutLinksGrp),     "Link" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kDebugGrp),          "Solo debug" },

                // About fields
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ModName)), "Nome mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ModName)),  "Nome visualizzato di questa mod." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.VersionText)), "Versione" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.VersionText)),  "Versione corrente di Zone Tools." },

                // About links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadox)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadox)),  "Apri la pagina Paradox Mods dell’autore." },

                // Actions toggles
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectOccupiedCells)), "Proteggi celle occupate (con edifici)" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectOccupiedCells)),
                    "**[ ✓ ] attivato**, Zone Tools non cambia profondità/area di zonizzazione sulle celle che hanno già un edificio.\n" +
                    "**[   ] disattivato**, gli edifici potrebbero essere demoliti cambiando la zonizzazione sotto di essi."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectZonedCells)), "Proteggi celle già zonizzate ma vuote" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectZonedCells)),
                    "**[ ✓ ] attivato**, Zone Tools non cambia profondità/area di zonizzazione sulle celle già zonizzate (anche se vuote).\n" +
                    "**[   ] disattivato**, celle già zonizzate (dipinte RCIO) potrebbero essere sovrascritte usando Zone Tools."
                },

                // Compatibility (Phase 1: manual user control only)
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ShowContourButton)), "Pulsante contorni" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ShowContourButton)),
                    "**[ ✓ ] attivato**, mostra il pulsante delle linee del terreno nel pannello Zone Tools.\n" +
                    "Fornisce un’opzione dove il gioco lo consente negli strumenti. " +
                    "Disattiva se preferisci un’altra mod per le linee di contorno.\n" +
                    "Nota: anche se non disattivi questo strumento contorni, probabilmente va comunque bene.\n" +
                    "L’altra mod sarà semplicemente la boss e prenderà il controllo come pulsante contorni funzionante (il nostro pulsante diventa innocuo)."
                },

                // Keybinding option (Options → Mods)
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.TogglePanelBinding)), "Mostra/Nascondi pannello" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.TogglePanelBinding)),
                    "Scorciatoia **da tastiera** per mostrare o nascondere il pannello Zone Tools (come cliccare l’icona del menu in alto a sinistra)."
                },

                // Keybinding action name (Options → Keybindings)
                { m_Setting.GetBindingKeyLocaleID(Mod.kTogglePanelActionName), "Zone Tools – Mostra/Nascondi pannello" },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpDebugReport)), "Rapporto di debug dettagliato nel log" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpDebugReport)),
                    "Scrive un lungo rapporto di debug una sola volta in Logs/ZoneTools.log (solo per debug).\n" +
                    "**Non necessario per il gioco normale**; crea un log enorme che puoi eliminare."
                },

                // -----------------------------------------------------------------
                // React UI strings
                // -----------------------------------------------------------------
                { "ZoneTools.UI.UpdateRoad", "Aggiorna strada" },
                { "ZoneTools.UI.Tooltip.UpdateRoad",
                    "Pannello Zone Tools ON / OFF (spostabile)"
                },

                { "ZoneTools.UI.Contour", "Contorni" },
                { "ZoneTools.UI.Tooltip.Contour", "Attiva/disattiva le linee del terreno." },

                { "ZoneTools.UI.Tooltip.ModeDefault", "Entrambi (predefinito)" },
                { "ZoneTools.UI.Tooltip.ModeLeft",    "Solo sinistra" },
                { "ZoneTools.UI.Tooltip.ModeRight",   "Solo destra" },
                { "ZoneTools.UI.Tooltip.ModeNone",    "Nessuno" },

                // GameTopLeft button tooltip
                { "ZoneTools.UI.Fab.Title", "Zone Tools" },
                { "ZoneTools.UI.Fab.Desc",  "Modifica la zonizzazione lungo le strade.\nScorciatoia: Shift+X (impostata in Opzioni)." },
            };

            return d;
        }

        public void Unload( )
        {
        }
    }
}
