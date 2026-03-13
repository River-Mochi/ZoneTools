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
                { m_Setting.GetOptionGroupLocaleID(Setting.kBindingsGrp),       "按鍵綁定" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kCompatibilityGrp),  "相容性" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kUiGrp),             "介面" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutGrp),          "關於" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutLinksGrp),     "連結" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kDebugGrp),          "僅限除錯" },

                // About fields
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ModName)), "模組名稱" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ModName)),  "此模組的顯示名稱。" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.VersionText)), "版本" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.VersionText)),  "目前的 Zone Tools 版本。" },

                // About links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadox)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadox)),  "開啟作者的 Paradox Mods 頁面。" },

                // Actions toggles
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectOccupiedCells)), "● 保護已占用格子（有建築）" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectOccupiedCells)),
                    "**[ ✓ ] 已啟用**，Zone Tools 不會修改已有建築格子的分區深度/範圍。\n" +
                    "**[   ] 已關閉**，更改建築下方分區時，建築可能會被判定為需拆除。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectZonedCells)), "● 保護已分區但為空的格子" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectZonedCells)),
                    "**[ ✓ ] 已啟用**，Zone Tools 不會修改已分區格子（即使是空的）的分區深度/範圍。\n" +
                    "**[   ] 已關閉**，已分區格子（已塗上的 RCIO）可能會被 Zone Tools 覆蓋。"
                },

                // Compatibility
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ShowContourButton)), "◉ 等高線按鈕" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ShowContourButton)),
                    "**[ ✓ ] 已啟用**，在 Zone Tools 面板中顯示 Contour 按鈕。\n\n" +
                    "● 即使沒有開啟原版道路工具，也能啟用地形等高線。\n" +
                    "● **Update Road** 開啟時，原版 **Topography** 按鈕會顯示在左下角原版位置。\n" +
                    "[ ] 如果想要更小的面板，或使用其他模組處理地形線，可關閉此項。\n" +
                    "關閉後，只有在 **Update Road** 為 ON 時才能使用 Contour。"
                },

                // UI
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.UseGlassPanel)), "◉ 玻璃面板樣式" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.UseGlassPanel)),
                    "**[ ✓ ] 已啟用**，使用更通透的半透明面板樣式。\n" +
                    "**[   ] 已關閉**，使用原版風格的灰色面板（更深一些）。\n" +
                    "兩種樣式都不使用模糊；這只是視覺偏好選項。"
                },

                // Keybinding option (Options → Mods)
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.TogglePanelBinding)), "切換面板" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.TogglePanelBinding)),
                    "**鍵盤** 快捷鍵，用於顯示或隱藏 Zone Tools 面板（與點擊左上角圖示相同）。"
                },

                // Keybinding action name (Options → Keybindings)
                { m_Setting.GetBindingKeyLocaleID(Mod.kTogglePanelActionName), "Zone Tools – 切換面板" },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpDebugReport)), "將詳細除錯報告寫入日誌" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpDebugReport)),
                    "向 Logs/ZoneTools.log 寫入一次較長的除錯報告（僅供除錯使用）。\n" +
                    "**正常遊玩不需要**；會產生很大的日誌（可刪除）。"
                },

                // -----------------------------------------------------------------
                // React UI strings
                // -----------------------------------------------------------------
                { "ZoneTools.UI.Tooltip.TitleBar", "可從標題列拖曳面板。" },

                { "ZoneTools.UI.UpdateRoad", "更新道路" },
                { "ZoneTools.UI.Tooltip.UpdateRoad", "編輯現有道路 ON / OFF" },

                { "ZoneTools.UI.Contour", "等高線" },
                { "ZoneTools.UI.Tooltip.Contour", "顯示地形等高線。" },

                { "ZoneTools.UI.Tooltip.ModeDefault", "兩側" },
                { "ZoneTools.UI.Tooltip.ModeLeft",    "僅左側" },
                { "ZoneTools.UI.Tooltip.ModeRight",   "僅右側" },
                { "ZoneTools.UI.Tooltip.ModeNone",    "無" },

                // GameTopLeft button tooltip
                { "ZoneTools.UI.Fab.Title", "Zone Tools" },
                { "ZoneTools.UI.Fab.Desc",  "修改道路沿線分區。\n快捷鍵：Shift+X（可在選項中設定）\n面板可移動。" },
            };

            return d;
        }

        public void Unload( )
        {
        }
    }
}
