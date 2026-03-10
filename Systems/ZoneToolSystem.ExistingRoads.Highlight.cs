// File: Systems/ZoneToolSystem.ExistingRoads.Highlight.cs
// Purpose: Hover highlight / outline logic for Existing Roads tool.
// Notes:
// - Uses vanilla highlight outline (Highlighted + BatchesUpdated).
// - Provides ECB and immediate cleanup paths.

namespace ZoningToolkit.Systems
{
    using Game.Common;      // Updated, Highlighted, BatchesUpdated
    using Game.Net;         // Edge
    using Game.Tools;
    using Unity.Entities;   // Entity, EntityCommandBuffer
    using ZoningToolkit.Components; // ZoningInfo, ZoningMode

    internal sealed partial class ZoneToolSystemExistingRoads
    {
        // Highlight only when the hovered road would actually change.
        private void UpdateHoverHighlight(EntityCommandBuffer ecb)
        {
            Entity target = Entity.Null;

            if (m_Hovered != Entity.Null && EntityManager.Exists(m_Hovered))
            {
                ZoningMode desired = m_UISystem.CurrentZoningMode;
                ZoningMode current = EntityManager.HasComponent<ZoningInfo>(m_Hovered)
                    ? EntityManager.GetComponentData<ZoningInfo>(m_Hovered).zoningMode
                    : ZoningMode.Default;

                if (current != desired)
                {
                    target = m_Hovered;
                }
            }

            if (target == m_Highlighted)
            {
                return;
            }

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
