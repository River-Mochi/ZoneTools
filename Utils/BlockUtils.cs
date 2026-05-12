// File: Utils/BlockUtils.cs
// Purpose: Helpers for deciding which side of a road a block belongs to,
// and for enabling/disabling zone depth on that side.
// Notes:
// - Zone depth is controlled by validArea.m_Area.w and block.m_Size.y.
// - Value 0 disables zoning on that side.
// - Value 6 restores normal zoning depth.
// - Protection checks for occupied cells and painted zoning are handled here.

namespace ZoningToolkit.Utils
{
    using Colossal.Mathematics;     // MathUtils
    using Game.Net;                 // Curve
    using Game.Zones;               // Block, Cell, ValidArea
    using Unity.Entities;           // Entity
    using Unity.Mathematics;        // float2
    using ZoningToolkit.Components; // ZoningInfo, ZoningMode

    internal static class BlockUtils
    {
        private const float SideEpsilon = 0.01f;

        public static void applyBlockSizes(float dotProduct, ZoningMode zoningMode, ref ValidArea validArea, ref Block block)
        {
            // Set zone depth to 0 to disable zoning on that side.
            // Set zone depth to 6 to keep or restore normal zoning on that side.
            if (dotProduct > 0)
            {
                if (zoningMode == ZoningMode.Right || zoningMode == ZoningMode.None)
                {
                    validArea.m_Area.w = 0;
                    block.m_Size.y = 0;
                }
                else
                {
                    validArea.m_Area.w = 6;
                    block.m_Size.y = 6;
                }
            }
            else
            {
                if (zoningMode == ZoningMode.Left || zoningMode == ZoningMode.None)
                {
                    validArea.m_Area.w = 0;
                    block.m_Size.y = 0;
                }
                else
                {
                    validArea.m_Area.w = 6;
                    block.m_Size.y = 6;
                }
            }
        }

        public static void applyBlockDepth(int depth, ref ValidArea validArea, ref Block block)
        {
            validArea.m_Area.w = depth;
            block.m_Size.y = depth;
        }

        public static void applyPreviewDepths(bool isLeftSide, int2 depths, ref ValidArea validArea, ref Block block)
        {
            int depth = isLeftSide ? depths.x : depths.y;
            applyBlockDepth(depth, ref validArea, ref block);
        }

        public static int getDepthForMode(bool isLeftSide, ZoningMode zoningMode)
        {
            bool disabled = isLeftSide
                ? zoningMode == ZoningMode.Right || zoningMode == ZoningMode.None
                : zoningMode == ZoningMode.Left || zoningMode == ZoningMode.None;

            return disabled ? 0 : 6;
        }

        public static bool shouldProtectDepthReduction(
            int targetDepth,
            ref DynamicBuffer<Cell> cells,
            ref Block block,
            ref ValidArea validArea,
            bool protectOccupiedCells,
            bool protectZonedCells)
        {
            int currentDepth = math.max(block.m_Size.y, validArea.m_Area.w);
            if (targetDepth >= currentDepth)
            {
                return false;
            }

            return
                (protectOccupiedCells && isAnyCellOccupied(ref cells, ref block, ref validArea)) ||
                (protectZonedCells && isAnyCellZoned(ref cells, ref block, ref validArea));
        }

        public static bool isBlockOnLeft(Block block, Curve curve)
        {
            float dot = blockCurveDotProduct(block, curve);
            if (dot > SideEpsilon)
            {
                return true;
            }

            if (dot < -SideEpsilon)
            {
                return false;
            }

            return math.dot(new float2(1f, 1f), block.m_Direction) < 0f;
        }

        public static float blockCurveDotProduct(Block block, Curve curve)
        {
#if DEBUG
            Mod.s_Log.Debug($"Block direction {block.m_Direction}");
            Mod.s_Log.Debug($"Block position {block.m_Position}");
#endif

            float dot = getBlockCurveDotProduct(block, curve);

#if DEBUG
            Mod.s_Log.Debug($"Dot product: {dot}");
#endif

            return dot;
        }

        private static float getBlockCurveDotProduct(Block block, Curve curve)
        {
            MathUtils.Distance(curve.m_Bezier.xz, block.m_Position.xz, out float t);

            float oneMinusT = 1f - t;
            float2 tangent =
                3f * oneMinusT * oneMinusT * (curve.m_Bezier.xz.b - curve.m_Bezier.xz.a) +
                6f * oneMinusT * t * (curve.m_Bezier.xz.c - curve.m_Bezier.xz.b) +
                3f * t * t * (curve.m_Bezier.xz.d - curve.m_Bezier.xz.c);

            tangent = math.normalizesafe(tangent);
            if (math.lengthsq(tangent) <= 1E-07f)
            {
                return 0f;
            }

            float2 perpendicular = new float2(tangent.y, -tangent.x);
            return math.dot(perpendicular, block.m_Direction);
        }

        public static void editBlockSizes(float dotProduct, ZoningInfo newZoningInfo, ValidArea validArea, Block block, Entity entity, EntityCommandBuffer ecb)
        {
            applyBlockSizes(dotProduct, newZoningInfo.zoningMode, ref validArea, ref block);

            ecb.SetComponent(entity, validArea);
            ecb.SetComponent(entity, block);
        }

        public static bool isAnyCellOccupied(ref DynamicBuffer<Cell> cells, ref Block block, ref ValidArea validArea)
        {
#if DEBUG
            Mod.s_Log.Debug($"Block size x: {block.m_Size.x}, y: {block.m_Size.y}");
            Mod.s_Log.Debug($"Valid area x: {validArea.m_Area.x}, y: {validArea.m_Area.y}, z: {validArea.m_Area.z}, w: {validArea.m_Area.w}");
#endif

            // No active area means there is nothing to scan.
            if (validArea.m_Area.y * validArea.m_Area.w == 0)
            {
                return false;
            }

            // Scan only the active zone area.
            // Any occupied cell blocks the edit when the protection option is enabled.
            for (int z = validArea.m_Area.z; z < validArea.m_Area.w; z++)
            {
                for (int x = validArea.m_Area.x; x < validArea.m_Area.y; x++)
                {
                    int index = z * block.m_Size.x + x;
                    if (index < 0 || index >= cells.Length)
                    {
                        continue;
                    }

                    Cell cell = cells[index];
                    if ((cell.m_State & CellFlags.Occupied) != 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool isAnyCellZoned(ref DynamicBuffer<Cell> cells, ref Block block, ref ValidArea validArea)
        {
            // No active area means there is nothing to scan.
            if (validArea.m_Area.y * validArea.m_Area.w == 0)
            {
                return false;
            }

            // Scan only the active zone area.
            // Painted-zone protection follows the CS2 lot model: a grown building
            // still sits on painted RCIO zoning, so this protects both empty and occupied painted cells.
            for (int z = validArea.m_Area.z; z < validArea.m_Area.w; z++)
            {
                for (int x = validArea.m_Area.x; x < validArea.m_Area.y; x++)
                {
                    int index = z * block.m_Size.x + x;
                    if (index < 0 || index >= cells.Length)
                    {
                        continue;
                    }

                    Cell cell = cells[index];
                    if (cell.m_Zone.m_Index != ZoneType.None.m_Index)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
