// File: Localization/LocaleZH_HANT.cs
// Purpose: Traditional Chinese (zh-HANT) localization entries for Zone Tools.
// Notes:
// - Settings UI strings generated via ModSetting helper IDs.
// - React UI strings use fixed keys.

namespace ZoningToolkit
{
    using Colossal;
    using System.Collections.Generic;

    public sealed class LocaleZH_HANT : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleZH_HANT(Setting setting)
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
                { m_Setting.GetOptionTabLocaleID(Setting.kActionsTab), "操作" },
                { m_Setting.GetOptionTabLocaleID(Setting.kAboutTab),   "關於" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.kActionsGrp),        "操作" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kCompatibilityGrp), "相容性" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kBindingsGrp),       "按鍵綁定" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutGrp),          "關於" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutLinksGrp),     "連結" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kDebugGrp),          "僅除錯" },

                // About fields
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ModName)), "模組名稱" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ModName)),  "此模組的顯示名稱。" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.VersionText)), "版本" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.VersionText)),  "目前的 Zone Tools 版本。" },

                // About links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadox)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadox)),  "開啟作者的 Paradox Mods 頁面。" },

                // Actions toggles
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectOccupiedCells)), "保護已佔用格（有建築）" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectOccupiedCells)),
                    "**[ ✓ ] 啟用**：Zone Tools 不會更改已有建築之格子的分區深度/面積。\n" +
                    "**[   ] 停用**：變更其下方分區時，建築可能會被判定為拆除。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectZonedCells)), "保護已分區但為空的格子" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectZonedCells)),
                    "**[ ✓ ] 啟用**：Zone Tools 不會更改已分區的格子的分區深度/面積（即使為空）。\n" +
                    "**[   ] 停用**：使用 Zone Tools 時，已分區的格子（塗抹的 RCIO）可能會被覆寫。"
                },

                // Compatibility (Phase 1: manual user control only)
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ShowContourButton)), "等高線按鈕" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ShowContourButton)),
                    "**[ ✓ ] 啟用**：在 Zone Tools 面板中顯示地形線按鈕。\n" +
                    "在遊戲允許的工具中提供該選項。" +
                    "若偏好其他等高線模組，請停用。\n" +
                    "注意：即使不停用此等高線工具，通常也沒問題。\n" +
                    "另一個模組會成為 boss 並接管為可用的等高線按鈕（我們的按鈕會變得無害）。"
                },

                // Keybinding option (Options → Mods)
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.TogglePanelBinding)), "切換面板" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.TogglePanelBinding)),
                    "用於顯示或隱藏 Zone Tools 面板的 **鍵盤** 快捷鍵（等同於點擊左上角選單圖示）。"
                },

                // Keybinding action name (Options → Keybindings)
                { m_Setting.GetBindingKeyLocaleID(Mod.kTogglePanelActionName), "Zone Tools – 切換面板" },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpDebugReport)), "輸出詳細除錯報告到日誌" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpDebugReport)),
                    "將一次性的長除錯報告寫入 Logs/ZoneTools.log（僅供除錯）。\n" +
                    "**一般遊玩不需要**；會產生很大的日誌檔，可自行刪除。"
                },

                // -----------------------------------------------------------------
                // React UI strings
                // -----------------------------------------------------------------
                { "ZoneTools.UI.UpdateRoad", "更新道路" },
                { "ZoneTools.UI.Tooltip.UpdateRoad",
                    "Zone Tools 面板 開 / 關（可移動）"
                },

                { "ZoneTools.UI.Contour", "等高線" },
                { "ZoneTools.UI.Tooltip.Contour", "切換地形線。" },

                { "ZoneTools.UI.Tooltip.ModeDefault", "兩側（預設）" },
                { "ZoneTools.UI.Tooltip.ModeLeft",    "僅左側" },
                { "ZoneTools.UI.Tooltip.ModeRight",   "僅右側" },
                { "ZoneTools.UI.Tooltip.ModeNone",    "無" },

                // GameTopLeft button tooltip
                { "ZoneTools.UI.Fab.Title", "Zone Tools" },
                { "ZoneTools.UI.Fab.Desc",  "沿道路修改分區。\n快捷鍵：Shift+X（在選項中設定）。" },
            };

            return d;
        }

        public void Unload( )
        {
        }
    }
}
