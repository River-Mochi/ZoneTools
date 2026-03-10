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
                { m_Setting.GetOptionGroupLocaleID(Setting.kCompatibilityGrp), "Compatibilité" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kBindingsGrp),       "Raccourcis clavier" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutGrp),          "À propos" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutLinksGrp),     "Liens" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kDebugGrp),          "Débogage uniquement" },

                // About fields
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ModName)), "Nom du mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ModName)),  "Nom d’affichage de ce mod." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.VersionText)), "Version" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.VersionText)),  "Version actuelle de Zone Tools." },

                // About links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadox)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadox)),  "Ouvrir la page Paradox Mods de l’auteur." },

                // Actions toggles
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectOccupiedCells)), "Protéger les cases occupées (avec bâtiments)" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectOccupiedCells)),
                    "**[ ✓ ] activé**, Zone Tools ne modifie pas la profondeur / zone sur les cases qui ont déjà un bâtiment.\n" +
                    "**[   ] désactivé**, des bâtiments peuvent être condamnés si le zonage est modifié sous eux."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectZonedCells)), "Protéger les cases déjà zonées mais vides" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectZonedCells)),
                    "**[ ✓ ] activé**, Zone Tools ne modifie pas la profondeur / zone sur les cases déjà zonées (même si elles sont vides).\n" +
                    "**[   ] désactivé**, des cases déjà zonées (peintes RCIO) peuvent être écrasées lors de l’utilisation de Zone Tools."
                },

                // Compatibility (Phase 1: manual user control only)
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ShowContourButton)), "Bouton Contour" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ShowContourButton)),
                    "**[ ✓ ] activé**, affiche le bouton Contour dans le panneau Zone Tools.\n\n" +
                    "Utilise les propres règles d'outil du jeu là où les lignes de terrain sont autorisées.\n" +
                    "Décoche cette case si un autre mod gère déjà le contour/la topographie et/ou si le bouton ne doit pas être affiché.\n" +
                    "Le laisser activé est généralement quand même sans problème et inoffensif, même avec un autre mod de contour/topographie.\n" +
                    "L'autre mod devient simplement le boss de la topographie."
                },

                // Keybinding option (Options → Mods)
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.TogglePanelBinding)), "Basculer le panneau" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.TogglePanelBinding)),
                    "Raccourci **clavier** pour afficher ou masquer le panneau Zone Tools (comme cliquer l’icône du menu en haut à gauche)."
                },

                // Keybinding action name (Options → Keybindings)
                { m_Setting.GetBindingKeyLocaleID(Mod.kTogglePanelActionName), "Zone Tools – Basculer le panneau" },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpDebugReport)), "Rapport de débogage détaillé dans le log" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpDebugReport)),
                    "Écrire un long rapport de débogage (une seule fois) dans Logs/ZoneTools.log (usage debug uniquement).\n" +
                    "**Pas nécessaire pour jouer normalement** ; crée un énorme log qui peut être supprimé."
                },

                // -----------------------------------------------------------------
                // React UI strings
                // -----------------------------------------------------------------
                { "ZoneTools.UI.UpdateRoad", "Mettre à jour la route" },
                { "ZoneTools.UI.Tooltip.UpdateRoad",
                    "Panneau Zone Tools ON / OFF (déplaçable)"
                },

                { "ZoneTools.UI.Contour", "Contour" },
                { "ZoneTools.UI.Tooltip.Contour", "Basculer les lignes de terrain." },

                { "ZoneTools.UI.Tooltip.ModeDefault", "Les deux (par défaut)" },
                { "ZoneTools.UI.Tooltip.ModeLeft",    "Gauche seulement" },
                { "ZoneTools.UI.Tooltip.ModeRight",   "Droite seulement" },
                { "ZoneTools.UI.Tooltip.ModeNone",    "Aucun" },

                // GameTopLeft button tooltip
                { "ZoneTools.UI.Fab.Title", "Zone Tools" },
                { "ZoneTools.UI.Fab.Desc",  "Modifier le zonage le long des routes.\nRaccourci : Maj+X (défini dans Options)." },
            };

            return d;
        }

        public void Unload( )
        {
        }
    }
}
