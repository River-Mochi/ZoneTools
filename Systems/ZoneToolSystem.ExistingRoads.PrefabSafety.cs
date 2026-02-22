// File: Systems/ZoneToolSystem.ExistingRoads.PrefabSafety.cs
// Purpose: Provide a non-null prefab for ToolBaseSystem.GetPrefab() and optional PrefabID dump helpers.
// Notes:
// - Avoid automatic debug dumping during loading; logging can throw in some setups.
// - Debug dump helpers remain available for manual use in DEBUG builds.
// - Retry resolution a limited number of times until PrefabSystem is populated.

namespace ZoningToolkit.Systems
{
    using Game.Prefabs;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;

    internal sealed partial class ZoneToolSystemExistingRoads
    {
        private PrefabBase? m_SafePrefabForUI;
        private int m_SafePrefabResolveAttempts;

#if DEBUG
        // Default OFF. Enable only when actively debugging prefab IDs.
        private const bool kEnableAutoDebugDump = false;
        private bool m_DidDebugDump;
#endif

        private void EnsureSafePrefabForUI( )
        {
            if (m_SafePrefabForUI != null)
            {
                return;
            }

            // Avoid runaway work if something is truly broken.
            // Resolution attempts occur only when GetPrefab() is asked for.
            if (m_SafePrefabResolveAttempts >= 32)
            {
                return;
            }

            m_SafePrefabResolveAttempts++;

            // 1) Best-effort: borrow a prefab from NetToolSystem if it has one.
            // Avoid recursion: active tool can be this tool.
            TryAssignSafePrefabFromToolSystem();

            if (m_SafePrefabForUI != null)
            {
                return;
            }

            // 2) Prefer “tool-ish” candidates first. Best-effort attempts.
            TryAssignSafePrefab(new PrefabID("FencePrefab", "Quay"));
            TryAssignSafePrefab(new PrefabID("FencePrefab", "Quay01"));
            TryAssignSafePrefab(new PrefabID("FencePrefab", "RetainingWall"));
            TryAssignSafePrefab(new PrefabID("FencePrefab", "RetainingWall01"));

            if (m_SafePrefabForUI != null)
            {
                return;
            }

            // 3) Fallback: pick the first prefab ID discoverable via reflection.
            if (TryGetAnyPrefabIdKey(out PrefabID anyId))
            {
                TryAssignSafePrefab(anyId);
            }

#if DEBUG
            if (kEnableAutoDebugDump && !m_DidDebugDump && m_SafePrefabForUI != null)
            {
                m_DidDebugDump = true;

                DebugDumpPrefabIds("Crosswalk");
                DebugDumpPrefabIds("Wide Sidewalk");
                DebugDumpAnyPrefabDonors(max: 25);
            }
#endif
        }

        private PrefabBase GetSafePrefabForUI( )
        {
            EnsureSafePrefabForUI();

            // Last-chance retry once more (PrefabSystem might populate slightly later).
            if (m_SafePrefabForUI == null && TryGetAnyPrefabIdKey(out PrefabID anyId))
            {
                TryAssignSafePrefab(anyId);
            }

            // Returning null is technically allowed by runtime even if signature is non-nullable,
            // but it defeats the purpose. Keep retrying via EnsureSafePrefabForUI() call sites.
            return m_SafePrefabForUI!;
        }

        private void TryAssignSafePrefabFromToolSystem( )
        {
            if (m_SafePrefabForUI != null)
            {
                return;
            }

            // Prefer the NetToolSystem's prefab if available.
            // This is a cheap, no-reflection path.
            try
            {
                if (m_NetToolSystem != null)
                {
                    PrefabBase prefab = m_NetToolSystem.GetPrefab();
                    if (prefab != null)
                    {
                        m_SafePrefabForUI = prefab;
                    }
                }
            }
            catch
            {
                // No logging here.
            }
        }

        private void TryAssignSafePrefab(PrefabID id)
        {
            if (m_SafePrefabForUI != null)
            {
                return;
            }

            if (m_ZTPrefabSystem.TryGetPrefab(id, out PrefabBase prefab) && prefab != null)
            {
                m_SafePrefabForUI = prefab;
            }
        }

        private bool TryGetAnyPrefabIdKey(out PrefabID id)
        {
            id = default;

            try
            {
                foreach (IDictionary dict in EnumeratePrefabIdDictionaries(m_ZTPrefabSystem))
                {
                    foreach (object key in dict.Keys)
                    {
                        if (key is PrefabID pid)
                        {
                            id = pid;
                            return true;
                        }
                    }
                }
            }
            catch
            {
                // No logging here; this can be called early.
            }

            return false;
        }

        internal void DebugDumpPrefabIds(string contains)
        {
#if !DEBUG
            _ = contains;
#else
            try
            {
                int dictCount = 0;
                int keyCount = 0;
                int matchCount = 0;

                foreach (IDictionary dict in EnumeratePrefabIdDictionaries(m_ZTPrefabSystem))
                {
                    dictCount++;

                    foreach (object key in dict.Keys)
                    {
                        keyCount++;

                        if (key is not PrefabID pid)
                        {
                            continue;
                        }

                        string s = pid.ToString();
                        if (s.IndexOf(contains, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            matchCount++;
                            Mod.s_Log.Info($"{Mod.ModTag} PrefabID match ({contains}): {s}");
                        }
                    }
                }

                Mod.s_Log.Info($"{Mod.ModTag} PrefabID scan '{contains}': dicts={dictCount}, keys={keyCount}, matches={matchCount}");
            }
            catch (Exception ex)
            {
                Mod.s_Log.Warn($"{Mod.ModTag} DebugDumpPrefabIds('{contains}') failed: {ex.GetType().Name}");
            }
#endif
        }

        internal void DebugDumpAnyPrefabDonors(int max)
        {
#if !DEBUG
            _ = max;
#else
            try
            {
                int printed = 0;
                int scanned = 0;

                foreach (IDictionary dict in EnumeratePrefabIdDictionaries(m_ZTPrefabSystem))
                {
                    foreach (object key in dict.Keys)
                    {
                        if (printed >= max)
                        {
                            Mod.s_Log.Info($"{Mod.ModTag} Donor dump done: printed={printed}, scanned={scanned}");
                            return;
                        }

                        scanned++;

                        if (key is not PrefabID pid)
                        {
                            continue;
                        }

                        if (!m_ZTPrefabSystem.TryGetPrefab(pid, out PrefabBase prefab) || prefab == null)
                        {
                            continue;
                        }

                        Mod.s_Log.Info($"{Mod.ModTag} Donor candidate: {pid} -> {prefab.name}");
                        printed++;
                    }
                }

                Mod.s_Log.Info($"{Mod.ModTag} Donor dump done: printed={printed}, scanned={scanned}");
            }
            catch (Exception ex)
            {
                Mod.s_Log.Warn($"{Mod.ModTag} DebugDumpAnyPrefabDonors failed: {ex.GetType().Name}");
            }
#endif
        }

        private static IEnumerable<IDictionary> EnumeratePrefabIdDictionaries(PrefabSystem prefabSystem)
        {
            // Scan all Dictionary<PrefabID, *> fields inside PrefabSystem.
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

            FieldInfo[] fields = typeof(PrefabSystem).GetFields(flags);
            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo f = fields[i];

                if (!f.FieldType.IsGenericType)
                {
                    continue;
                }

                Type gen = f.FieldType.GetGenericTypeDefinition();
                if (gen != typeof(Dictionary<,>))
                {
                    continue;
                }

                Type[] args = f.FieldType.GetGenericArguments();
                if (args.Length != 2 || args[0] != typeof(PrefabID))
                {
                    continue;
                }

                object? obj = f.GetValue(prefabSystem);
                if (obj is IDictionary dict && dict.Count > 0)
                {
                    yield return dict;
                }
            }
        }
    }
}
