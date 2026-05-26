// File: Localization/LocaleTR.cs
// Purpose: Turkish (tr-TR) localization entries for Zone Tools.
// Notes:
// - Settings UI strings generated via ModSetting helper IDs.
// - React UI strings use fixed keys.

namespace ZoningToolkit
{
    using Colossal;
    using System.Collections.Generic;

    public sealed class LocaleTR : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleTR(Setting setting)
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
                { m_Setting.GetOptionTabLocaleID(Setting.kActionsTab), "Eylemler" },
                { m_Setting.GetOptionTabLocaleID(Setting.kAboutTab),   "Hakkında" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.kActionsGrp),        "Koruma" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kBindingsGrp),       "Tuş atamaları" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kCompatibilityGrp),  "Uyumluluk" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kUiGrp),             "Görsel Seçenekler" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kUsageGrp),          "KULLANIM" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutGrp),          "Hakkında" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutLinksGrp),     "Bağlantılar" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kDebugGrp),          "DEBUG" },

                // About fields
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ModName)), "Mod adı" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ModName)),  "Bu modun görünen adı." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.VersionText)), "Sürüm" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.VersionText)),  "Geçerli Zone Tools sürümü." },

                // About links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadox)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadox)),  "Yazarın Paradox Mods sayfasını aç." },

                // Actions toggles
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectOccupiedCells)), "● Dolu hücreleri koru (bina var)" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectOccupiedCells)),
                    "**[ ✓ ] açık**, Zone Tools, yol kesiminin bina bulunan tarafındaki bölgeleme hücrelerini kaldırmaz.\n" +
                    "**[   ] kapalı**, altındaki bölgeleme değiştirilirse binalar yıkılacak duruma düşebilir."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectZonedCells)), "● Boyalı bölgeleme hücrelerini koru" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectZonedCells)),
                    "**[ ✓ ] açık**, Zone Tools boyalı RCIO hücrelerinde (boş veya dolu) bölgeleme derinliğini/alanını değiştirmez.\n" +
                    "**[   ] kapalı**, boyalı bölge blokları kaldırılabilir."
                },

                // Compatibility
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ContourIconText)), "Eşyükselti Çizgileri" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ContourIconText)), "" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ShowContourButton)), "Butonu göster" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ShowContourButton)),
                    "● Yol aracı açık değilken bile arazi eşyükselti çizgileri görüntülenebilir.\n" +
                    "Uyumluluk seçeneği\n" +
                    "**[ ] kapat**, arazi çizgilerini göstermek için başka bir mod kullanılıyorsa (veya paneli küçültmek istiyorsan) bunu kapat.\n" +
                    "**[ ✓ ] açık**, Zone Tools panel kutusunda Eşyükselti butonunu gösterir.\n\n" +
                    "● Not: **Yolu Güncelle** simgesini etkinleştirdiğinde, Topografya butonu vanilla konumunda sol altta görünür.\n" +
                    "   - Bu [ ] seçenek kapalıyken bile, **Yolu Güncelle** AÇIK olduğu sürece eşyükselti kullanılabilir; çünkü bu, oyunun kendi vanilla arazi çizgileri görünüm aracını etkinleştirir."
                },

                // UI
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.UseGlassPanel)), "◉ Cam panel stili" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.UseGlassPanel)),
                    "**[ ✓ ] açık**, daha net yarı saydam panel stilini kullan.\n" +
                    "**[   ] kapalı**, vanilla tarzı gri paneli kullan (daha koyu).\n" +
                    "Bu yalnızca görsel bir tercih seçeneğidir."
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DefaultPanelLocation)), "Panel varsayılan konumu" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DefaultPanelLocation)),
                    "Zone Tools panelinin **varsayılan konumunu** seç (sol üst butondan veya Shift+X kısayolundan açıldığında).\n" +
                    "● Panel açıldıktan sonra başlık çubuğundan tutularak sürüklenebilir."
                },
                { m_Setting.GetEnumValueLocaleID(Setting.PanelLocation.ScreenTopLeft), "Sol Üst" },
                { m_Setting.GetEnumValueLocaleID(Setting.PanelLocation.ScreenBottomLeft), "Sol Alt" },
                { m_Setting.GetEnumValueLocaleID(Setting.PanelLocation.ScreenBottomRight), "Sağ Alt" },
                
                // Keybinding option (Options → Mods)
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.TogglePanelBinding)), "Paneli aç/kapat" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.TogglePanelBinding)),
                    "Zone Tools panelini göstermek veya gizlemek için **klavye** kısayolu\n" +
                    "Bu, ZT panelini açmak için sol üst menü simgesine tıklamakla aynıdır.\n"+
                    "Tercih ettiğin şekilde yeniden ayarla."
                },

                // Usage toggle + multiline block
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ShowUsage)), "Talimatları Göster" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ShowUsage)),
                    "Aşağıdaki **kullanım talimatlarını** göster veya gizle." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.UsageText)),
                    "<Paneli aç>\n" +
                    "1. Paneli açmak veya kapatmak için şehrin sol üstündeki Zone Tools butonuna tıkla ya da <Shift+X> tuşlarına bas.\n" +
                    "2. Paneli başka bir yere koymak istersen başlık çubuğundan sürükle.\n\n" +
                    "<Mevcut yollar>\n" +
                    "1. Paneldeki <Yolu Güncelle> simgesini AÇIK duruma getir.\n" +
                    "2. Seç: İki Taraf, Sol, Sağ veya Yok.\n" +
                    "3. Hangi bölgeleme hücrelerinin değişeceğini önizlemek için imleci bir yolun üzerine getir.\n" +
                    "4. <LMB> değişikliği uygular. Yol kesimleri boyunca <LMB> basılı tutup sürükle, ardından uygulamak için bırak.\n" +
                    "5. Aracı kullanırken <RMB> modlar arasında hızlıca geçiş yapar.\n\n" +
                    "<Koruma>\n" +
                    "Koruma seçenekleri, binaların altındaki veya zaten boyalı bölge hücrelerindeki bölgelemeyi kaldırmaktan kaçınmaya yardımcı olur.\n\n" +
                    "<Eşyükselti Çizgileri>\n" +
                    "Eşyükselti butonu, aynı Zone Tools panelinden arazi yükseklik çizgilerini gösterir."
                },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.UsageText)), "" },

                // Keybinding action name (Options → Keybindings)
                { m_Setting.GetBindingKeyLocaleID(Mod.kTogglePanelActionName), "Zone Tools – Paneli aç/kapat" },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpDebugReport)), "Günlüğe tek seferlik anlık rapor yaz" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpDebugReport)),
                    "Tek seferlik uzun bir debug raporunu şuraya yazar: \n" +
                    "<Logs/ZoneTools.log> (yalnızca debug kullanımı).\n" +
                    "**Normal oyun için gerekli değildir**;\n" +
                    "büyük bir günlük oluşturur (sonradan silebilirsin)."
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenLog)), "Günlüğü Aç" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenLog)),
                    "Varsa <Logs/ZoneTools.log> dosyasını açar.\n" +
                    "Günlük dosyası yoksa bunun yerine **Logs/** klasörünü açar."
                },

                // -----------------------------------------------------------------
                // React UI strings
                // -----------------------------------------------------------------
                { "ZoneTools.UI.Tooltip.TitleBar", "Paneli başlık çubuğundan sürükle." },

                { "ZoneTools.UI.UpdateRoad", "Yolu Güncelle" },
                { "ZoneTools.UI.Tooltip.UpdateRoad", "Mevcut yolları düzenle AÇIK / KAPALI" },

                { "ZoneTools.UI.Contour", "Eşyükselti" },
                { "ZoneTools.UI.Tooltip.Contour", "Arazi çizgilerini göster." },

                { "ZoneTools.UI.Tooltip.ModeDefault", "İki taraf" },
                { "ZoneTools.UI.Tooltip.ModeLeft",    "Sadece sol" },
                { "ZoneTools.UI.Tooltip.ModeRight",   "Sadece sağ" },
                { "ZoneTools.UI.Tooltip.ModeNone",    "Yok" },

                // GameTopLeft button tooltip
                { "ZoneTools.UI.Fab.Title", "Zone Tools" },
                { "ZoneTools.UI.Fab.Desc",  "Yollar boyunca bölgeleri değiştir.\nKısayol: Shift+X (Seçenekler'de ayarlanır)\nPanel taşınabilir." },
            };

            return d;
        }

        public void Unload( )
        {
        }
    }
}
