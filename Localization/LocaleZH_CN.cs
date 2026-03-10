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
                { m_Setting.GetOptionGroupLocaleID(Setting.kCompatibilityGrp), "兼容性" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kBindingsGrp),       "按键绑定" },
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
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectOccupiedCells)), "保护已占用格（有建筑）" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectOccupiedCells)),
                    "**[ ✓ ] 启用**：Zone Tools 不会更改已存在建筑的格子的分区深度/面积。\n" +
                    "**[   ] 禁用**：更改其下方分区时，建筑可能会被判定为拆除。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectZonedCells)), "保护已分区但为空的格子" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectZonedCells)),
                    "**[ ✓ ] 启用**：Zone Tools 不会更改已经分区的格子的分区深度/面积（即使为空）。\n" +
                    "**[   ] 禁用**：使用 Zone Tools 时，已分区的格子（涂抹的 RCIO）可能会被覆盖。"
                },

                // Compatibility (Phase 1: manual user control only)
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ShowContourButton)), "等高线按钮" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ShowContourButton)),
                    "**[ ✓ ] 已启用**，在 Zone Tools 面板中显示等高线按钮。\n\n" +
                    "在游戏工具允许显示地形线的地方，会使用游戏自身的工具规则。\n" +
                    "如果其他模组已经处理等高线/地形显示，和/或不想显示这个按钮，请取消勾选此项。\n" +
                    "即使装了其他等高线/地形显示模组，通常也可以继续勾选这个选项，基本无害。\n" +
                    "那时另一个模组就会成为地形显示的 boss。"
                },

                // Keybinding option (Options → Mods)
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.TogglePanelBinding)), "切换面板" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.TogglePanelBinding)),
                    "用于显示或隐藏 Zone Tools 面板的 **键盘** 快捷键（与点击左上角菜单图标相同）。"
                },

                // Keybinding action name (Options → Keybindings)
                { m_Setting.GetBindingKeyLocaleID(Mod.kTogglePanelActionName), "Zone Tools – 切换面板" },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpDebugReport)), "输出详细调试报告到日志" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpDebugReport)),
                    "将一次性的长调试报告写入 Logs/ZoneTools.log（仅用于调试）。\n" +
                    "**正常游戏不需要**；会生成一个很大的日志文件，可自行删除。"
                },

                // -----------------------------------------------------------------
                // React UI strings
                // -----------------------------------------------------------------
                { "ZoneTools.UI.UpdateRoad", "更新道路" },
                { "ZoneTools.UI.Tooltip.UpdateRoad",
                    "Zone Tools 面板 开 / 关（可移动）"
                },

                { "ZoneTools.UI.Contour", "等高线" },
                { "ZoneTools.UI.Tooltip.Contour", "切换地形线。" },

                { "ZoneTools.UI.Tooltip.ModeDefault", "两侧（默认）" },
                { "ZoneTools.UI.Tooltip.ModeLeft",    "仅左侧" },
                { "ZoneTools.UI.Tooltip.ModeRight",   "仅右侧" },
                { "ZoneTools.UI.Tooltip.ModeNone",    "无" },

                // GameTopLeft button tooltip
                { "ZoneTools.UI.Fab.Title", "Zone Tools" },
                { "ZoneTools.UI.Fab.Desc",  "沿道路修改分区。\n快捷键：Shift+X（在选项中设置）。" },
            };

            return d;
        }

        public void Unload( )
        {
        }
    }
}
