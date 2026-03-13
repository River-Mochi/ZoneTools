// File: Localization/LocaleZH_CN.cs
// Purpose: Simplified Chinese (zh-HANS) localization entries for Zone Tools.
// Notes:
// - Settings UI strings generated via ModSetting helper IDs.
// - React UI strings use fixed keys.

namespace ZoningToolkit
{
    using Colossal;
    using System.Collections.Generic;

    public sealed class LocaleZH_CN : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleZH_CN(Setting setting)
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
                { m_Setting.GetOptionTabLocaleID(Setting.kAboutTab),   "关于" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.kActionsGrp),        "操作" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kBindingsGrp),       "按键绑定" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kCompatibilityGrp),  "兼容性" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kUiGrp),             "界面" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutGrp),          "关于" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutLinksGrp),     "链接" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kDebugGrp),          "仅调试" },

                // About fields
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ModName)), "模组名称" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ModName)),  "此模组的显示名称。" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.VersionText)), "版本" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.VersionText)),  "当前 Zone Tools 版本。" },

                // About links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadox)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadox)),  "打开作者的 Paradox Mods 页面。" },

                // Actions toggles
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectOccupiedCells)), "● 保护已占用格子（有建筑）" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectOccupiedCells)),
                    "**[ ✓ ] 已启用**，Zone Tools 不会修改已有建筑格子的分区深度/范围。\n" +
                    "**[   ] 已关闭**，更改建筑下方分区时，建筑可能会被判定为需拆除。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectZonedCells)), "● 保护已分区但为空的格子" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectZonedCells)),
                    "**[ ✓ ] 已启用**，Zone Tools 不会修改已分区格子（即使是空的）的分区深度/范围。\n" +
                    "**[   ] 已关闭**，已分区格子（已涂上的 RCIO）可能会被 Zone Tools 覆盖。"
                },

                // Compatibility
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ShowContourButton)), "◉ 等高线按钮" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ShowContourButton)),
                    "**[ ✓ ] 已启用**，在 Zone Tools 面板中显示 Contour 按钮。\n\n" +
                    "● 即使没有打开原版道路工具，也能开启地形等高线。\n" +
                    "● **Update Road** 开启时，原版 **Topography** 按钮会显示在左下角原版位置。\n" +
                    "[ ] 如果想要更小的面板，或使用其他模组处理地形线，可关闭此项。\n" +
                    "关闭后，只有在 **Update Road** 为 ON 时才能使用 Contour。"
                },

                // UI
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.UseGlassPanel)), "◉ 玻璃面板样式" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.UseGlassPanel)),
                    "**[ ✓ ] 已启用**，使用更通透的半透明面板样式。\n" +
                    "**[   ] 已关闭**，使用原版风格的灰色面板（更深一些）。\n" +
                    "两种样式都不使用模糊；这只是视觉偏好选项。"
                },

                // Keybinding option (Options → Mods)
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.TogglePanelBinding)), "切换面板" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.TogglePanelBinding)),
                    "**键盘** 快捷键，用于显示或隐藏 Zone Tools 面板（与点击左上角图标相同）。"
                },

                // Keybinding action name (Options → Keybindings)
                { m_Setting.GetBindingKeyLocaleID(Mod.kTogglePanelActionName), "Zone Tools – 切换面板" },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpDebugReport)), "将详细调试报告写入日志" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpDebugReport)),
                    "向 Logs/ZoneTools.log 写入一次更长的调试报告（仅调试用）。\n" +
                    "**正常游玩不需要**；会生成很大的日志（可删除）。"
                },

                // -----------------------------------------------------------------
                // React UI strings
                // -----------------------------------------------------------------
                { "ZoneTools.UI.Tooltip.TitleBar", "可从标题栏拖动面板。" },

                { "ZoneTools.UI.UpdateRoad", "更新道路" },
                { "ZoneTools.UI.Tooltip.UpdateRoad", "编辑现有道路 ON / OFF" },

                { "ZoneTools.UI.Contour", "等高线" },
                { "ZoneTools.UI.Tooltip.Contour", "显示地形等高线。" },

                { "ZoneTools.UI.Tooltip.ModeDefault", "两侧" },
                { "ZoneTools.UI.Tooltip.ModeLeft",    "仅左侧" },
                { "ZoneTools.UI.Tooltip.ModeRight",   "仅右侧" },
                { "ZoneTools.UI.Tooltip.ModeNone",    "无" },

                // GameTopLeft button tooltip
                { "ZoneTools.UI.Fab.Title", "Zone Tools" },
                { "ZoneTools.UI.Fab.Desc",  "修改道路沿线分区。\n快捷键：Shift+X（可在选项中设置）\n面板可移动。" },
            };

            return d;
        }

        public void Unload( )
        {
        }
    }
}
