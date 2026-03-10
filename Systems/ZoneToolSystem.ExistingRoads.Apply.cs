// File: Systems/ZoneToolSystem.ExistingRoads.Apply.cs
// Purpose: Existing Roads hover/select/apply logic.
// Notes:
// - Keeps selection/apply code out of the lifecycle file.
// - Existing Roads only works on road edge entities that own SubBlock buffers.

namespace ZoningToolkit.Systems
{
    using Game.Common;               // Updated
    using Game.Net;                  // Edge, SubBlock
    using Game.Tools;                // RaycastHit
    using Game.Zones;
    using Unity.Entities;            // Entity, EntityCommandBuffer, DynamicBuffer
    using ZoningToolkit.Components;  // ZoningInfo, ZoningInfoUpdated, ZoningMode

    internal sealed partial class ZoneToolSystemExistingRoads
    {
        private void CycleZoningMode( )
        {
            ZoningMode current = m_UISystem.CurrentZoningMode;
            ZoningMode next = current switch
            {
                ZoningMode.Default => ZoningMode.Left,
                ZoningMode.Left => ZoningMode.Right,
                ZoningMode.Right => ZoningMode.None,
                ZoningMode.None => ZoningMode.Default,
                _ => ZoningMode.Default
            };

            m_UISystem.SetZoningModeFromTool(next);
        }

        private void UpdateHover( )
        {
            Entity newHovered = TryGetRaycastRoad(out Entity e) ? e : Entity.Null;
            if (newHovered == m_Hovered)
            {
                return;
            }

            m_Hovered = newHovered;
        }

        private void AddHoveredToSelection( )
        {
            if (m_Hovered == Entity.Null)
            {
                return;
            }

            if (!EntityManager.Exists(m_Hovered))
            {
                return;
            }

            if (!m_Selected.Contains(m_Hovered))
            {
                m_Selected.Add(m_Hovered);
                m_SelectedCount++;

                PlaySelectSound();
            }
        }

        private void ClearSelection( )
        {
            m_Selected.Clear();
            m_SelectedCount = 0;
        }

        private void ApplySelection( )
        {
            if (m_SelectedCount == 0)
            {
                return;
            }

            ZoningMode desired = m_UISystem.CurrentZoningMode;

            // Only create an ECB if at least one entity actually needs changes.
            bool didWork = false;
            EntityCommandBuffer ecb = default;

            foreach (Entity roadEntity in m_Selected)
            {
                if (roadEntity == Entity.Null || !EntityManager.Exists(roadEntity))
                {
                    continue;
                }

                ZoningMode current = EntityManager.HasComponent<ZoningInfo>(roadEntity)
                    ? EntityManager.GetComponentData<ZoningInfo>(roadEntity).zoningMode
                    : ZoningMode.Default;

                if (current == desired)
                {
                    continue;
                }

                if (!didWork)
                {
                    didWork = true;
                    ecb = m_ToolOutputBarrier.CreateCommandBuffer();
                }

                AddOrSetZoningInfo(ecb, roadEntity, desired);
                TagSubBlocksForUpdate(ecb, roadEntity);
            }

            ClearSelection();

            if (didWork)
            {
                PlayBuildSound();
            }
            else
            {
                // No-op feedback: quieter than build thud.
                PlaySelectSound();
            }
        }

        private bool TryGetRaycastRoad(out Entity entity)
        {
            entity = Entity.Null;

            if (!base.GetRaycastResult(out Entity hit, out RaycastHit _))
            {
                return false;
            }

            if (!EntityManager.HasComponent<Edge>(hit))
            {
                return false;
            }

            if (!EntityManager.HasBuffer<SubBlock>(hit))
            {
                return false;
            }

            entity = hit;
            return true;
        }

        private void AddOrSetZoningInfo(EntityCommandBuffer ecb, Entity owner, ZoningMode mode)
        {
            ZoningInfo zi = new ZoningInfo { zoningMode = mode };

            if (EntityManager.HasComponent<ZoningInfo>(owner))
            {
                ecb.SetComponent(owner, zi);
            }
            else
            {
                ecb.AddComponent(owner, zi);
            }
        }

        private void TagSubBlocksForUpdate(EntityCommandBuffer ecb, Entity roadEntity)
        {
            if (!EntityManager.HasBuffer<SubBlock>(roadEntity))
            {
                return;
            }

            DynamicBuffer<SubBlock> subBlocks = EntityManager.GetBuffer<SubBlock>(roadEntity, isReadOnly: true);

            for (int i = 0; i < subBlocks.Length; i++)
            {
                Entity blockEntity = subBlocks[i].m_SubBlock;
                if (blockEntity == Entity.Null)
                {
                    continue;
                }

                if (!EntityManager.Exists(blockEntity))
                {
                    continue;
                }

                if (!EntityManager.HasComponent<ZoningInfoUpdated>(blockEntity))
                {
                    ecb.AddComponent<ZoningInfoUpdated>(blockEntity);
                }

                if (!EntityManager.HasComponent<Updated>(blockEntity))
                {
                    ecb.AddComponent<Updated>(blockEntity);
                }
            }
        }
    }
}
