// File: Localization/LocaleJA.cs
// Purpose: Japanese (ja-JP) localization entries for Zone Tools.
// Notes:
// - Settings UI strings generated via ModSetting helper IDs.
// - React UI strings use fixed keys.

namespace ZoningToolkit
{
    using Colossal;
    using System.Collections.Generic;

    public sealed class LocaleJA : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleJA(Setting setting)
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
                { m_Setting.GetOptionTabLocaleID(Setting.kActionsTab), "アクション" },
                { m_Setting.GetOptionTabLocaleID(Setting.kAboutTab),   "情報" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.kActionsGrp),        "アクション" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kBindingsGrp),       "キー設定" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kCompatibilityGrp),  "互換性" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kUiGrp),             "UI" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutGrp),          "情報" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutLinksGrp),     "リンク" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kDebugGrp),          "Debug only" },

                // About fields
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ModName)), "MOD名" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ModName)),  "このMODの表示名です。" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.VersionText)), "バージョン" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.VersionText)),  "現在の Zone Tools バージョンです。" },

                // About links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadox)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadox)),  "作者の Paradox Mods ページを開きます。" },

                // Actions toggles
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectOccupiedCells)), "● 使用中セルを保護（建物あり）" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectOccupiedCells)),
                    "**[ ✓ ] 有効** の場合、建物があるセルのゾーン深さ/範囲を Zone Tools は変更しません。\n" +
                    "**[   ] 無効** の場合、その下のゾーン変更で建物が廃墟化する可能性があります。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectZonedCells)), "● すでにゾーン済みの空セルを保護" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectZonedCells)),
                    "**[ ✓ ] 有効** の場合、すでにゾーン済みのセル（空でも）を Zone Tools は変更しません。\n" +
                    "**[   ] 無効** の場合、すでにゾーン済みのセル（塗った RCIO）が Zone Tools で上書きされる可能性があります。"
                },

                // Compatibility
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ShowContourButton)), "◉ Contour ボタン" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ShowContourButton)),
                    "**[ ✓ ] 有効** の場合、Zone Tools パネルに Contour ボタンを表示します。\n\n" +
                    "● vanilla の道路ツールが開いていなくても地形線を有効化できます。\n" +
                    "● **Update Road** が有効なとき、vanilla の Topography ボタンが左下に表示されます。\n" +
                    "[ ] 小さいパネルがいい場合や、別 MOD で地形線を扱う場合は無効化してください。\n" +
                    "無効時は **Update Road** が ON の間だけ Contour が使えます。"
                },

                // UI
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.UseGlassPanel)), "◉ ガラス風パネル" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.UseGlassPanel)),
                    "**[ ✓ ] 有効** の場合、よりクリアな半透明パネルを使います。\n" +
                    "**[   ] 無効** の場合、vanilla 風のグレーのパネル（やや暗め）を使います。\n" +
                    "どちらも blur は使いません。見た目の好みだけです。"
                },

                // Keybinding option (Options → Mods)
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.TogglePanelBinding)), "パネル切り替え" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.TogglePanelBinding)),
                    "**キーボード** ショートカットで Zone Tools パネルを表示/非表示します（左上アイコンと同じ）。"
                },

                // Keybinding action name (Options → Keybindings)
                { m_Setting.GetBindingKeyLocaleID(Mod.kTogglePanelActionName), "Zone Tools – パネル切り替え" },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpDebugReport)), "詳細デバッグレポートをログに出力" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpDebugReport)),
                    "Logs/ZoneTools.log に長めのデバッグレポートを 1 回だけ書き出します（デバッグ専用）。\n" +
                    "**通常プレイでは不要**。巨大なログを作ります（削除可）。"
                },

                // -----------------------------------------------------------------
                // React UI strings
                // -----------------------------------------------------------------
                { "ZoneTools.UI.Tooltip.TitleBar", "タイトルバーをつかんでパネルを移動します。" },

                { "ZoneTools.UI.UpdateRoad", "Update Road" },
                { "ZoneTools.UI.Tooltip.UpdateRoad", "既存の道路編集 ON / OFF" },

                { "ZoneTools.UI.Contour", "Contour" },
                { "ZoneTools.UI.Tooltip.Contour", "地形線を表示します。" },

                { "ZoneTools.UI.Tooltip.ModeDefault", "両側" },
                { "ZoneTools.UI.Tooltip.ModeLeft",    "左のみ" },
                { "ZoneTools.UI.Tooltip.ModeRight",   "右のみ" },
                { "ZoneTools.UI.Tooltip.ModeNone",    "なし" },

                // GameTopLeft button tooltip
                { "ZoneTools.UI.Fab.Title", "Zone Tools" },
                { "ZoneTools.UI.Fab.Desc",  "道路沿いのゾーンを変更します。\nショートカット: Shift+X（オプションで変更可）\nパネルは移動できます。" },
            };

            return d;
        }

        public void Unload( )
        {
        }
    }
}
