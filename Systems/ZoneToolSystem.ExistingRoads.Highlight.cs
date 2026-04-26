// File: Systems/ZoneToolSystem.ExistingRoads.Highlight.cs
// Purpose: Hover highlight / outline logic for Existing Roads tool.
// Notes:
// - Uses vanilla highlight outline (Highlighted + BatchesUpdated).
// - Provides ECB and immediate cleanup paths.

namespace ZoningToolkit.Systems
{
    using Game.Common;      // Updated, BatchesUpdated
    using Game.Net;         // Curve, Edge
    using Game.Tools;       // Highlighted
    using Game.Zones;       // Block, Cell, SubBlock, ValidArea
    using Unity.Entities;   // Entity, EntityCommandBuffer
    using ZoningToolkit.Components; // ZoningInfo, ZoningMode, ZoningPreviewMode
    using ZoningToolkit.Utils; // BlockUtils

    internal sealed partial class ZoneToolSystemExistingRoads
    {
        // Highlight only when the hovered road would actually change.
        private void UpdateHoverHighlight(EntityCommandBuffer ecb)
        {
            Entity target = Entity.Null;
            ZoningMode desired = ZoningMode.Default;
            ZoningMode current = ZoningMode.Default;

            if (m_Hovered != Entity.Null && EntityManager.Exists(m_Hovered))
            {
                desired = m_UISystem.CurrentZoningMode;
                current = GetEffectiveRoadZoningMode(m_Hovered);

                if (current != desired)
                {
                    target = m_Hovered;
                }
            }

            if (target != m_Highlighted)
            {
                if (m_Highlighted != Entity.Null)
                {
                    SetHighlighted(ecb, m_Highlighted, value: false);
                }

                m_Highlighted = target;

                if (m_Highlighted != Entity.Null)
                {
                    SetHighlighted(ecb, m_Highlighted, value: true);
                }
            }

            UpdateRoadPreview(ecb, target, current, desired);
        }

        // Immediate cleanup path (no ECB).
        private void ClearHoverHighlightImmediate( )
        {
            if (m_Highlighted == Entity.Null)
            {
                return;
            }

            if (!EntityManager.Exists(m_Highlighted))
            {
                m_Highlighted = Entity.Null;
                return;
            }

            SetHighlightedImmediate(m_Highlighted, value: false);
            m_Highlighted = Entity.Null;
        }

        // Queue preview updates so the core zoning system can resize the real zone blocks.
        // This gives ZT a true UER preview instead of only a road outline.
        private void UpdateRoadPreview(EntityCommandBuffer ecb, Entity roadEntity, ZoningMode current, ZoningMode desired)
        {
            if (roadEntity == m_PreviewRoad &&
                current == m_PreviewCurrent &&
                desired == m_PreviewDesired)
            {
                return;
            }

            bool hadPreview = m_PreviewRoad != Entity.Null && m_PreviewCurrent != m_PreviewDesired;
            bool wantsPreview = roadEntity != Entity.Null && current != desired;

            if (roadEntity != m_PreviewRoad)
            {
                if (hadPreview)
                {
                    QueuePreviewMode(ecb, m_PreviewRoad, m_PreviewCurrent);
                }

                if (wantsPreview)
                {
                    QueuePreviewMode(ecb, roadEntity, desired);
                }
            }
            else
            {
                if (hadPreview && !wantsPreview)
                {
                    QueuePreviewMode(ecb, roadEntity, current);
                }
                else if (wantsPreview &&
                         (current != m_PreviewCurrent || desired != m_PreviewDesired))
                {
                    QueuePreviewMode(ecb, roadEntity, desired);
                }
            }

            m_PreviewRoad = roadEntity;
            m_PreviewCurrent = current;
            m_PreviewDesired = desired;
        }

        private void QueuePreviewMode(EntityCommandBuffer ecb, Entity roadEntity, ZoningMode mode)
        {
            if (roadEntity == Entity.Null ||
                !EntityManager.Exists(roadEntity) ||
                !EntityManager.HasBuffer<SubBlock>(roadEntity))
            {
                return;
            }

            DynamicBuffer<SubBlock> subBlocks = EntityManager.GetBuffer<SubBlock>(roadEntity, isReadOnly: true);
            ZoningPreviewMode preview = new() { zoningMode = mode };

            for (int i = 0; i < subBlocks.Length; i++)
            {
                Entity blockEntity = subBlocks[i].m_SubBlock;
                if (blockEntity == Entity.Null ||
                    !EntityManager.Exists(blockEntity) ||
                    !EntityManager.HasComponent<Block>(blockEntity) ||
                    !EntityManager.HasComponent<ValidArea>(blockEntity) ||
                    !EntityManager.HasBuffer<Cell>(blockEntity))
                {
                    continue;
                }

                if (EntityManager.HasComponent<ZoningPreviewMode>(blockEntity))
                {
                    ecb.SetComponent(blockEntity, preview);
                }
                else
                {
                    ecb.AddComponent(blockEntity, preview);
                }

                if (!EntityManager.HasComponent<Updated>(blockEntity))
                {
                    ecb.AddComponent<Updated>(blockEntity);
                }
            }
        }

        private void ClearRoadPreviewImmediate( )
        {
            if (m_PreviewRoad != Entity.Null &&
                EntityManager.Exists(m_PreviewRoad) &&
                m_PreviewCurrent != m_PreviewDesired)
            {
                ApplyPreviewModeImmediate(m_PreviewRoad, m_PreviewCurrent);
            }

            m_PreviewRoad = Entity.Null;
            m_PreviewCurrent = ZoningMode.Default;
            m_PreviewDesired = ZoningMode.Default;
        }

        private void ApplyPreviewModeImmediate(Entity roadEntity, ZoningMode mode)
        {
            if (roadEntity == Entity.Null ||
                !EntityManager.Exists(roadEntity) ||
                !EntityManager.HasComponent<Curve>(roadEntity) ||
                !EntityManager.HasBuffer<SubBlock>(roadEntity))
            {
                return;
            }

            Curve curve = EntityManager.GetComponentData<Curve>(roadEntity);
            DynamicBuffer<SubBlock> subBlocks = EntityManager.GetBuffer<SubBlock>(roadEntity, isReadOnly: true);
            bool protectOccupiedCells = Mod.Settings?.ProtectOccupiedCells ?? true;
            bool protectZonedCells = Mod.Settings?.ProtectZonedCells ?? false;

            for (int i = 0; i < subBlocks.Length; i++)
            {
                Entity blockEntity = subBlocks[i].m_SubBlock;
                if (blockEntity == Entity.Null ||
                    !EntityManager.Exists(blockEntity) ||
                    !EntityManager.HasComponent<Block>(blockEntity) ||
                    !EntityManager.HasComponent<ValidArea>(blockEntity) ||
                    !EntityManager.HasBuffer<Cell>(blockEntity))
                {
                    continue;
                }

                Block block = EntityManager.GetComponentData<Block>(blockEntity);
                ValidArea validArea = EntityManager.GetComponentData<ValidArea>(blockEntity);
                DynamicBuffer<Cell> cells = EntityManager.GetBuffer<Cell>(blockEntity);

                bool blocked =
                    (protectOccupiedCells && BlockUtils.isAnyCellOccupied(ref cells, ref block, ref validArea)) ||
                    (protectZonedCells && BlockUtils.isAnyCellZoned(ref cells, ref block, ref validArea));
                if (blocked)
                {
                    continue;
                }

                float dot = BlockUtils.blockCurveDotProduct(block, curve);
                BlockUtils.applyBlockSizes(dot, mode, ref validArea, ref block);

                EntityManager.SetComponentData(blockEntity, block);
                EntityManager.SetComponentData(blockEntity, validArea);

                if (!EntityManager.HasComponent<Updated>(blockEntity))
                {
                    EntityManager.AddComponent<Updated>(blockEntity);
                }
            }
        }

        private void SetHighlighted(EntityCommandBuffer ecb, Entity entity, bool value)
        {
            if (value)
            {
                if (!EntityManager.HasComponent<Highlighted>(entity))
                {
                    ecb.AddComponent<Highlighted>(entity);
                }

                if (!EntityManager.HasComponent<Updated>(entity))
                {
                    ecb.AddComponent<Updated>(entity);
                }
            }
            else
            {
                if (EntityManager.HasComponent<Highlighted>(entity))
                {
                    ecb.RemoveComponent<Highlighted>(entity);
                }

                if (!EntityManager.HasComponent<Updated>(entity))
                {
                    ecb.AddComponent<Updated>(entity);
                }
            }

            if (EntityManager.HasComponent<Edge>(entity))
            {
                Edge edge = EntityManager.GetComponentData<Edge>(entity);

                if (edge.m_Start != Entity.Null)
                {
                    ecb.AddComponent<BatchesUpdated>(edge.m_Start);
                }

                if (edge.m_End != Entity.Null)
                {
                    ecb.AddComponent<BatchesUpdated>(edge.m_End);
                }
            }
        }

        // Immediate version (no ECB).
        private void SetHighlightedImmediate(Entity entity, bool value)
        {
            if (value)
            {
                if (!EntityManager.HasComponent<Highlighted>(entity))
                {
                    EntityManager.AddComponent<Highlighted>(entity);
                }

                if (!EntityManager.HasComponent<Updated>(entity))
                {
                    EntityManager.AddComponent<Updated>(entity);
                }
            }
            else
            {
                if (EntityManager.HasComponent<Highlighted>(entity))
                {
                    EntityManager.RemoveComponent<Highlighted>(entity);
                }

                if (!EntityManager.HasComponent<Updated>(entity))
                {
                    EntityManager.AddComponent<Updated>(entity);
                }
            }

            if (EntityManager.HasComponent<Edge>(entity))
            {
                Edge edge = EntityManager.GetComponentData<Edge>(entity);

                AddBatchesUpdatedImmediate(edge.m_Start);
                AddBatchesUpdatedImmediate(edge.m_End);
            }
        }

        private void AddBatchesUpdatedImmediate(Entity node)
        {
            if (node == Entity.Null)
            {
                return;
            }

            if (!EntityManager.Exists(node))
            {
                return;
            }

            if (!EntityManager.HasComponent<BatchesUpdated>(node))
            {
                EntityManager.AddComponent<BatchesUpdated>(node);
            }
        }
    }
}
