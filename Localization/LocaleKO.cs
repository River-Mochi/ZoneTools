// File: Localization/LocaleKO.cs
// Purpose: Korean (ko-KR) localization entries for Zone Tools.
// Notes:
// - Settings UI strings generated via ModSetting helper IDs.
// - React UI strings use fixed keys.

namespace ZoningToolkit
{
    using Colossal;
    using System.Collections.Generic;

    public sealed class LocaleKO : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleKO(Setting setting)
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
                { m_Setting.GetOptionTabLocaleID(Setting.kActionsTab), "작업" },
                { m_Setting.GetOptionTabLocaleID(Setting.kAboutTab),   "정보" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.kActionsGrp),        "작업" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kBindingsGrp),       "키 설정" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kCompatibilityGrp),  "호환성" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kUiGrp),             "UI" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutGrp),          "정보" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutLinksGrp),     "링크" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kDebugGrp),          "Debug only" },

                // About fields
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ModName)), "모드 이름" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ModName)),  "이 모드의 표시 이름입니다." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.VersionText)), "버전" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.VersionText)),  "현재 Zone Tools 버전입니다." },

                // About links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadox)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadox)),  "제작자의 Paradox Mods 페이지를 엽니다." },

                // Actions toggles
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectOccupiedCells)), "● 점유된 셀 보호 (건물 있음)" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectOccupiedCells)),
                    "**[ ✓ ] 활성화** 시, 건물이 이미 있는 셀의 구역 깊이/범위를 Zone Tools가 바꾸지 않습니다.\n" +
                    "**[   ] 비활성화** 시, 아래 구역을 바꾸면 건물이 철거 판정될 수 있습니다."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectZonedCells)), "● 이미 구역 지정된 빈 셀 보호" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectZonedCells)),
                    "**[ ✓ ] 활성화** 시, 이미 구역 지정된 셀(비어 있어도)을 Zone Tools가 바꾸지 않습니다.\n" +
                    "**[   ] 비활성화** 시, 이미 구역 지정된 셀(칠해진 RCIO)이 Zone Tools로 덮어써질 수 있습니다."
                },

                // Compatibility
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ShowContourButton)), "◉ Contour 버튼" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ShowContourButton)),
                    "**[ ✓ ] 활성화** 시, Zone Tools 패널에 Contour 버튼을 표시합니다.\n\n" +
                    "● 바닐라 도로 도구가 열려 있지 않아도 지형선을 켤 수 있습니다.\n" +
                    "● **Update Road** 가 켜져 있으면, 바닐라 Topography 버튼이 왼쪽 아래에 표시됩니다.\n" +
                    "[ ] 더 작은 패널이 좋거나 다른 모드가 지형선을 처리한다면 비활성화하세요.\n" +
                    "비활성화 시 Contour는 **Update Road** 가 ON일 때만 사용할 수 있습니다."
                },

                // UI
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.UseGlassPanel)), "◉ 유리 패널 스타일" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.UseGlassPanel)),
                    "**[ ✓ ] 활성화** 시, 더 선명한 반투명 패널을 사용합니다.\n" +
                    "**[   ] 비활성화** 시, 바닐라 스타일의 회색 패널(더 어두움)을 사용합니다.\n" +
                    "두 스타일 모두 blur를 쓰지 않습니다. 단순한 시각적 선택입니다."
                },

                // Keybinding option (Options → Mods)
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.TogglePanelBinding)), "패널 토글" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.TogglePanelBinding)),
                    "**키보드** 단축키로 Zone Tools 패널을 표시/숨김합니다 (왼쪽 위 아이콘과 동일)."
                },

                // Keybinding action name (Options → Keybindings)
                { m_Setting.GetBindingKeyLocaleID(Mod.kTogglePanelActionName), "Zone Tools – 패널 토글" },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpDebugReport)), "상세 디버그 보고서를 로그에 기록" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpDebugReport)),
                    "Logs/ZoneTools.log 에 더 긴 디버그 보고서를 한 번 기록합니다 (디버그 전용).\n" +
                    "**일반 플레이에는 필요 없음**. 매우 큰 로그를 만듭니다 (삭제 가능)."
                },

                // -----------------------------------------------------------------
                // React UI strings
                // -----------------------------------------------------------------
                { "ZoneTools.UI.Tooltip.TitleBar", "제목 표시줄을 잡고 패널을 이동하세요." },

                { "ZoneTools.UI.UpdateRoad", "Update Road" },
                { "ZoneTools.UI.Tooltip.UpdateRoad", "기존 도로 편집 ON / OFF" },

                { "ZoneTools.UI.Contour", "Contour" },
                { "ZoneTools.UI.Tooltip.Contour", "지형선을 표시합니다." },

                { "ZoneTools.UI.Tooltip.ModeDefault", "양쪽" },
                { "ZoneTools.UI.Tooltip.ModeLeft",    "왼쪽만" },
                { "ZoneTools.UI.Tooltip.ModeRight",   "오른쪽만" },
                { "ZoneTools.UI.Tooltip.ModeNone",    "없음" },

                // GameTopLeft button tooltip
                { "ZoneTools.UI.Fab.Title", "Zone Tools" },
                { "ZoneTools.UI.Fab.Desc",  "도로를 따라 구역을 수정합니다.\n단축키: Shift+X (옵션에서 변경 가능)\n패널 이동 가능." },
            };

            return d;
        }

        public void Unload( )
        {
        }
    }
}
