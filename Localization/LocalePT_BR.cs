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
                { m_Setting.GetOptionGroupLocaleID(Setting.kBindingsGrp),       "Atalhos de teclado" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kCompatibilityGrp),  "Compatibilidade" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kUiGrp),             "UI" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutGrp),          "Sobre" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutLinksGrp),     "Links" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kDebugGrp),          "Debug only" },

                // About fields
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ModName)), "Nome do mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ModName)),  "Nome exibido deste mod." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.VersionText)), "Versão" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.VersionText)),  "Versão atual do Zone Tools." },

                // About links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadox)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadox)),  "Abrir a página do autor no Paradox Mods." },

                // Actions toggles
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectOccupiedCells)), "● Proteger células ocupadas (com construções)" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectOccupiedCells)),
                    "**[ ✓ ] ativado**, o Zone Tools não altera a profundidade/área de células que já têm construção.\n" +
                    "**[   ] desativado**, construções podem ser condenadas ao mudar o zoneamento por baixo."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectZonedCells)), "● Proteger células já zoneadas, mas vazias" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectZonedCells)),
                    "**[ ✓ ] ativado**, o Zone Tools não altera a profundidade/área de células já zoneadas (mesmo vazias).\n" +
                    "**[   ] desativado**, células já zoneadas (RCIO pintado) podem ser sobrescritas pelo Zone Tools."
                },

                // Compatibility
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ShowContourButton)), "◉ Botão Contour" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ShowContourButton)),
                    "**[ ✓ ] ativado**, mostra o botão Contour no painel do Zone Tools.\n\n" +
                    "● Isso permite ativar linhas de terreno mesmo sem uma ferramenta de estrada vanilla aberta.\n" +
                    "● **Update Road**: quando ativado, o botão vanilla Topography fica visível no canto inferior esquerdo.\n" +
                    "[ ] desative isto se preferir um painel menor ou se outro mod cuidar das linhas de terreno.\n" +
                    "Quando desativado, Contour só fica disponível enquanto **Update Road** estiver ON."
                },

                // UI
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.UseGlassPanel)), "◉ Estilo de painel de vidro" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.UseGlassPanel)),
                    "**[ ✓ ] ativado**, usa o painel translúcido mais claro.\n" +
                    "**[   ] desativado**, usa o painel cinza estilo vanilla (mais escuro).\n" +
                    "Os dois estilos evitam blur; isso é só uma preferência visual."
                },

                // Keybinding option (Options → Mods)
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.TogglePanelBinding)), "Alternar painel" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.TogglePanelBinding)),
                    "**Atalho de teclado** para mostrar ou esconder o painel do Zone Tools (igual ao ícone no canto superior esquerdo)."
                },

                // Keybinding action name (Options → Keybindings)
                { m_Setting.GetBindingKeyLocaleID(Mod.kTogglePanelActionName), "Zone Tools – Alternar painel" },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpDebugReport)), "Gravar relatório de debug detalhado no log" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpDebugReport)),
                    "Grava uma vez um relatório de debug mais longo em Logs/ZoneTools.log (só para debug).\n" +
                    "**Não é necessário para jogar normalmente**; cria um log enorme (pode apagar)."
                },

                // -----------------------------------------------------------------
                // React UI strings
                // -----------------------------------------------------------------
                { "ZoneTools.UI.Tooltip.TitleBar", "Arraste o painel pela barra de título." },

                { "ZoneTools.UI.UpdateRoad", "Update Road" },
                { "ZoneTools.UI.Tooltip.UpdateRoad", "Editar estradas existentes ON / OFF" },

                { "ZoneTools.UI.Contour", "Contour" },
                { "ZoneTools.UI.Tooltip.Contour", "Mostrar linhas de terreno." },

                { "ZoneTools.UI.Tooltip.ModeDefault", "Ambos os lados" },
                { "ZoneTools.UI.Tooltip.ModeLeft",    "Só esquerda" },
                { "ZoneTools.UI.Tooltip.ModeRight",   "Só direita" },
                { "ZoneTools.UI.Tooltip.ModeNone",    "Nenhum" },

                // GameTopLeft button tooltip
                { "ZoneTools.UI.Fab.Title", "Zone Tools" },
                { "ZoneTools.UI.Fab.Desc",  "Modifica zonas ao longo das estradas.\nAtalho: Shift+X (configurável nas Opções)\nO painel pode se mover." },
            };

            return d;
        }

        public void Unload( )
        {
        }
    }
}
