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
                { m_Setting.GetOptionGroupLocaleID(Setting.kActionsGrp),        "Acciones" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kBindingsGrp),       "Atajos de teclado" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kCompatibilityGrp),  "Compatibilidad" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kUiGrp),             "UI" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutGrp),          "Acerca de" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutLinksGrp),     "Enlaces" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kDebugGrp),          "Debug only" },

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
                    "**[ ✓ ] activado**, Zone Tools no cambia la profundidad/zona de celdas que ya tienen un edificio.\n" +
                    "**[   ] desactivado**, los edificios podrían ser condenados al cambiar el zonificado debajo."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectZonedCells)), "● Proteger celdas zonificadas pero vacías" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectZonedCells)),
                    "**[ ✓ ] activado**, Zone Tools no cambia la profundidad/zona de celdas ya zonificadas (aunque estén vacías).\n" +
                    "**[   ] desactivado**, las celdas ya zonificadas (RCIO pintado) podrían sobrescribirse con Zone Tools."
                },

                // Compatibility
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ShowContourButton)), "◉ Botón de contorno" },
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

                // Keybinding option (Options → Mods)
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.TogglePanelBinding)), "Mostrar/Ocultar panel" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.TogglePanelBinding)),
                    "**Atajo de teclado** para mostrar u ocultar el panel de Zone Tools (igual que el icono arriba a la izquierda)."
                },

                // Keybinding action name (Options → Keybindings)
                { m_Setting.GetBindingKeyLocaleID(Mod.kTogglePanelActionName), "Zone Tools – Mostrar/Ocultar panel" },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpDebugReport)), "Informe detallado de debug en el log" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpDebugReport)),
                    "Escribe un informe de debug más largo en Logs/ZoneTools.log (solo para debug).\n" +
                    "**No hace falta para jugar normal**; crea un log enorme (se puede borrar)."
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
