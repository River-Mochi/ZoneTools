// File: Systems/ZoneToolSystem.ExistingRoads.Vanilla.cs
// Purpose: Vanilla road-zoning compatibility helpers for Update Existing Roads.
// Notes:
// - Reads/writes the game's per-side ZonesDisabled flags when present.
// - Keeps Zone Tools' 4-mode UI, but aligns the road state with vanilla.

namespace ZoningToolkit.Systems
{
    using Game.Common;               // Updated
    using Game.Net;                  // Curve, Upgraded
    using Game.Prefabs;              // CompositionFlags
    using Game.Zones;                // Block, ValidArea, SubBlock
    using Unity.Entities;            // Entity, EntityCommandBuffer
    using ZoningToolkit.Components;  // ZoningInfo, ZoningMode
    using ZoningToolkit.Utils;       // BlockUtils

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

            // When vanilla flags are clear, prefer the actual block layout over stale legacy data.
            if (TryGetModeFromBlockLayout(roadEntity, out ZoningMode blockLayoutMode))
            {
                return blockLayoutMode;
            }

            if (roadEntity != Entity.Null &&
                EntityManager.Exists(roadEntity) &&
                EntityManager.HasComponent<ZoningInfo>(roadEntity))
            {
                return EntityManager.GetComponentData<ZoningInfo>(roadEntity).zoningMode;
            }

            return ZoningMode.Default;
        }

        private bool TryGetModeFromBlockLayout(Entity roadEntity, out ZoningMode mode)
        {
            mode = ZoningMode.Default;

            if (roadEntity == Entity.Null ||
                !EntityManager.Exists(roadEntity) ||
                !EntityManager.HasComponent<Curve>(roadEntity) ||
                !EntityManager.HasBuffer<SubBlock>(roadEntity))
            {
                return false;
            }

            Curve curve = EntityManager.GetComponentData<Curve>(roadEntity);
            DynamicBuffer<SubBlock> subBlocks = EntityManager.GetBuffer<SubBlock>(roadEntity, isReadOnly: true);

            bool sawLeft = false;
            bool sawRight = false;
            bool leftEnabled = false;
            bool leftDisabled = false;
            bool rightEnabled = false;
            bool rightDisabled = false;

            for (int i = 0; i < subBlocks.Length; i++)
            {
                Entity blockEntity = subBlocks[i].m_SubBlock;
                if (blockEntity == Entity.Null ||
                    !EntityManager.Exists(blockEntity) ||
                    !EntityManager.HasComponent<Block>(blockEntity) ||
                    !EntityManager.HasComponent<ValidArea>(blockEntity))
                {
                    continue;
                }

                Block block = EntityManager.GetComponentData<Block>(blockEntity);
                ValidArea validArea = EntityManager.GetComponentData<ValidArea>(blockEntity);
                float dot = BlockUtils.blockCurveDotProduct(block, curve);
                bool enabled = block.m_Size.y > 0 && validArea.m_Area.w > 0;

                if (dot > 0f)
                {
                    sawLeft = true;
                    leftEnabled |= enabled;
                    leftDisabled |= !enabled;
                }
                else
                {
                    sawRight = true;
                    rightEnabled |= enabled;
                    rightDisabled |= !enabled;
                }
            }

            // Mixed block states usually mean protections or partial edits; keep the legacy fallback there.
            if (!sawLeft || !sawRight ||
                (leftEnabled && leftDisabled) ||
                (rightEnabled && rightDisabled))
            {
                return false;
            }

            mode = GetZoningModeFromDisabledSides(leftDisabled, rightDisabled);
            return true;
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
