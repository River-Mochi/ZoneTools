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
                { m_Setting.GetOptionTabLocaleID(Setting.kActionsTab), "操作" },
                { m_Setting.GetOptionTabLocaleID(Setting.kAboutTab),   "情報" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.kActionsGrp),        "操作" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kCompatibilityGrp), "互換性" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kBindingsGrp),       "キー割り当て" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutGrp),          "情報" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutLinksGrp),     "リンク" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kDebugGrp),          "デバッグ専用" },

                // About fields
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ModName)), "MOD名" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ModName)),  "このMODの表示名です。" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.VersionText)), "バージョン" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.VersionText)),  "現在の Zone Tools バージョンです。" },

                // About links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadox)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadox)),  "作者の Paradox Mods ページを開きます。" },

                // Actions toggles
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectOccupiedCells)), "使用中セルを保護（建物あり）" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectOccupiedCells)),
                    "**[ ✓ ] 有効**, Zone Tools は建物がすでにあるセルのゾーン深度/範囲を変更しません。\n" +
                    "**[   ] 無効**, 下のゾーンを変更すると建物が立ち退きになる可能性があります。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectZonedCells)), "ゾーン済みだが空のセルを保護" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectZonedCells)),
                    "**[ ✓ ] 有効**, Zone Tools は既にゾーン済み（空でも）のセルのゾーン深度/範囲を変更しません。\n" +
                    "**[   ] 無効**, Zone Tools 使用時に既にゾーン済みのセル（塗り RCIO）が上書きされる可能性があります。"
                },

                // Compatibility (Phase 1: manual user control only)
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ShowContourButton)), "等高線ボタン" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ShowContourButton)),
                    "**[ ✓ ] 有効**, Zone Tools パネルに地形ライン（等高線）ボタンを表示します。\n" +
                    "ゲーム側がツールで許可している場合に使えるオプションです。\n" +
                    "等高線用に別MODを使いたい場合は無効にしてください。\n" +
                    "注: この等高線ツールを無効にしなくても、おそらく問題ありません。\n" +
                    "別MODが主導して動作する等高線ボタンになり（こちらのボタンは無害になります）。"
                },

                // Keybinding option (Options → Mods)
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.TogglePanelBinding)), "パネル切り替え" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.TogglePanelBinding)),
                    "Zone Tools パネルを表示/非表示にする **キーボード** ショートカット（左上メニューアイコンのクリックと同じ）。"
                },

                // Keybinding action name (Options → Keybindings)
                { m_Setting.GetBindingKeyLocaleID(Mod.kTogglePanelActionName), "Zone Tools – パネル切り替え" },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpDebugReport)), "詳細デバッグレポートをログへ" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpDebugReport)),
                    "Logs/ZoneTools.log に長いデバッグレポートを1回だけ書き込みます（デバッグ専用）。\n" +
                    "**通常プレイでは不要**；巨大なログが作成され、削除できます。"
                },

                // -----------------------------------------------------------------
                // React UI strings
                // -----------------------------------------------------------------
                { "ZoneTools.UI.UpdateRoad", "道路を更新" },
                { "ZoneTools.UI.Tooltip.UpdateRoad",
                    "Zone Tools パネル ON / OFF（移動可能）"
                },

                { "ZoneTools.UI.Contour", "等高線" },
                { "ZoneTools.UI.Tooltip.Contour", "地形ラインの切り替え。" },

                { "ZoneTools.UI.Tooltip.ModeDefault", "両側（デフォルト）" },
                { "ZoneTools.UI.Tooltip.ModeLeft",    "左のみ" },
                { "ZoneTools.UI.Tooltip.ModeRight",   "右のみ" },
                { "ZoneTools.UI.Tooltip.ModeNone",    "なし" },

                // GameTopLeft button tooltip
                { "ZoneTools.UI.Fab.Title", "Zone Tools" },
                { "ZoneTools.UI.Fab.Desc",  "道路に沿ってゾーンを調整します。\nショートカット: Shift+X（オプションで設定）。" },
            };

            return d;
        }

        public void Unload( )
        {
        }
    }
}
