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
    using ZoningToolkit.Components; // ZoningInfo, ZoningMode, ZoningPreviewMode, ZoningRestoreMode
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
        // EasyZoning-style behavior is more reliable for ZT right now:
        // show the actual desired side depths on hover, even for removals.
        private void UpdateRoadPreview(EntityCommandBuffer ecb, Entity roadEntity, ZoningMode current, ZoningMode desired)
        {
            if (roadEntity == m_PreviewRoad &&
                current == m_PreviewCurrent &&
                desired == m_PreviewDesired)
            {
                if (roadEntity != Entity.Null && current != desired)
                {
                    // Keep the preview warm while hovering/cycling. Add-previews can need
                    // a later game refresh after vanilla ZonesDisabled flags change.
                    TagRoadForUpdate(ecb, roadEntity);
                    TagSubBlocksForUpdate(ecb, roadEntity);

#if DEBUG
                    m_DebugPreviewRefreshTick++;
                    if ((m_DebugPreviewRefreshTick % 30) == 0)
                    {
                        LogRoadPreviewState("preview refresh", roadEntity, current, desired);
                    }
#endif
                }

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

#if DEBUG
            m_DebugPreviewRefreshTick = 0;
            LogRoadPreviewState("preview state", roadEntity, current, desired);
#endif
        }

        private void QueuePreviewMode(EntityCommandBuffer ecb, Entity roadEntity, ZoningMode current, ZoningMode desired)
        {
            if (roadEntity == Entity.Null ||
                !EntityManager.Exists(roadEntity) ||
                !EntityManager.HasBuffer<SubBlock>(roadEntity))
            {
                return;
            }

            ZoningMode visualMode = current != desired ? desired : current;

            if (current != desired)
            {
                ZoningPreviewMode preview = new()
                {
                    depths = GetDepthsForMode(visualMode)
                };

                if (EntityManager.HasComponent<ZoningPreviewMode>(roadEntity))
                {
                    ecb.SetComponent(roadEntity, preview);
                }
                else
                {
                    ecb.AddComponent(roadEntity, preview);
                }

                ecb.RemoveComponent<ZoningRestoreMode>(roadEntity);

                // Temporarily align vanilla's per-side ZonesDisabled flags too.
                // This lets add-previews show on roads whose zones are currently disabled.
                SyncVanillaZoneFlags(ecb, roadEntity, visualMode);
            }
            else
            {
                ecb.RemoveComponent<ZoningPreviewMode>(roadEntity);

                ZoningRestoreMode restore = new()
                {
                    depths = GetDepthsForMode(visualMode)
                };

                if (EntityManager.HasComponent<ZoningRestoreMode>(roadEntity))
                {
                    ecb.SetComponent(roadEntity, restore);
                }
                else
                {
                    ecb.AddComponent(roadEntity, restore);
                }

                SyncVanillaZoneFlags(ecb, roadEntity, visualMode);
            }

            // Apply the visual block depth immediately so add-side previews do not
            // wait on vanilla's later refresh pass, which can leave disabled sides hidden.
            ApplyPreviewModeImmediate(roadEntity, visualMode);

            TagRoadForUpdate(ecb, roadEntity);
            TagSubBlocksForUpdate(ecb, roadEntity);

#if DEBUG
            LogRoadPreviewState("preview queue", roadEntity, current, desired);
#endif
        }

        private void ClearRoadPreviewImmediate( )
        {
            if (m_PreviewRoad != Entity.Null &&
                EntityManager.Exists(m_PreviewRoad) &&
                m_PreviewCurrent != m_PreviewDesired)
            {
                ClearRoadPreviewComponentsImmediate(m_PreviewRoad);
                SyncVanillaZoneFlagsImmediate(m_PreviewRoad, m_PreviewCurrent);
                ApplyPreviewModeImmediate(m_PreviewRoad, m_PreviewCurrent);
            }

            m_PreviewRoad = Entity.Null;
            m_PreviewCurrent = ZoningMode.Default;
            m_PreviewDesired = ZoningMode.Default;
        }

        private void ClearRoadPreviewComponentsImmediate(Entity roadEntity)
        {
            if (roadEntity == Entity.Null || !EntityManager.Exists(roadEntity))
            {
                return;
            }

            if (EntityManager.HasComponent<ZoningPreviewMode>(roadEntity))
            {
                EntityManager.RemoveComponent<ZoningPreviewMode>(roadEntity);
            }

            if (EntityManager.HasComponent<ZoningRestoreMode>(roadEntity))
            {
                EntityManager.RemoveComponent<ZoningRestoreMode>(roadEntity);
            }
        }

        private void UpdateVanillaRemovalPreview(Entity roadEntity, ZoningMode current, ZoningMode desired)
        {
            // Disabled for now.
            // The direct Highlight attempt did not produce reliable vanilla-red results in ZT,
            // so this branch is focusing on stable desired-depth preview first.
            ClearVanillaRemovalPreviewImmediate();
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
            int2 depths = GetDepthsForMode(mode);

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

                bool isLeftSide = BlockUtils.isBlockOnLeft(block, curve);
                BlockUtils.applyPreviewDepths(isLeftSide, depths, ref validArea, ref block);

                EntityManager.SetComponentData(blockEntity, block);
                EntityManager.SetComponentData(blockEntity, validArea);

                if (!EntityManager.HasComponent<Updated>(blockEntity))
                {
                    EntityManager.AddComponent<Updated>(blockEntity);
                }
            }
        }

        private static int2 GetDepthsForMode(ZoningMode mode)
        {
            return new int2(
                ShouldDisableLeft(mode) ? 0 : 6,
                ShouldDisableRight(mode) ? 0 : 6);
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
