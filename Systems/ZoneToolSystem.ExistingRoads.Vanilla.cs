// File: Systems/ZoneToolSystem.ExistingRoads.Vanilla.cs
// Purpose: Vanilla road-zoning compatibility helpers for Update Existing Roads.
// Notes:
// - Reads/writes the game's per-side ZonesDisabled flags when present.
// - Keeps Zone Tools' 4-mode UI, but aligns the road state with vanilla.

namespace ZoningToolkit.Systems
{
    using Game.Common;               // Updated
    using Game.Net;                  // Upgraded
    using Game.Prefabs;              // CompositionFlags
    using Unity.Entities;            // Entity, EntityCommandBuffer
    using ZoningToolkit.Components;  // ZoningInfo, ZoningMode

    internal sealed partial class ZoneToolSystemExistingRoads
    {
        private static readonly CompositionFlags.Side kZonesDisabled = CompositionFlags.Side.ZonesDisabled;

        private ZoningMode GetEffectiveRoadZoningMode(Entity roadEntity)
        {
            if (roadEntity != Entity.Null &&
                EntityManager.Exists(roadEntity) &&
                EntityManager.HasComponent<Upgraded>(roadEntity))
            {
                Upgraded upgraded = EntityManager.GetComponentData<Upgraded>(roadEntity);
                bool leftDisabled = (upgraded.m_Flags.m_Left & kZonesDisabled) != 0;
                bool rightDisabled = (upgraded.m_Flags.m_Right & kZonesDisabled) != 0;

                // Vanilla state wins when a side is explicitly disabled.
                if (leftDisabled || rightDisabled)
                {
                    return GetZoningModeFromDisabledSides(leftDisabled, rightDisabled);
                }
            }

            if (roadEntity != Entity.Null &&
                EntityManager.Exists(roadEntity) &&
                EntityManager.HasComponent<ZoningInfo>(roadEntity))
            {
                return EntityManager.GetComponentData<ZoningInfo>(roadEntity).zoningMode;
            }

            return ZoningMode.Default;
        }

        private void SyncVanillaZoneFlags(EntityCommandBuffer ecb, Entity roadEntity, ZoningMode mode)
        {
            if (roadEntity == Entity.Null || !EntityManager.Exists(roadEntity))
            {
                return;
            }

            bool hasUpgraded = EntityManager.HasComponent<Upgraded>(roadEntity);
            CompositionFlags flags = hasUpgraded
                ? EntityManager.GetComponentData<Upgraded>(roadEntity).m_Flags
                : default;

            flags.m_Left = SetZonesDisabled(flags.m_Left, ShouldDisableLeft(mode));
            flags.m_Right = SetZonesDisabled(flags.m_Right, ShouldDisableRight(mode));

            bool hasAnyUpgradeFlags = !flags.Equals(default(CompositionFlags));

            if (hasAnyUpgradeFlags)
            {
                Upgraded upgraded = new Upgraded { m_Flags = flags };

                if (hasUpgraded)
                {
                    ecb.SetComponent(roadEntity, upgraded);
                }
                else
                {
                    ecb.AddComponent(roadEntity, upgraded);
                }
            }
            else if (hasUpgraded)
            {
                ecb.RemoveComponent<Upgraded>(roadEntity);
            }

            if (!EntityManager.HasComponent<Updated>(roadEntity))
            {
                ecb.AddComponent<Updated>(roadEntity);
            }
        }

        private static ZoningMode GetZoningModeFromDisabledSides(bool leftDisabled, bool rightDisabled)
        {
            if (leftDisabled && rightDisabled)
            {
                return ZoningMode.None;
            }

            if (leftDisabled)
            {
                return ZoningMode.Right;
            }

            if (rightDisabled)
            {
                return ZoningMode.Left;
            }

            return ZoningMode.Default;
        }

        private static bool ShouldDisableLeft(ZoningMode mode)
        {
            return mode == ZoningMode.Right || mode == ZoningMode.None;
        }

        private static bool ShouldDisableRight(ZoningMode mode)
        {
            return mode == ZoningMode.Left || mode == ZoningMode.None;
        }

        private static CompositionFlags.Side SetZonesDisabled(CompositionFlags.Side side, bool disabled)
        {
            if (disabled)
            {
                return side | kZonesDisabled;
            }

            return side & ~kZonesDisabled;
        }
    }
}
