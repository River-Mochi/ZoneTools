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
                { m_Setting.GetOptionGroupLocaleID(Setting.kActionsGrp),        "보호" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kBindingsGrp),       "키 설정" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kCompatibilityGrp),  "호환성" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kUiGrp),             "UI" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kUsageGrp),          "사용법" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutGrp),          "정보" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutLinksGrp),     "링크" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kDebugGrp),          "DEBUG" },

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
                    "**[ ✓ ] 활성화** 시, 건물이 있는 도로 쪽 구역 셀을 Zone Tools가 제거하지 않습니다.\n" +
                    "**[   ] 비활성화** 시, 아래 구역을 바꾸면 건물이 철거 판정될 수 있습니다."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectZonedCells)), "● 칠해진 구역 셀 보호" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectZonedCells)),
                    "**[ ✓ ] 활성화** 시, 칠해진 RCIO 셀(비어 있거나 점유됨)을 Zone Tools가 바꾸지 않습니다.\n" +
                    "**[   ] 비활성화** 시, 칠해진 구역 블록이 제거될 수 있습니다."
                },

                // Compatibility
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ContourIconText)), "등고선" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ContourIconText)), "" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ShowContourButton)), "● Contour 버튼" },
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
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DefaultPanelLocation)), "패널 기본 위치" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.DefaultPanelLocation)),
                    "왼쪽 상단 아이콘이나 Shift+X로 열 때 Zone Tools 패널이 나타날 위치를 선택합니다.\n" +
                    "패널은 열린 뒤에도 드래그할 수 있습니다."
                },
                { m_Setting.GetEnumValueLocaleID(Setting.PanelLocation.ScreenTopLeft), "화면 왼쪽 상단" },
                { m_Setting.GetEnumValueLocaleID(Setting.PanelLocation.ScreenBottomLeft), "화면 왼쪽 하단" },
                { m_Setting.GetEnumValueLocaleID(Setting.PanelLocation.ScreenBottomRight), "화면 오른쪽 하단" },

                // Keybinding option (Options → Mods)
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.TogglePanelBinding)), "패널 토글" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.TogglePanelBinding)),
                    "**키보드** 단축키로 Zone Tools 패널을 표시/숨김합니다 (왼쪽 위 아이콘과 동일)."
                },

                // Usage toggle + multiline block
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ShowUsage)), "설명 표시" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ShowUsage)),
                    "아래의 **사용 설명**을 표시하거나 숨깁니다." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.UsageText)),
                    "<패널 열기>\n" +
                    "1. 도시 화면 왼쪽 위의 Zone Tools 버튼을 클릭하거나 <Shift+X>를 눌러 패널을 열거나 닫습니다.\n" +
                    "2. 다른 위치에 두고 싶으면 제목 표시줄을 드래그해서 패널을 이동합니다.\n\n" +
                    "<기존 도로>\n" +
                    "1. 패널에서 <Update Road> 아이콘을 ON으로 켭니다.\n" +
                    "2. 선택: 양쪽, 왼쪽, 오른쪽, 없음.\n" +
                    "3. 도로에 마우스를 올리면 변경될 구역 셀을 미리 볼 수 있습니다.\n" +
                    "4. <LMB>로 변경을 적용합니다. 도로 구간 위에서 <LMB>를 누른 채 드래그한 뒤 놓으면 적용됩니다.\n" +
                    "5. 도구 사용 중 <RMB>로 모드를 빠르게 전환할 수 있습니다.\n\n" +
                    "<보호>\n" +
                    "보호 옵션은 건물 아래나 이미 칠해진 구역 셀의 구역이 제거되는 것을 방지하는 데 도움이 됩니다.\n\n" +
                    "<등고선>\n" +
                    "Contour 버튼은 같은 Zone Tools 패널에서 지형 고도선을 표시합니다."
                },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.UsageText)), "" },

                // Keybinding action name (Options → Keybindings)
                { m_Setting.GetBindingKeyLocaleID(Mod.kTogglePanelActionName), "Zone Tools – 패널 토글" },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpDebugReport)), "상세 디버그 보고서를 로그에 기록" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpDebugReport)),
                    "Logs/ZoneTools.log 에 더 긴 디버그 보고서를 한 번 기록합니다 (디버그 전용).\n" +
                    "**일반 플레이에는 필요 없음**. 매우 큰 로그를 만듭니다 (삭제 가능)."
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenLog)), "로그 열기" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenLog)),
                    "**ZoneTools.log** 가 있으면 엽니다.\n" +
                    "아직 로그 파일이 없으면 대신 **Logs** 폴더를 엽니다."
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
