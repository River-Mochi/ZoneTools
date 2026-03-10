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
                { m_Setting.GetOptionGroupLocaleID(Setting.kCompatibilityGrp), "Compatibilidad" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kBindingsGrp),       "Atajos de teclado" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutGrp),          "Acerca de" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutLinksGrp),     "Enlaces" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kDebugGrp),          "Solo depuración" },

                // About fields
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ModName)), "Nombre del mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ModName)),  "Nombre visible de este mod." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.VersionText)), "Versión" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.VersionText)),  "Versión actual de Zone Tools." },

                // About links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadox)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadox)),  "Abrir la página de Paradox Mods del autor." },

                // Actions toggles
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectOccupiedCells)), "Proteger celdas ocupadas (con edificios)" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectOccupiedCells)),
                    "**[ ✓ ] activado**, Zone Tools no cambia la profundidad / área de zonificación en celdas que ya tienen un edificio.\n" +
                    "**[   ] desactivado**, los edificios podrían ser condenados al cambiar la zonificación debajo de ellos."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectZonedCells)), "Proteger celdas zonificadas pero vacías" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectZonedCells)),
                    "**[ ✓ ] activado**, Zone Tools no cambia la profundidad / área de zonificación en celdas que ya están zonificadas (aunque estén vacías).\n" +
                    "**[   ] desactivado**, celdas ya zonificadas (pintadas RCIO) podrían sobrescribirse al usar Zone Tools."
                },

                // Compatibility (Phase 1: manual user control only)
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ShowContourButton)), "Botón de contorno" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ShowContourButton)),
                    "**[ ✓ ] activado**, mostrar el botón de líneas de terreno en el panel de Zone Tools.\n" +
                    "Proporciona una opción donde el juego lo permite en herramientas. " +
                    "Desactívalo si prefieres otro mod para las líneas de contorno.\n" +
                    "Nota: incluso si no desactivas esta herramienta de contorno, probablemente siga estando bien.\n" +
                    "El otro mod simplemente mandará y será el botón de contorno que funcione (nuestro botón se vuelve inofensivo)."
                },

                // Keybinding option (Options → Mods)
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.TogglePanelBinding)), "Alternar panel" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.TogglePanelBinding)),
                    "Atajo de **teclado** para mostrar u ocultar el panel de Zone Tools (igual que hacer clic en el icono del menú arriba a la izquierda)."
                },

                // Keybinding action name (Options → Keybindings)
                { m_Setting.GetBindingKeyLocaleID(Mod.kTogglePanelActionName), "Zone Tools – Alternar panel" },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpDebugReport)), "Informe de depuración detallado al log" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpDebugReport)),
                    "Escribir un informe largo de depuración (una vez) en Logs/ZoneTools.log (solo para depuración).\n" +
                    "**No es necesario para jugar normalmente**; crea un log enorme que puedes borrar."
                },

                // -----------------------------------------------------------------
                // React UI strings
                // -----------------------------------------------------------------
                { "ZoneTools.UI.UpdateRoad", "Actualizar carretera" },
                { "ZoneTools.UI.Tooltip.UpdateRoad",
                    "Panel de Zone Tools ON / OFF (movible)"
                },

                { "ZoneTools.UI.Contour", "Contorno" },
                { "ZoneTools.UI.Tooltip.Contour", "Alternar líneas de terreno." },

                { "ZoneTools.UI.Tooltip.ModeDefault", "Ambos (predeterminado)" },
                { "ZoneTools.UI.Tooltip.ModeLeft",    "Solo izquierda" },
                { "ZoneTools.UI.Tooltip.ModeRight",   "Solo derecha" },
                { "ZoneTools.UI.Tooltip.ModeNone",    "Ninguno" },

                // GameTopLeft button tooltip
                { "ZoneTools.UI.Fab.Title", "Zone Tools" },
                { "ZoneTools.UI.Fab.Desc",  "Modificar la zonificación a lo largo de las carreteras.\nAtajo: Shift+X (se configura en Opciones)." },
            };

            return d;
        }

        public void Unload( )
        {
        }
    }
}
