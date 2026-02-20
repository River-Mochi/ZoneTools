// File: Systems/ZoneToolSystem.Highlight.cs
// Purpose: Apply/remove vanilla selection outline (Game.Tools.Highlighted) via queued requests.

namespace ZoningToolkit.Systems
{
    using Game.Tools;
    using Unity.Collections;
    using Unity.Entities;

    internal partial class ZoneToolSystemHighlight : SystemBase
    {
        private NativeHashSet<Entity> m_ToHighlight;
        private NativeHashSet<Entity> m_ToUnhighlight;
        private ComponentLookup<Edge> m_EdgeLookup;
        private ComponentLookup<Highlighted> m_HighlightedLookup;

        public void RequestHighlight(Entity entity, bool value)
        {
            if (entity == Entity.Null)
            {
                return;
            }

            if (value)
            {
                m_ToUnhighlight.Remove(entity);
                m_ToHighlight.Add(entity);
            }
            else
            {
                m_ToHighlight.Remove(entity);
                m_ToUnhighlight.Add(entity);
            }
        }

        protected override void OnUpdate()
        {
            if (m_ToHighlight.IsEmpty && m_ToUnhighlight.IsEmpty)
            {
                return;
            }

            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);

            // Handle highlight/unhighlight logic
        }
    }
}
