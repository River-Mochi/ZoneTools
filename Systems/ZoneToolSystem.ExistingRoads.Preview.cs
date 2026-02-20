// File: Systems/ZoneToolSystem.ExistingRoads.Preview.cs
// Purpose: Handle zoning previews (hover + select) for the Existing Roads tool.

namespace ZoningToolkit.Systems
{
    using Unity.Collections;
    using Unity.Entities;
    using ZoningToolkit.Components;

    internal sealed partial class ZoneToolSystemPreview : SystemBase
    {
        private NativeParallelHashMap<Entity, PreviewBackup> m_PreviewBackups;
        private int m_PreviewBackupCount;
        private bool m_PreviewDirty;
        private ZoningMode m_LastPreviewMode;

        public void UpdatePreview()
        {
            if (!m_PreviewDirty)
            {
                return;
            }

            ZoningMode desired = m_UISystem.CurrentZoningMode;
            bool hasTargets = (m_SelectedCount > 0) || (m_Hovered != Entity.Null);
            if (!hasTargets)
            {
                RestoreAndClearPreview();
                m_LastPreviewMode = desired;
                m_PreviewDirty = false;
                return;
            }

            RestoreAndClearPreview();
            EntityCommandBuffer ecb = m_ToolOutputBarrier.CreateCommandBuffer();

            foreach (Entity road in m_Selected)
            {
                ApplyPreviewForRoad(ecb, road, desired);
            }

            if (m_Hovered != Entity.Null && (!m_Selected.IsCreated || !m_Selected.Contains(m_Hovered)))
            {
                ApplyPreviewForRoad(ecb, m_Hovered, desired);
            }

            m_LastPreviewMode = desired;
            m_PreviewDirty = false;
        }

        public void MarkPreviewDirty()
        {
            m_PreviewDirty = true;
        }

        public void DiscardPreviewBackups()
        {
            m_PreviewBackups.Clear();
            m_PreviewBackupCount = 0;
        }

        public void RestoreAndClearPreview()
        {
            if (!m_PreviewBackups.IsCreated || m_PreviewBackupCount == 0)
            {
                return;
            }

            EntityCommandBuffer ecb = m_ToolOutputBarrier.CreateCommandBuffer();

            foreach (var kv in m_PreviewBackups)
            {
                Entity blockEntity = kv.Key;
                if (blockEntity == Entity.Null || !EntityManager.Exists(blockEntity))
                {
                    continue;
                }

                PreviewBackup backup = kv.Value;
                ecb.SetComponent(blockEntity, backup.ValidArea);
                ecb.SetComponent(blockEntity, backup.Block);
            }

            m_PreviewBackups.Clear();
            m_PreviewBackupCount = 0;
        }

        private void ApplyPreviewForRoad(EntityCommandBuffer ecb, Entity roadEntity, ZoningMode desiredMode)
        {
            // Apply preview edits, backup original values
        }
    }
}
