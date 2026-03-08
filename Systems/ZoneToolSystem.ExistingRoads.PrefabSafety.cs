// File: Systems/ZoneToolSystem.ExistingRoads.PrefabSafety.cs
// Purpose: Provide a guaranteed non-null prefab for ToolBaseSystem.GetPrefab().
// Notes:
// - Some mods assume ToolBaseSystem.GetPrefab() is never null and crash if it is.
// - EnableTool refuses to activate unless a safe prefab is already resolved.
// - Resolution order:
//   1) Fixed vanilla FencePrefab candidates (RoadsServices items).
//   2) Minimal extra fallbacks (known to exist).
//   3) Vanilla tool donor prefab (DefaultToolSystem/NetToolSystem).
//   4) Reflection fallback: find any PrefabID key inside PrefabSystem dictionaries (last resort).
// - Reflection fallback touches PrefabSystem fields only. Runs only when enabling tool, not per-frame.

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

            // 1) Fixed, known prefabs.
            if (TryResolveFromFixedRoadServices(out prefab) ||
                TryResolveFromMinimalFallbacks(out prefab) ||
                TryResolveFromVanillaToolDonor(out prefab) ||
                TryResolveFromReflectionAnyPrefab(out prefab))
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
            return m_ZTDefaultToolSystem.GetPrefab();
        }

        private bool TryResolveFromFixedRoadServices(out PrefabBase prefab)
        {
            PrefabID[] candidates =
            {
                // FencePrefab items commonly present in vanilla.
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
            // DefaultToolSystem donor first.
            PrefabBase d = m_ZTDefaultToolSystem.GetPrefab();
            if (d != null)
            {
                prefab = d;
                return true;
            }

            // NetToolSystem donor as backup.
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
                // Finds the first PrefabID key from any Dictionary<PrefabID, *> field on PrefabSystem.
                // Used only as a last resort to avoid null-prefab tool crashes.
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
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

            FieldInfo[] fields = typeof(PrefabSystem).GetFields(flags);
            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo f = fields[i];

                if (!f.FieldType.IsGenericType)
                {
                    continue;
                }

                if (f.FieldType.GetGenericTypeDefinition() != typeof(Dictionary<,>))
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
