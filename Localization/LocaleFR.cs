// File: Localization/LocaleFR.cs
// Purpose: French (fr-FR) localization entries for Zone Tools.
// Notes:
// - Settings UI strings generated via ModSetting helper IDs.
// - React UI strings use fixed keys.

namespace ZoningToolkit
{
    using Colossal;
    using System.Collections.Generic;

    public sealed class LocaleFR : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleFR(Setting setting)
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
                { m_Setting.GetOptionTabLocaleID(Setting.kActionsTab), "Actions" },
                { m_Setting.GetOptionTabLocaleID(Setting.kAboutTab),   "À propos" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.kActionsGrp),        "Actions" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kBindingsGrp),       "Raccourcis clavier" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kCompatibilityGrp),  "Compatibilité" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kUiGrp),             "UI" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutGrp),          "À propos" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutLinksGrp),     "Liens" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kDebugGrp),          "Debug only" },

                // About fields
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ModName)), "Nom du mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ModName)),  "Nom affiché de ce mod." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.VersionText)), "Version" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.VersionText)),  "Version actuelle de Zone Tools." },

                // About links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadox)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadox)),  "Ouvrir la page Paradox Mods de l'auteur." },

                // Actions toggles
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectOccupiedCells)), "● Protéger les cases occupées (avec bâtiments)" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectOccupiedCells)),
                    "**[ ✓ ] activé**, Zone Tools ne change pas la profondeur/la zone des cases qui ont déjà un bâtiment.\n" +
                    "**[   ] désactivé**, des bâtiments peuvent être condamnés en changeant le zonage dessous."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectZonedCells)), "● Protéger les cases déjà zonées mais vides" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectZonedCells)),
                    "**[ ✓ ] activé**, Zone Tools ne change pas la profondeur/la zone des cases déjà zonées (même vides).\n" +
                    "**[   ] désactivé**, les cases déjà zonées (RCIO peint) peuvent être écrasées avec Zone Tools."
                },

                // Compatibility
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ShowContourButton)), "◉ Bouton de contour" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ShowContourButton)),
                    "**[ ✓ ] activé**, affiche le bouton Contour dans le panneau Zone Tools.\n\n" +
                    "● Permet d'activer les lignes de relief même sans outil de route vanilla ouvert.\n" +
                    "● **Update Road** : activé, le bouton vanilla Topography est visible en bas à gauche.\n" +
                    "[ ] désactive ceci pour un panneau plus petit ou si un autre mod gère le relief.\n" +
                    "Quand désactivé, le contour est dispo seulement quand **Update Road** est ON."
                },

                // UI
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.UseGlassPanel)), "◉ Style de panneau verre" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.UseGlassPanel)),
                    "**[ ✓ ] activé**, utilise le panneau translucide plus clair.\n" +
                    "**[   ] désactivé**, utilise le panneau gris style vanilla (plus sombre).\n" +
                    "Les deux styles évitent le flou ; c'est juste un choix visuel."
                },

                // Keybinding option (Options → Mods)
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.TogglePanelBinding)), "Afficher/Masquer le panneau" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.TogglePanelBinding)),
                    "**Raccourci clavier** pour afficher ou masquer le panneau Zone Tools (comme cliquer l'icône en haut à gauche)."
                },

                // Keybinding action name (Options → Keybindings)
                { m_Setting.GetBindingKeyLocaleID(Mod.kTogglePanelActionName), "Zone Tools – Afficher/Masquer le panneau" },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpDebugReport)), "Rapport debug détaillé dans le log" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpDebugReport)),
                    "Écrit un rapport debug plus long dans Logs/ZoneTools.log (debug uniquement).\n" +
                    "**Pas nécessaire en jeu normal** ; crée un énorme log (supprimable)."
                },

                // -----------------------------------------------------------------
                // React UI strings
                // -----------------------------------------------------------------
                { "ZoneTools.UI.Tooltip.TitleBar", "Faites glisser le panneau depuis la barre de titre." },

                { "ZoneTools.UI.UpdateRoad", "Update Road" },
                { "ZoneTools.UI.Tooltip.UpdateRoad", "Modifier les routes existantes ON / OFF" },

                { "ZoneTools.UI.Contour", "Contour" },
                { "ZoneTools.UI.Tooltip.Contour", "Afficher les lignes de relief." },

                { "ZoneTools.UI.Tooltip.ModeDefault", "Deux côtés" },
                { "ZoneTools.UI.Tooltip.ModeLeft",    "Gauche uniquement" },
                { "ZoneTools.UI.Tooltip.ModeRight",   "Droite uniquement" },
                { "ZoneTools.UI.Tooltip.ModeNone",    "Aucun" },

                // GameTopLeft button tooltip
                { "ZoneTools.UI.Fab.Title", "Zone Tools" },
                { "ZoneTools.UI.Fab.Desc",  "Modifier le zonage le long des routes.\nRaccourci : Shift+X (réglable dans Options)\nLe panneau peut être déplacé." },
            };

            return d;
        }

        public void Unload( )
        {
        }
    }
}
