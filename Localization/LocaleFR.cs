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
                { m_Setting.GetOptionGroupLocaleID(Setting.kActionsGrp),        "Protection" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kBindingsGrp),       "Raccourcis clavier" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kCompatibilityGrp),  "Compatibilité" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kUiGrp),             "UI" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kUsageGrp),          "UTILISATION" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutGrp),          "À propos" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutLinksGrp),     "Liens" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kDebugGrp),          "DEBUG" },

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
                    "**[ ✓ ] activé**, Zone Tools ne retire pas les cases de zonage du côté de route avec bâtiments.\n" +
                    "**[   ] désactivé**, des bâtiments peuvent être condamnés si le zonage dessous change."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectZonedCells)), "● Protéger les cases zonées peintes" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectZonedCells)),
                    "**[ ✓ ] activé**, Zone Tools ne change pas les cases RCIO peintes (vides ou occupées).\n" +
                    "**[   ] désactivé**, les blocs de zonage peints peuvent être retirés."
                },

                // Compatibility
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ContourIconText)), "Lignes de contour" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ContourIconText)), "" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ShowContourButton)), "● Bouton de contour" },
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
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DefaultPanelLocation)), "Emplacement par défaut du panneau" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.DefaultPanelLocation)),
                    "Choisissez où le panneau Zone Tools apparaît lorsqu'il est ouvert avec l'icône en haut à gauche ou Shift+X.\n" +
                    "Le panneau peut toujours être déplacé après son ouverture."
                },
                { m_Setting.GetEnumValueLocaleID(Setting.PanelLocation.ScreenTopLeft), "Écran en haut à gauche" },
                { m_Setting.GetEnumValueLocaleID(Setting.PanelLocation.ScreenBottomLeft), "Écran en bas à gauche" },
                { m_Setting.GetEnumValueLocaleID(Setting.PanelLocation.ScreenBottomRight), "Écran en bas à droite" },

                // Keybinding option (Options → Mods)
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.TogglePanelBinding)), "Afficher/Masquer le panneau" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.TogglePanelBinding)),
                    "**Raccourci clavier** pour afficher ou masquer le panneau Zone Tools (comme cliquer l'icône en haut à gauche)."
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
                { m_Setting.GetBindingKeyLocaleID(Mod.kTogglePanelActionName), "Zone Tools – Afficher/Masquer le panneau" },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpDebugReport)), "Rapport debug détaillé dans le log" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpDebugReport)),
                    "Écrit un rapport debug plus long dans Logs/ZoneTools.log (debug uniquement).\n" +
                    "**Pas nécessaire en jeu normal** ; crée un énorme log (supprimable)."
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenLog)), "Ouvrir le log" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenLog)),
                    "Ouvre **ZoneTools.log** s'il existe.\n" +
                    "Si le fichier de log n'existe pas encore, ouvre le dossier **Logs** à la place."
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
