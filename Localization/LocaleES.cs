// File: Localization/LocaleES.cs
// Purpose: Spanish (es-ES) localization entries for Zone Tools.
// Notes:
// - Settings UI strings generated via ModSetting helper IDs.
// - React UI strings use fixed keys.

namespace ZoningToolkit
{
    using Colossal;
    using System.Collections.Generic;

    public sealed class LocaleES : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleES(Setting setting)
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
                { m_Setting.GetOptionTabLocaleID(Setting.kActionsTab), "Acciones" },
                { m_Setting.GetOptionTabLocaleID(Setting.kAboutTab),   "Acerca de" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.kActionsGrp),        "Protección" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kBindingsGrp),       "Atajos de teclado" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kCompatibilityGrp),  "Compatibilidad" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kUiGrp),             "UI" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kUsageGrp),          "USO" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutGrp),          "Acerca de" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutLinksGrp),     "Enlaces" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kDebugGrp),          "DEBUG" },

                // About fields
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ModName)), "Nombre del mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ModName)),  "Nombre visible de este mod." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.VersionText)), "Versión" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.VersionText)),  "Versión actual de Zone Tools." },

                // About links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadox)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadox)),  "Abrir la página de Paradox Mods del autor." },

                // Actions toggles
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectOccupiedCells)), "● Proteger celdas ocupadas (con edificios)" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectOccupiedCells)),
                    "**[ ✓ ] activado**, Zone Tools no quita celdas de zona en el lado de carretera con edificios.\n" +
                    "**[   ] desactivado**, los edificios podrían condenarse al cambiar la zona debajo."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectZonedCells)), "● Proteger celdas zonificadas pintadas" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectZonedCells)),
                    "**[ ✓ ] activado**, Zone Tools no cambia celdas RCIO pintadas (vacías u ocupadas).\n" +
                    "**[   ] desactivado**, los bloques de zona pintados podrían eliminarse."
                },

                // Compatibility
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ContourIconText)), "Líneas de contorno" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ContourIconText)), "" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ShowContourButton)), "● Botón de contorno" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ShowContourButton)),
                    "**[ ✓ ] activado**, muestra el botón Contour en el panel de Zone Tools.\n\n" +
                    "● Permite activar líneas de terreno aunque no haya una herramienta de carretera vanilla abierta.\n" +
                    "● **Update Road**: activado, el botón vanilla Topography aparece abajo a la izquierda.\n" +
                    "[ ] desactiva esto si prefieres un panel más pequeño o si otro mod maneja el terreno.\n" +
                    "Si está desactivado, Contour solo está disponible cuando **Update Road** está ON."
                },

                // UI
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.UseGlassPanel)), "◉ Estilo de panel de cristal" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.UseGlassPanel)),
                    "**[ ✓ ] activado**, usa el panel translúcido más claro.\n" +
                    "**[   ] desactivado**, usa el panel gris estilo vanilla (más oscuro).\n" +
                    "Ambos estilos evitan el desenfoque; es solo una preferencia visual."
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DefaultPanelLocation)), "Ubicación predeterminada del panel" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.DefaultPanelLocation)),
                    "Elige dónde aparece el panel de Zone Tools al abrirlo con el icono superior izquierdo o Shift+X.\n" +
                    "El panel todavía se puede arrastrar después de abrirse."
                },
                { m_Setting.GetEnumValueLocaleID(Setting.PanelLocation.ScreenTopLeft), "Pantalla arriba a la izquierda" },
                { m_Setting.GetEnumValueLocaleID(Setting.PanelLocation.ScreenBottomLeft), "Pantalla abajo a la izquierda" },
                { m_Setting.GetEnumValueLocaleID(Setting.PanelLocation.ScreenBottomRight), "Pantalla abajo a la derecha" },

                // Keybinding option (Options → Mods)
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.TogglePanelBinding)), "Mostrar/Ocultar panel" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.TogglePanelBinding)),
                    "**Atajo de teclado** para mostrar u ocultar el panel de Zone Tools (igual que el icono arriba a la izquierda)."
                },

                // Usage toggle + multiline block
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ShowUsage)), "Mostrar instrucciones" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ShowUsage)),
                    "Muestra u oculta las **instrucciones de uso** de abajo." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.UsageText)),
                    "<Abrir panel>\n" +
                    "1. Haz clic en el botón Zone Tools en la parte superior izquierda de la ciudad, o pulsa <Shift+X>, para abrir o cerrar el panel.\n" +
                    "2. Arrastra el panel desde la barra de título si quieres ponerlo en otro lugar.\n\n" +
                    "<Carreteras existentes>\n" +
                    "1. Activa el icono <Update Road> en el panel.\n" +
                    "2. Elige: Ambos lados, Izquierda, Derecha o Ninguno.\n" +
                    "3. Pasa el cursor sobre una carretera para previsualizar qué celdas de zonificación cambiarán.\n" +
                    "4. <LMB> aplica el cambio. Mantén y arrastra <LMB> por secciones de carretera, luego suelta para aplicar.\n" +
                    "5. <RMB> cambia rápidamente entre modos mientras usas la herramienta.\n\n" +
                    "<Protección>\n" +
                    "Las opciones de protección ayudan a evitar quitar zonificación bajo edificios o celdas de zona ya pintadas.\n\n" +
                    "<Líneas de contorno>\n" +
                    "El botón Contour muestra las líneas de elevación del terreno desde el mismo panel de Zone Tools."
                },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.UsageText)), "" },              

                // Keybinding action name (Options → Keybindings)
                { m_Setting.GetBindingKeyLocaleID(Mod.kTogglePanelActionName), "Zone Tools – Mostrar/Ocultar panel" },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpDebugReport)), "Informe detallado de debug en el log" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpDebugReport)),
                    "Escribe un informe de debug más largo en Logs/ZoneTools.log (solo para debug).\n" +
                    "**No hace falta para jugar normal**; crea un log enorme (se puede borrar)."
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenLog)), "Abrir log" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenLog)),
                    "Abre **ZoneTools.log** si existe.\n" +
                    "Si el archivo de log todavía no está, abre la carpeta **Logs** en su lugar."
                },

                // -----------------------------------------------------------------
                // React UI strings
                // -----------------------------------------------------------------
                { "ZoneTools.UI.Tooltip.TitleBar", "Arrastra el panel desde la barra de título." },

                { "ZoneTools.UI.UpdateRoad", "Update Road" },
                { "ZoneTools.UI.Tooltip.UpdateRoad", "Editar carreteras existentes ON / OFF" },

                { "ZoneTools.UI.Contour", "Contour" },
                { "ZoneTools.UI.Tooltip.Contour", "Mostrar líneas del terreno." },

                { "ZoneTools.UI.Tooltip.ModeDefault", "Ambos lados" },
                { "ZoneTools.UI.Tooltip.ModeLeft",    "Solo izquierda" },
                { "ZoneTools.UI.Tooltip.ModeRight",   "Solo derecha" },
                { "ZoneTools.UI.Tooltip.ModeNone",    "Ninguno" },

                // GameTopLeft button tooltip
                { "ZoneTools.UI.Fab.Title", "Zone Tools" },
                { "ZoneTools.UI.Fab.Desc",  "Modifica zonas a lo largo de las carreteras.\nAtajo: Shift+X (se ajusta en Opciones)\nEl panel se puede mover." },
            };

            return d;
        }

        public void Unload( )
        {
        }
    }
}
