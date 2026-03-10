// File: Systems/ZoneToolSystem.ExistingRoads.Audio.cs
// Purpose: Vanilla tool audio feedback for Existing Roads tool.
// Notes:
// - Uses ToolUXSoundSettingsData singleton.
// - Provides select/build/cancel/snap sounds.

namespace ZoningToolkit.Systems
{
    using Game.Audio;        // AudioManager
    using Game.Prefabs;      // ToolUXSoundSettingsData
    using Unity.Collections; // Allocator
    using Unity.Entities;    // EntityQuery, EntityQueryBuilder

    internal sealed partial class ZoneToolSystemExistingRoads
    {
        private EntityQuery m_SoundbankQuery;

        private void InitializeAudio( )
        {
            m_SoundbankQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<ToolUXSoundSettingsData>()
                .Build(this);
        }

        private bool TryGetSoundbank(out ToolUXSoundSettingsData soundbank)
        {
            if (m_SoundbankQuery.IsEmptyIgnoreFilter)
            {
                soundbank = default;
                return false;
            }

            soundbank = m_SoundbankQuery.GetSingleton<ToolUXSoundSettingsData>();
            return true;
        }

        private void PlaySelectSound( )
        {
            if (!TryGetSoundbank(out ToolUXSoundSettingsData soundbank))
            {
                return;
            }

            AudioManager.instance.PlayUISound(soundbank.m_SelectEntitySound);
        }

        private void PlayBuildSound( )
        {
            if (!TryGetSoundbank(out ToolUXSoundSettingsData soundbank))
            {
                return;
            }

            AudioManager.instance.PlayUISound(soundbank.m_NetBuildSound);
        }

        private void PlayCancelSound( )
        {
            if (!TryGetSoundbank(out ToolUXSoundSettingsData soundbank))
            {
                return;
            }

            AudioManager.instance.PlayUISound(soundbank.m_NetCancelSound);
        }

        private void PlaySnapSound( )
        {
            if (!TryGetSoundbank(out ToolUXSoundSettingsData soundbank))
            {
                return;
            }

            AudioManager.instance.PlayUISound(soundbank.m_SnapSound);
        }
    }
}
