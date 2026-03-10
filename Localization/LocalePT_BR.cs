// File: Localization/LocalePT_BR.cs
// Purpose: Portuguese (pt-BR) localization entries for Zone Tools.
// Notes:
// - Settings UI strings generated via ModSetting helper IDs.
// - React UI strings use fixed keys.

namespace ZoningToolkit
{
    using Colossal;
    using System.Collections.Generic;

    public sealed class LocalePT_BR : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocalePT_BR(Setting setting)
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
                { m_Setting.GetOptionTabLocaleID(Setting.kActionsTab), "Ações" },
                { m_Setting.GetOptionTabLocaleID(Setting.kAboutTab),   "Sobre" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.kActionsGrp),        "Ações" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kCompatibilityGrp), "Compatibilidade" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kBindingsGrp),       "Atalhos de teclado" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutGrp),          "Sobre" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutLinksGrp),     "Links" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kDebugGrp),          "Apenas debug" },

                // About fields
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ModName)), "Nome do mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ModName)),  "Nome exibido deste mod." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.VersionText)), "Versão" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.VersionText)),  "Versão atual do Zone Tools." },

                // About links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadox)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadox)),  "Abrir a página do autor no Paradox Mods." },

                // Actions toggles
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectOccupiedCells)), "Proteger células ocupadas (com edifícios)" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectOccupiedCells)),
                    "**[ ✓ ] ativado**, Zone Tools não altera a profundidade/área de zoneamento em células que já têm um edifício.\n" +
                    "**[   ] desativado**, edifícios podem ser condenados ao mudar o zoneamento sob eles."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectZonedCells)), "Proteger células zoneadas, mas vazias" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectZonedCells)),
                    "**[ ✓ ] ativado**, Zone Tools não altera a profundidade/área de zoneamento em células que já estão zoneadas (mesmo se vazias).\n" +
                    "**[   ] desativado**, células já zoneadas (pintadas RCIO) podem ser sobrescritas ao usar Zone Tools."
                },

                // Compatibility (Phase 1: manual user control only)
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ShowContourButton)), "Botão de contorno" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ShowContourButton)),
                    "**[ ✓ ] ativado**, mostrar o botão de linhas do terreno no painel do Zone Tools.\n" +
                    "Fornece uma opção onde o jogo permite isso nas ferramentas." +
                    "Desative se preferir outro mod para linhas de contorno.\n" +
                    "Observação: mesmo se você não desativar esta ferramenta de contorno, provavelmente ainda está tudo bem.\n" +
                    "O outro mod simplesmente será o boss, assumirá o controle e será o botão de contorno funcional (nosso botão fica inofensivo)."
                },

                // Keybinding option (Options → Mods)
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.TogglePanelBinding)), "Alternar painel" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.TogglePanelBinding)),
                    "Atalho de **teclado** para mostrar ou ocultar o painel do Zone Tools (o mesmo que clicar no ícone do menu no canto superior esquerdo)."
                },

                // Keybinding action name (Options → Keybindings)
                { m_Setting.GetBindingKeyLocaleID(Mod.kTogglePanelActionName), "Zone Tools – Alternar painel" },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpDebugReport)), "Relatório de debug detalhado no log" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpDebugReport)),
                    "Escreve um relatório longo de debug, uma única vez, em Logs/ZoneTools.log (somente para debug).\n" +
                    "**Não é necessário para o jogo normal**; cria um log enorme que pode ser apagado."
                },

                // -----------------------------------------------------------------
                // React UI strings
                // -----------------------------------------------------------------
                { "ZoneTools.UI.UpdateRoad", "Atualizar estrada" },
                { "ZoneTools.UI.Tooltip.UpdateRoad",
                    "Painel do Zone Tools LIG. / DESL. (movível)"
                },

                { "ZoneTools.UI.Contour", "Contorno" },
                { "ZoneTools.UI.Tooltip.Contour", "Alternar linhas do terreno." },

                { "ZoneTools.UI.Tooltip.ModeDefault", "Ambos (padrão)" },
                { "ZoneTools.UI.Tooltip.ModeLeft",    "Somente esquerda" },
                { "ZoneTools.UI.Tooltip.ModeRight",   "Somente direita" },
                { "ZoneTools.UI.Tooltip.ModeNone",    "Nenhum" },

                // GameTopLeft button tooltip
                { "ZoneTools.UI.Fab.Title", "Zone Tools" },
                { "ZoneTools.UI.Fab.Desc",  "Modificar o zoneamento ao longo das estradas.\nAtalho: Shift+X (definido em Opções)." },
            };

            return d;
        }

        public void Unload( )
        {
        }
    }
}
