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
    using Colossal.Mathematics;     // Bezier4x2, MathUtils
    using Game.Net;                 // Curve
    using Game.Zones;               // Block, Cell, ValidArea
    using Unity.Entities;           // Entity
    using Unity.Mathematics;        // float2
    using UnityEngine;              // Vector2
    using ZoningToolkit.Components; // ZoningInfo

    internal static class BlockUtils
    {
        public static float blockCurveDotProduct(Block block, Curve curve)
        {
#if DEBUG
            Mod.s_Log.Debug($"Block direction {block.m_Direction}");
            Mod.s_Log.Debug($"Block position {block.m_Position}");
#endif

            // Find the closest point on the road curve to this block.
            MathUtils.Distance(curve.m_Bezier.xz, block.m_Position.xz, out float t);

            // Build a perpendicular from the road tangent at that point.
            // The sign of the dot product tells which side of the road the block is on.
            Vector2 tangent = GetTangent(curve.m_Bezier.xz, t);
            Vector2 perpendicular = new(tangent.y, -tangent.x);

            float dot = Vector2.Dot(perpendicular, block.m_Direction);

#if DEBUG
            Mod.s_Log.Debug($"Dot product: {dot}");
#endif

            return dot;
        }

        public static void editBlockSizes(float dotProduct, ZoningInfo newZoningInfo, ValidArea validArea, Block block, Entity entity, EntityCommandBuffer ecb)
        {
            // Set zone depth to 0 to disable zoning on that side.
            // Set zone depth to 6 to keep or restore normal zoning on that side.
            if (dotProduct > 0)
            {
                if (newZoningInfo.zoningMode == ZoningMode.Right || newZoningInfo.zoningMode == ZoningMode.None)
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
                if (newZoningInfo.zoningMode == ZoningMode.Left || newZoningInfo.zoningMode == ZoningMode.None)
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
            // Any painted zone blocks the edit when the protection option is enabled.
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

        public static Vector2 GetTangent(Bezier4x2 curve, float t)
        {
            // Derivative of the bezier curve at t.
            // Used to build the side-test perpendicular in blockCurveDotProduct().
            float2 derivative =
                3 * math.pow(1 - t, 2) * (curve.b - curve.a) +
                6 * (1 - t) * t * (curve.c - curve.b) +
                3 * math.pow(t, 2) * (curve.d - curve.c);

            return new Vector2(derivative.x, derivative.y);
        }
    }
}
