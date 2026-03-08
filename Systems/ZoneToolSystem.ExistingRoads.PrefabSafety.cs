// File: Systems/ZoneToolSystem.ExistingRoads.PrefabSafety.cs
// Purpose: Provide a guaranteed non-null prefab for ToolBaseSystem.GetPrefab().
// Notes:
// - Some mods assume ToolBaseSystem.GetPrefab() is never null and crash if it is.
// - EnableTool refuses to activate unless a safe prefab is already resolved.
// - Resolution order:
//   1) Fixed RoadsServices FencePrefab candidates (Crosswalk, Wide Sidewalk, Trees, Grass).
//   2) Minimal extra fallbacks (Tunnel, Quay).
//   3) Vanilla tool donor prefab (DefaultToolSystem/NetToolSystem).
//   4) Reflection fallback: first resolvable PrefabID inside PrefabSystem dictionaries.
// - Diagnostic dump is on-demand only (Options button).

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

        private void ResetSafePrefabForUI( )
        {
            m_SafePrefabForUI = null;
        }

        // Called by EnableTool(). Guarantees a non-null prefab before activation.
        private bool TryResolveSafePrefabForUI(out PrefabBase prefab)
        {
            // Cached result first.
            if (m_SafePrefabForUI != null)
            {
                prefab = m_SafePrefabForUI;
                return true;
            }

            // 1) Fixed, known RoadsServices prefabs (FencePrefab).
            if (TryResolveFromFixedRoadServices(out prefab))
            {
                m_SafePrefabForUI = prefab;
                return true;
            }

            // 2) Minimal extra fallbacks (kept only because these are known to exist in vanilla).
            if (TryResolveFromMinimalFallbacks(out prefab))
            {
                m_SafePrefabForUI = prefab;
                return true;
            }

            // 3) Donor from vanilla tools (often becomes non-null after any tool opens once).
            if (TryResolveFromVanillaToolDonor(out prefab))
            {
                m_SafePrefabForUI = prefab;
                return true;
            }

            // 4) Last resort: reflection scan to find any resolvable PrefabID.
            if (TryResolveFromReflectionAnyPrefab(out prefab))
            {
                m_SafePrefabForUI = prefab;
                return true;
            }

            // Out parameter must be assigned on false path.
            prefab = null!;
            return false;
        }

        // Used by GetPrefab(). Must not return null when tool is active.
        private PrefabBase GetSafePrefabForUI( )
        {
            if (m_SafePrefabForUI != null)
            {
                return m_SafePrefabForUI;
            }

            if (TryResolveSafePrefabForUI(out PrefabBase resolved))
            {
                return resolved;
            }

            // EnableTool() should refuse activation before this is reachable.
            Mod.s_Log.Warn($"{Mod.ModTag} GetPrefab called without safe prefab; falling back to DefaultToolSystem.GetPrefab().");
            PrefabBase fallback = m_ZTDefaultToolSystem.GetPrefab();
            return fallback;
        }

        private bool TryResolveFromFixedRoadServices(out PrefabBase prefab)
        {
            PrefabID[] candidates =
            {
                // RoadsServices group (FencePrefab). Verified stable vanilla items.
                new PrefabID("FencePrefab", "Crosswalk"),
                new PrefabID("FencePrefab", "Wide Sidewalk"),
                new PrefabID("FencePrefab", "WideSidewalk"),
                new PrefabID("FencePrefab", "Trees"),
                new PrefabID("FencePrefab", "Grass"),
            };

            for (int i = 0; i < candidates.Length; i++)
            {
                if (m_ZTPrefabSystem.TryGetPrefab(candidates[i], out PrefabBase p) && p != null)
                {
                    prefab = p;
                    return true;
                }
            }

            prefab = null!;
            return false;
        }

        private bool TryResolveFromMinimalFallbacks(out PrefabBase prefab)
        {
            PrefabID[] candidates =
            {
                // Minimal extra fallbacks only.
                new PrefabID("FencePrefab", "Tunnel"),
                new PrefabID("FencePrefab", "Quay"),
            };

            for (int i = 0; i < candidates.Length; i++)
            {
                if (m_ZTPrefabSystem.TryGetPrefab(candidates[i], out PrefabBase p) && p != null)
                {
                    prefab = p;
                    return true;
                }
            }

            prefab = null!;
            return false;
        }

        private bool TryResolveFromVanillaToolDonor(out PrefabBase prefab)
        {
            // DefaultToolSystem prefab is usually the best donor (tool infrastructure expects it).
            PrefabBase d = m_ZTDefaultToolSystem.GetPrefab();
            if (d != null)
            {
                prefab = d;
                return true;
            }

            // NetToolSystem donor as secondary option.
            PrefabBase n = m_NetToolSystem.GetPrefab();
            if (n != null)
            {
                prefab = n;
                return true;
            }

            prefab = null!;
            return false;
        }

        private bool TryResolveFromReflectionAnyPrefab(out PrefabBase prefab)
        {
            try
            {
                if (!TryGetAnyPrefabIdKey(out PrefabID anyId))
                {
                    prefab = null!;
                    return false;
                }

                if (m_ZTPrefabSystem.TryGetPrefab(anyId, out PrefabBase p) && p != null)
                {
                    prefab = p;
                    return true;
                }
            }
            catch (Exception ex)
            {
                Mod.s_Log.Warn($"{Mod.ModTag} PrefabSafety reflection resolve failed: {ex.GetType().Name}");
            }

            prefab = null!;
            return false;
        }

        private bool TryGetAnyPrefabIdKey(out PrefabID id)
        {
            id = default;

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

            return false;
        }

        // Called by BridgeUI on button click. Compact in Release; expanded in DEBUG.
        internal void DumpDebugReportOnDemand( )
        {
            try
            {
                Mod.s_Log.Info($"{Mod.ModTag} Diagnostic report begin");

                PrefabBase d = m_ZTDefaultToolSystem.GetPrefab();
                PrefabBase n = m_NetToolSystem.GetPrefab();

                Mod.s_Log.Info($"{Mod.ModTag} Donor(DefaultToolSystem).GetPrefab = {(d != null ? d.name : "<null>")}");
                Mod.s_Log.Info($"{Mod.ModTag} Donor(NetToolSystem).GetPrefab     = {(n != null ? n.name : "<null>")}");
                Mod.s_Log.Info($"{Mod.ModTag} Cached safe prefab               = {(m_SafePrefabForUI != null ? m_SafePrefabForUI.name : "<null>")}");

                // Fixed-candidate spot checks.
                DumpCandidate("FencePrefab", "Crosswalk");
                DumpCandidate("FencePrefab", "Wide Sidewalk");
                DumpCandidate("FencePrefab", "WideSidewalk");
                DumpCandidate("FencePrefab", "Trees");
                DumpCandidate("FencePrefab", "Grass");
                DumpCandidate("FencePrefab", "Tunnel");
                DumpCandidate("FencePrefab", "Quay");

                bool ok = TryResolveSafePrefabForUI(out PrefabBase resolved);
                Mod.s_Log.Info($"{Mod.ModTag} Resolve now = {ok} -> {(ok ? resolved.name : "<none>")}");

#if DEBUG
                // DEBUG-only: limited, deduped scan to validate PrefabID availability.
                DumpPrefabIdMatchesUnique("Crosswalk", max: 40);
                DumpPrefabIdMatchesUnique("Sidewalk", max: 40);
#endif

                Mod.s_Log.Info($"{Mod.ModTag} Diagnostic report end");
            }
            catch (Exception ex)
            {
                Mod.s_Log.Warn($"{Mod.ModTag} Diagnostic report failed: {ex.GetType().Name}");
            }
        }

        private void DumpCandidate(string typeName, string name)
        {
            PrefabID id = new PrefabID(typeName, name);

            // Null-safe output avoids nullable warnings and keeps logs stable.
            string result = "<missing>";
            if (m_ZTPrefabSystem.TryGetPrefab(id, out PrefabBase prefab) && prefab != null)
            {
                result = prefab.name;
            }

            Mod.s_Log.Info($"{Mod.ModTag} Candidate {id} -> {result}");
        }

#if DEBUG
        private void DumpPrefabIdMatchesUnique(string contains, int max)
        {
            HashSet<string> printed = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
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
                    if (s.IndexOf(contains, StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        continue;
                    }

                    matchCount++;

                    if (printed.Count >= max)
                    {
                        continue;
                    }

                    if (printed.Add(s))
                    {
                        Mod.s_Log.Info($"{Mod.ModTag} PrefabID match ({contains}): {s}");
                    }
                }
            }

            Mod.s_Log.Info($"{Mod.ModTag} PrefabID scan '{contains}': dicts={dictCount}, keys={keyCount}, matches={matchCount}, printed={printed.Count}");
        }
#endif

        private static IEnumerable<IDictionary> EnumeratePrefabIdDictionaries(PrefabSystem prefabSystem)
        {
            // yield return avoids building an intermediate list; enumerates dictionaries lazily.
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
