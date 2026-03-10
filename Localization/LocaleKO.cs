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
                { m_Setting.GetOptionGroupLocaleID(Setting.kCompatibilityGrp), "호환성" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kBindingsGrp),       "키 바인딩" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutGrp),          "정보" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutLinksGrp),     "링크" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kDebugGrp),          "디버그 전용" },

                // About fields
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ModName)), "모드 이름" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ModName)),  "이 모드의 표시 이름입니다." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.VersionText)), "버전" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.VersionText)),  "현재 Zone Tools 버전입니다." },

                // About links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadox)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadox)),  "제작자의 Paradox Mods 페이지를 엽니다." },

                // Actions toggles
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectOccupiedCells)), "점유된 셀 보호(건물 있음)" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectOccupiedCells)),
                    "**[ ✓ ] 활성화**, Zone Tools는 이미 건물이 있는 셀의 구역 깊이/영역을 변경하지 않습니다.\n" +
                    "**[   ] 비활성화**, 아래 구역을 변경하면 건물이 철거될 수 있습니다."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectZonedCells)), "구역 지정됐지만 빈 셀 보호" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectZonedCells)),
                    "**[ ✓ ] 활성화**, Zone Tools는 이미 구역 지정된 셀(비어 있어도)의 구역 깊이/영역을 변경하지 않습니다.\n" +
                    "**[   ] 비활성화**, Zone Tools 사용 시 이미 구역 지정된 셀(RCIO로 칠해진 셀)이 덮어써질 수 있습니다."
                },

                // Compatibility (Phase 1: manual user control only)
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ShowContourButton)), "윤곽선 버튼" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ShowContourButton)),
                    "**[ ✓ ] 활성화**, Zone Tools 패널에 지형선 버튼을 표시합니다.\n" +
                    "게임이 도구에서 허용하는 경우에 사용할 수 있는 옵션입니다.\n" +
                    "윤곽선(지형선)용 다른 모드를 선호하면 비활성화하세요.\n" +
                    "참고: 이 윤곽선 도구를 비활성화하지 않아도, 아마도 괜찮습니다.\n" +
                    "다른 모드가 보스가 되어 작동하는 윤곽선 버튼이 되고(우리 버튼은 무해해집니다)."
                },

                // Keybinding option (Options → Mods)
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.TogglePanelBinding)), "패널 토글" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.TogglePanelBinding)),
                    "Zone Tools 패널을 표시/숨김하는 **키보드** 단축키(좌측 상단 메뉴 아이콘 클릭과 동일)."
                },

                // Keybinding action name (Options → Keybindings)
                { m_Setting.GetBindingKeyLocaleID(Mod.kTogglePanelActionName), "Zone Tools – 패널 토글" },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpDebugReport)), "상세 디버그 보고서 로그 기록" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpDebugReport)),
                    "Logs/ZoneTools.log에 긴 디버그 보고서를 1회 기록합니다(디버그 전용).\n" +
                    "**일반 플레이에는 필요 없음**; 매우 큰 로그가 생성되며 삭제할 수 있습니다."
                },

                // -----------------------------------------------------------------
                // React UI strings
                // -----------------------------------------------------------------
                { "ZoneTools.UI.UpdateRoad", "도로 업데이트" },
                { "ZoneTools.UI.Tooltip.UpdateRoad",
                    "Zone Tools 패널 ON / OFF(이동 가능)"
                },

                { "ZoneTools.UI.Contour", "윤곽선" },
                { "ZoneTools.UI.Tooltip.Contour", "지형선 토글." },

                { "ZoneTools.UI.Tooltip.ModeDefault", "양쪽(기본값)" },
                { "ZoneTools.UI.Tooltip.ModeLeft",    "왼쪽만" },
                { "ZoneTools.UI.Tooltip.ModeRight",   "오른쪽만" },
                { "ZoneTools.UI.Tooltip.ModeNone",    "없음" },

                // GameTopLeft button tooltip
                { "ZoneTools.UI.Fab.Title", "Zone Tools" },
                { "ZoneTools.UI.Fab.Desc",  "도로를 따라 구역을 수정합니다.\n단축키: Shift+X(옵션에서 설정)." },
            };

            return d;
        }

        public void Unload( )
        {
        }
    }
}
