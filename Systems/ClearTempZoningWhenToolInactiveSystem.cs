// File: Systems/ClearTempZoningWhenToolInactiveSystem.cs
// Purpose: Ensure TempZoning never sticks around after the tool is deactivated.

namespace ZoningToolkit.Systems
{
    using Game;
    using Game.Common;
    using Game.Tools;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Jobs;
    using ZoningToolkit.Components;

    public partial class ClearTempZoningWhenToolInactiveSystem : GameSystemBase
    {
        private ToolSystem m_ToolSystem = null!;
        private ToolOutputBarrier m_ToolOutputBarrier = null!;
        private ZoneToolSystemExistingRoads m_ExistingRoadsTool = null!;

        private EntityQuery m_TempQuery;

        protected override void OnCreate()
        {
            base.OnCreate();

            m_ToolSystem = World.GetOrCreateSystemManaged<ToolSystem>();
            m_ToolOutputBarrier = World.GetOrCreateSystemManaged<ToolOutputBarrier>();
            m_ExistingRoadsTool = World.GetOrCreateSystemManaged<ZoneToolSystemExistingRoads>();

            m_TempQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<TempZoning>()
                .Build(this);
        }

        protected override void OnUpdate()
        {
            if (m_TempQuery.IsEmptyIgnoreFilter)
                return;

            // If our tool is active, let it manage TempZoning normally.
            if (m_ToolSystem.activeTool == m_ExistingRoadsTool)
                return;

            NativeArray<Entity> entities = m_TempQuery.ToEntityArray(Allocator.TempJob);

            JobHandle job = new RemoveTempZoningJob
            {
                Entities = entities.AsReadOnly(),
                ECB = m_ToolOutputBarrier.CreateCommandBuffer().AsParallelWriter(),
            }.Schedule(entities.Length, 64, Dependency);

            entities.Dispose(job);

            Dependency = job;
            m_ToolOutputBarrier.AddJobHandleForProducer(Dependency);
        }

        private struct RemoveTempZoningJob : IJobParallelFor
        {
            public NativeArray<Entity>.ReadOnly Entities;
            public EntityCommandBuffer.ParallelWriter ECB;

            public void Execute(int index)
            {
                Entity e = Entities[index];
                if (e == Entity.Null)
                    return;

                ECB.RemoveComponent<TempZoning>(index, e);
                ECB.AddComponent<Updated>(index, e);
            }
        }
    }
}
