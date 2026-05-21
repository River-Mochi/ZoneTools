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
                { m_Setting.GetOptionGroupLocaleID(Setting.kActionsGrp),        "Proteção" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kBindingsGrp),       "Atalhos de teclado" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kCompatibilityGrp),  "Compatibilidade" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kUiGrp),             "UI" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kUsageGrp),          "USO" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutGrp),          "Sobre" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutLinksGrp),     "Links" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kDebugGrp),          "DEBUG" },

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
                    "**[ ✓ ] ativado**, o Zone Tools não remove células de zona no lado da via com construções.\n" +
                    "**[   ] desativado**, construções podem ser condenadas ao mudar a zona por baixo."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectZonedCells)), "● Proteger células zoneadas pintadas" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectZonedCells)),
                    "**[ ✓ ] ativado**, o Zone Tools não altera células RCIO pintadas (vazias ou ocupadas).\n" +
                    "**[   ] desativado**, blocos de zona pintados podem ser removidos."
                },

                // Compatibility
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ContourIconText)), "Linhas de contorno" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ContourIconText)), "" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ShowContourButton)), "● Botão Contour" },
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
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DefaultPanelLocation)), "Local padrão do painel" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.DefaultPanelLocation)),
                    "Escolha onde o painel do Zone Tools aparece ao abrir pelo ícone no canto superior esquerdo ou Shift+X.\n" +
                    "O painel ainda pode ser arrastado depois de abrir."
                },
                { m_Setting.GetEnumValueLocaleID(Setting.PanelLocation.ScreenTopLeft), "Tela canto superior esquerdo" },
                { m_Setting.GetEnumValueLocaleID(Setting.PanelLocation.ScreenBottomLeft), "Tela canto inferior esquerdo" },
                { m_Setting.GetEnumValueLocaleID(Setting.PanelLocation.ScreenBottomRight), "Tela canto inferior direito" },

                // Keybinding option (Options → Mods)
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.TogglePanelBinding)), "Alternar painel" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.TogglePanelBinding)),
                    "**Atalho de teclado** para mostrar ou esconder o painel do Zone Tools (igual ao ícone no canto superior esquerdo)."
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
                { m_Setting.GetBindingKeyLocaleID(Mod.kTogglePanelActionName), "Zone Tools – Alternar painel" },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpDebugReport)), "Gravar relatório de debug detalhado no log" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpDebugReport)),
                    "Grava uma vez um relatório de debug mais longo em Logs/ZoneTools.log (só para debug).\n" +
                    "**Não é necessário para jogar normalmente**; cria um log enorme (pode apagar)."
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenLog)), "Abrir log" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenLog)),
                    "Abre **ZoneTools.log** se ele existir.\n" +
                    "Se o arquivo de log ainda não existir, abre a pasta **Logs** no lugar."
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
