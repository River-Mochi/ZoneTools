// File: Systems/ZoneToolSystem.ExistingRoads.PrefabSafety.cs
// Purpose: Minimal, safe prefab handling for ExistingRoads tool UI integration.
// Notes:
// - No reflection scanning.
// - Uses active tool / NetTool prefab as a donor if available.
// - Tool activation is done via ToolSystem.activeTool, so prefab is only for UI safety.

namespace ZoningToolkit.Systems
{
    using Game.Prefabs;
    using Game.Tools;

    internal sealed partial class ZoneToolSystemExistingRoads
    {
        private PrefabBase? m_SafePrefabForUI;

#if DEBUG
        private bool m_LoggedMissingSafePrefab;
#endif

        private PrefabBase? GetSafePrefabForUI()
        {
            return m_SafePrefabForUI;
        }

        private void EnsureSafePrefabForUI(bool allowReflectionFallback)
        {
            // Intentionally ignore allowReflectionFallback in this minimal version.
            if (m_SafePrefabForUI != null)
            {
                return;
            }

            // 1) Best donor: whatever tool is currently active (as long as it isn't us).
            ToolSystem toolSystem = World.GetOrCreateSystemManaged<ToolSystem>();
            ToolBaseSystem? active = toolSystem.activeTool;
            if (active != null && active != this)
            {
                PrefabBase p = active.GetPrefab();
                if (p != null)
                {
                    m_SafePrefabForUI = p;
                    return;
                }
            }

            // 2) Next donor: NetTool prefab (often a RoadPrefab).
            NetToolSystem netTool = World.GetOrCreateSystemManaged<NetToolSystem>();
            PrefabBase np = netTool.GetPrefab();
            if (np != null)
            {
                m_SafePrefabForUI = np;
                return;
            }

#if DEBUG
            if (!m_LoggedMissingSafePrefab)
            {
                m_LoggedMissingSafePrefab = true;
                Mod.s_Log.Debug($"{Mod.ModTag} ExistingRoads: no safe prefab cached yet (usually fine).");
            }
#endif
        }

        private void OnGameLoadingCompletePrefabSafety()
        {
            // Intentionally empty in the minimal safe version.
        }
    }
}
