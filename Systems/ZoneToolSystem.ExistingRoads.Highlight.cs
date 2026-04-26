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
    using Unity.Mathematics;
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
                current = GetToolRoadZoningMode(m_Hovered);

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
            UpdateVanillaRemovalPreview(target, current, desired);
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

        // Queue preview updates so the core zoning system can resize the visible zone blocks.
        // Preview shows the union of current + desired zoning:
        // - added cells appear in the normal translucent style
        // - removed cells stay visible so vanilla can tint them red
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
                    QueuePreviewMode(ecb, m_PreviewRoad, m_PreviewCurrent, m_PreviewCurrent);
                }

                if (wantsPreview)
                {
                    QueuePreviewMode(ecb, roadEntity, current, desired);
                }
            }
            else
            {
                if (hadPreview && !wantsPreview)
                {
                    QueuePreviewMode(ecb, roadEntity, current, current);
                }
                else if (wantsPreview &&
                         (current != m_PreviewCurrent || desired != m_PreviewDesired))
                {
                    QueuePreviewMode(ecb, roadEntity, current, desired);
                }
            }

            m_PreviewRoad = roadEntity;
            m_PreviewCurrent = current;
            m_PreviewDesired = desired;
        }

        private void QueuePreviewMode(EntityCommandBuffer ecb, Entity roadEntity, ZoningMode current, ZoningMode desired)
        {
            if (roadEntity == Entity.Null ||
                !EntityManager.Exists(roadEntity) ||
                !EntityManager.HasBuffer<SubBlock>(roadEntity))
            {
                return;
            }

            DynamicBuffer<SubBlock> subBlocks = EntityManager.GetBuffer<SubBlock>(roadEntity, isReadOnly: true);
            int currentLeft = ShouldDisableLeft(current) ? 0 : 6;
            int currentRight = ShouldDisableRight(current) ? 0 : 6;
            int desiredLeft = ShouldDisableLeft(desired) ? 0 : 6;
            int desiredRight = ShouldDisableRight(desired) ? 0 : 6;

            ZoningPreviewMode preview = new()
            {
                depths = new int2(
                    math.max(currentLeft, desiredLeft),
                    math.max(currentRight, desiredRight))
            };

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

        private void UpdateVanillaRemovalPreview(Entity roadEntity, ZoningMode current, ZoningMode desired)
        {
            bool removeLeft = !ShouldDisableLeft(current) && ShouldDisableLeft(desired);
            bool removeRight = !ShouldDisableRight(current) && ShouldDisableRight(desired);
            bool wantsRemovalPreview = roadEntity != Entity.Null && (removeLeft || removeRight);

            if (!wantsRemovalPreview)
            {
                ClearVanillaRemovalPreviewImmediate();
                return;
            }

            if (roadEntity == m_VanillaPreviewRoad &&
                removeLeft == m_VanillaPreviewLeft &&
                removeRight == m_VanillaPreviewRight)
            {
                return;
            }

            ClearVanillaRemovalPreviewImmediate();

            if (!EntityManager.Exists(roadEntity) ||
                !EntityManager.HasComponent<Curve>(roadEntity) ||
                !EntityManager.HasBuffer<SubBlock>(roadEntity))
            {
                return;
            }

            ApplyVanillaRemovalHighlights(roadEntity, removeLeft, removeRight);

            m_VanillaPreviewRoad = roadEntity;
            m_VanillaPreviewLeft = removeLeft;
            m_VanillaPreviewRight = removeRight;
        }

        private void ApplyVanillaRemovalHighlights(Entity roadEntity, bool removeLeft, bool removeRight)
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
                float dot = BlockUtils.blockCurveDotProduct(block, curve);

                bool isLeftSide = dot > 0f;
                bool highlightSide = isLeftSide ? removeLeft : removeRight;
                bool blocked =
                    (Mod.Settings?.ProtectOccupiedCells ?? true) && BlockUtils.isAnyCellOccupied(ref cells, ref block, ref validArea) ||
                    (Mod.Settings?.ProtectZonedCells ?? true) && BlockUtils.isAnyCellZoned(ref cells, ref block, ref validArea);

                for (int cellIndex = 0; cellIndex < cells.Length; cellIndex++)
                {
                    Cell cell = cells[cellIndex];
                    cell.m_State &= ~CellFlags.Highlight;

                    if (highlightSide && !blocked)
                    {
                        cell.m_State |= CellFlags.Highlight;
                    }

                    cells[cellIndex] = cell;
                }

                if (!EntityManager.HasComponent<Updated>(blockEntity))
                {
                    EntityManager.AddComponent<Updated>(blockEntity);
                }
            }
        }

        private void ClearVanillaRemovalPreviewImmediate()
        {
            if (m_VanillaPreviewRoad != Entity.Null &&
                EntityManager.Exists(m_VanillaPreviewRoad) &&
                EntityManager.HasBuffer<SubBlock>(m_VanillaPreviewRoad))
            {
                DynamicBuffer<SubBlock> subBlocks = EntityManager.GetBuffer<SubBlock>(m_VanillaPreviewRoad, isReadOnly: true);
                for (int i = 0; i < subBlocks.Length; i++)
                {
                    Entity blockEntity = subBlocks[i].m_SubBlock;
                    if (blockEntity == Entity.Null ||
                        !EntityManager.Exists(blockEntity) ||
                        !EntityManager.HasBuffer<Cell>(blockEntity))
                    {
                        continue;
                    }

                    DynamicBuffer<Cell> cells = EntityManager.GetBuffer<Cell>(blockEntity);
                    for (int cellIndex = 0; cellIndex < cells.Length; cellIndex++)
                    {
                        Cell cell = cells[cellIndex];
                        cell.m_State &= ~CellFlags.Highlight;
                        cells[cellIndex] = cell;
                    }

                    if (!EntityManager.HasComponent<Updated>(blockEntity))
                    {
                        EntityManager.AddComponent<Updated>(blockEntity);
                    }
                }
            }

            m_VanillaPreviewRoad = Entity.Null;
            m_VanillaPreviewLeft = false;
            m_VanillaPreviewRight = false;
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
