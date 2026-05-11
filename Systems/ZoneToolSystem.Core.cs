// File: Systems/ZoneToolSystem.Core.cs
// Core zoning application system (new + updated blocks).
// Applies optional protection rules from Settings:
// - ProtectOccupiedCells (default ON)
// - ProtectZonedCells (default OFF)

namespace ZoningToolkit.Systems
{
    using Game;                    // GameSystemBase, barriers/phases
    using Game.Common;             // Created, Deleted, Updated
    using Game.Net;                // Owner, Curve (road geometry owner)
    using Game.Zones;              // Block, Cell, ValidArea, Applied
    using Unity.Burst.Intrinsics;  // v128 (IJobChunk signature)
    using Unity.Collections;       // NativeArray, NativeParallelHashMap
    using Unity.Entities;          // EntityQuery, ComponentLookup, ECB
    using Unity.Jobs;              // JobHandle, IJob, IJobChunk
    using Unity.Mathematics;       // float2, int2
    using UnityEngine.Scripting;   // Preserve (keep OnUpdate/OnCreate from stripping)
    using ZoningToolkit.Components; // ZoningInfo, ZoningInfoUpdated, ZoningMode, ZoningPreviewMode, ZoningRestoreMode
    using ZoningToolkit.Utils;     // BlockUtils (block sizing helpers)

    public partial class ZoneToolSystemCore : GameSystemBase
    {
        // When vanilla UpgradeToolSystem is driving temp road previews, leave those
        // temp blocks alone so ZT's selected side mode does not bleed into vanilla tools.
        internal bool suppressTempRoadZoning;

        // New blocks: Created/Deleted blocks are observed so initial zone sizing is applied.
        private EntityQuery m_NewBlocksQuery;
        // Updated blocks: blocks tagged with ZoningInfoUpdated are re-applied once.
        private EntityQuery m_UpdateBlocksQuery;
        // Chunk access handles (refreshed each frame in OnUpdate).
        private ComponentTypeHandle<Block> m_BlockTypeHandle;
        private EntityTypeHandle m_EntityTypeHandle;
        private ComponentTypeHandle<ValidArea> m_ValidAreaTypeHandle;
        private ComponentTypeHandle<Deleted> m_DeletedTypeHandle;

        // Exposed as fields because jobs need them frequently.
        public ComponentTypeHandle<Owner> ownerTypeHandle;
        public BufferTypeHandle<Cell> cellBufferTypeHandle;

        // Lookups (random access by Entity). Updated each frame before scheduling jobs.
        public ComponentLookup<Owner> ownerComponentLookup;
        [ReadOnly] protected ComponentLookup<Curve> curveComponentLookup;
        private ComponentLookup<ZoningInfo> zoningInfoComponentLookup;
        private ComponentLookup<ZoningPreviewMode> zoningPreviewComponentLookup;
        private ComponentLookup<ZoningRestoreMode> zoningRestoreComponentLookup;
        private ComponentLookup<Deleted> deletedLookup;
        private ComponentLookup<Game.Tools.Temp> tempLookup;
        private ComponentLookup<Applied> appliedLookup;
        private ComponentLookup<Updated> updatedLookup;
        private ComponentLookup<ZoningInfoUpdated> zoningInfoUpdatedLookup;

#if DEBUG
        private int m_DebugUpdateLogTick;
        private int m_DebugLastUpdateBlockCount;
#endif

        // Barrier providing an ECB that plays back in Modification4B (safe structural changes).
        private ModificationBarrier4B m_ModificationBarrier4B = null!;

        // Current mode selected in UI (default: both sides).
        internal ZoningMode zoningMode;

        protected override void OnCreate( )
        {
#if DEBUG
            Mod.s_Log.Info("Creating ZoneToolSystemCore");
#endif
            base.OnCreate();

            // New blocks (Created or Deleted) with zone block data.
            m_NewBlocksQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new[]
                {
                    ComponentType.ReadWrite<Block>(),
                    ComponentType.ReadWrite<Owner>(),
                    ComponentType.ReadOnly<Cell>(),
                    ComponentType.ReadOnly<ValidArea>()
                },
                Any = new[]
                {
                    ComponentType.ReadOnly<Created>(),
                    ComponentType.ReadOnly<Deleted>()
                }
            });

            // Explicit update pass: blocks tagged by the update-existing-roads tool.
            m_UpdateBlocksQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new[]
                {
                    ComponentType.ReadWrite<Block>(),
                    ComponentType.ReadWrite<Owner>(),
                    ComponentType.ReadOnly<Cell>(),
                    ComponentType.ReadOnly<ValidArea>(),
                    ComponentType.ReadOnly<ZoningInfoUpdated>()
                }
            });

            m_BlockTypeHandle = GetComponentTypeHandle<Block>();
            ownerTypeHandle = GetComponentTypeHandle<Owner>();
            cellBufferTypeHandle = GetBufferTypeHandle<Cell>();
            m_EntityTypeHandle = GetEntityTypeHandle();
            m_ValidAreaTypeHandle = GetComponentTypeHandle<ValidArea>();
            m_DeletedTypeHandle = GetComponentTypeHandle<Deleted>();

            ownerComponentLookup = GetComponentLookup<Owner>();
            curveComponentLookup = GetComponentLookup<Curve>(true);
            zoningInfoComponentLookup = GetComponentLookup<ZoningInfo>();
            zoningPreviewComponentLookup = GetComponentLookup<ZoningPreviewMode>(true);
            zoningRestoreComponentLookup = GetComponentLookup<ZoningRestoreMode>(true);
            deletedLookup = GetComponentLookup<Deleted>();
            tempLookup = GetComponentLookup<Game.Tools.Temp>(true);
            appliedLookup = GetComponentLookup<Applied>();
            updatedLookup = GetComponentLookup<Updated>();
            zoningInfoUpdatedLookup = GetComponentLookup<ZoningInfoUpdated>();

            m_ModificationBarrier4B = World.GetOrCreateSystemManaged<ModificationBarrier4B>();

#if DEBUG
            m_DebugUpdateLogTick = 0;
            m_DebugLastUpdateBlockCount = -1;
#endif

            // System stays idle unless there is work in either query.
            RequireAnyForUpdate(m_NewBlocksQuery, m_UpdateBlocksQuery);

            zoningMode = ZoningMode.Default;
        }

        [Preserve]
        protected override void OnUpdate( )
        {
            // Refresh type handles / lookups for the current frame.
            m_BlockTypeHandle.Update(ref CheckedStateRef);
            ownerComponentLookup.Update(ref CheckedStateRef);
            curveComponentLookup.Update(ref CheckedStateRef);
            zoningInfoComponentLookup.Update(ref CheckedStateRef);
            zoningPreviewComponentLookup.Update(ref CheckedStateRef);
            zoningRestoreComponentLookup.Update(ref CheckedStateRef);
            m_EntityTypeHandle.Update(ref CheckedStateRef);
            m_ValidAreaTypeHandle.Update(ref CheckedStateRef);
            m_DeletedTypeHandle.Update(ref CheckedStateRef);
            ownerTypeHandle.Update(ref CheckedStateRef);
            deletedLookup.Update(ref CheckedStateRef);
            tempLookup.Update(ref CheckedStateRef);
            cellBufferTypeHandle.Update(ref CheckedStateRef);
            appliedLookup.Update(ref CheckedStateRef);
            updatedLookup.Update(ref CheckedStateRef);
            zoningInfoUpdatedLookup.Update(ref CheckedStateRef);

            // All structural changes (add/remove/set components) go through an ECB.
            EntityCommandBuffer ecb = m_ModificationBarrier4B.CreateCommandBuffer();

            bool protectOccupiedCells = Mod.Settings?.ProtectOccupiedCells ?? true;
            bool protectZonedCells = Mod.Settings?.ProtectZonedCells ?? false;

            // Hash maps track deleted curves by endpoint so zoning mode can be inherited
            // across split/replace operations (common during road edits).
            NativeParallelHashMap<float2, Entity> deletedByStart =
                new NativeParallelHashMap<float2, Entity>(32, Allocator.TempJob);
            NativeParallelHashMap<float2, Entity> deletedByEnd =
                new NativeParallelHashMap<float2, Entity>(32, Allocator.TempJob);

            JobHandle deps = Dependency;

            // Collect deleted curves first so new blocks can inherit prior mode when appropriate.
            JobHandle collectDeletedJob = new CollectDeletedCurves
            {
                ownerTypeHandle = ownerTypeHandle,
                deletedTypeHandle = m_DeletedTypeHandle,
                curveLookup = curveComponentLookup,
                deletedLookup = deletedLookup,
                curvesByStartPoint = deletedByStart,
                curvesByEndPoint = deletedByEnd
            }.Schedule(m_NewBlocksQuery, deps);

            deps = JobHandle.CombineDependencies(deps, collectDeletedJob);

            // New blocks: apply sizing + attach/update ZoningInfo on the owning curve entity.
            if (!m_NewBlocksQuery.IsEmptyIgnoreFilter)
            {
                JobHandle job = new UpdateZoneData
                {
                    zoningMode = zoningMode,
                    protectOccupiedCells = protectOccupiedCells,
                    protectZonedCells = protectZonedCells,

                    entityTypeHandle = m_EntityTypeHandle,
                    blockComponentTypeHandle = m_BlockTypeHandle,
                    validAreaComponentTypeHandle = m_ValidAreaTypeHandle,
                    deletedTypeHandle = m_DeletedTypeHandle,
                    bufferTypeHandle = cellBufferTypeHandle,
                    ownerComponentLookup = ownerComponentLookup,
                    curveComponentLookup = curveComponentLookup,
                    zoningInfoComponentLookup = zoningInfoComponentLookup,
                    tempLookup = tempLookup,
                    appliedLookup = appliedLookup,
                    entityCommandBuffer = ecb,
                    suppressTempRoadZoning = suppressTempRoadZoning,
                    entitiesByStartPoint = deletedByStart,
                    entitiesByEndPoint = deletedByEnd
                }.Schedule(m_NewBlocksQuery, deps);

                deps = JobHandle.CombineDependencies(deps, job);
            }

            // Updated blocks: re-apply sizing once, then remove the marker component.
            if (!m_UpdateBlocksQuery.IsEmptyIgnoreFilter)
            {
#if DEBUG
                int updateCount = m_UpdateBlocksQuery.CalculateEntityCount();
                m_DebugUpdateLogTick++;
                if (updateCount != m_DebugLastUpdateBlockCount || (m_DebugUpdateLogTick % 30) == 0)
                {
                    Mod.s_Log.Info($"{Mod.ModTag} Core update pass blocks={updateCount} (preview/apply markers)");
                    m_DebugLastUpdateBlockCount = updateCount;
                }
#endif
                JobHandle job = new UpdateZoningInfoJob
                {
                    zoningMode = zoningMode,
                    protectOccupiedCells = protectOccupiedCells,
                    protectZonedCells = protectZonedCells,

                    entityTypeHandle = m_EntityTypeHandle,
                    blockComponentTypeHandle = m_BlockTypeHandle,
                    validAreaComponentTypeHandle = m_ValidAreaTypeHandle,
                    bufferTypeHandle = cellBufferTypeHandle,
                    ownerComponentLookup = ownerComponentLookup,
                    curveComponentLookup = curveComponentLookup,
                    zoningInfoComponentLookup = zoningInfoComponentLookup,
                    zoningPreviewComponentLookup = zoningPreviewComponentLookup,
                    zoningRestoreComponentLookup = zoningRestoreComponentLookup,
                    zoningInfoUpdateComponentLookup = zoningInfoUpdatedLookup,
                    entityCommandBuffer = ecb,
                    updatedLookup = updatedLookup
                }.Schedule(m_UpdateBlocksQuery, deps);

                deps = JobHandle.CombineDependencies(deps, job);
            }

            // Dispose temp hash maps after all readers complete.
            JobHandle disposeJob = new DisposeHashMaps
            {
                toDispose1 = deletedByStart,
                toDispose2 = deletedByEnd
            }.Schedule(deps);

            Dependency = JobHandle.CombineDependencies(disposeJob, deps);
            m_ModificationBarrier4B.AddJobHandleForProducer(Dependency);
        }

        // Legacy helper: string-based mode selection (kept for compatibility with older UI wiring).
        public void SetZoningMode(string mode)
        {
#if DEBUG
            Mod.s_Log.Info($"Changing zoning mode to {mode}");
#endif
            zoningMode = mode switch
            {
                "Left" => ZoningMode.Left,
                "Right" => ZoningMode.Right,
                "Default" => ZoningMode.Default,
                "None" => ZoningMode.None,
                _ => ZoningMode.Default
            };
        }

        private static ZoningInfo DefaultZI( ) => new ZoningInfo { zoningMode = ZoningMode.Default };

        // Adds ZoningInfo if missing; sets it if already present.
        private static void AddOrSetZoningInfo(
            EntityCommandBuffer ecb,
            ComponentLookup<ZoningInfo> lookup,
            Entity owner,
            ZoningInfo zi)
        {
            if (lookup.HasComponent(owner))
            {
                ecb.SetComponent(owner, zi);
            }
            else
            {
                ecb.AddComponent(owner, zi);
            }
        }

        private struct UpdateZoningInfoJob : IJobChunk
        {
            [ReadOnly] public ZoningMode zoningMode;
            [ReadOnly] public bool protectOccupiedCells;
            [ReadOnly] public bool protectZonedCells;

            [ReadOnly] public EntityTypeHandle entityTypeHandle;
            public ComponentTypeHandle<Block> blockComponentTypeHandle;
            public ComponentTypeHandle<ValidArea> validAreaComponentTypeHandle;
            public BufferTypeHandle<Cell> bufferTypeHandle;

            [ReadOnly] public ComponentLookup<Owner> ownerComponentLookup;
            [ReadOnly] public ComponentLookup<Curve> curveComponentLookup;
            [ReadOnly] public ComponentLookup<ZoningInfo> zoningInfoComponentLookup;
            [ReadOnly] public ComponentLookup<ZoningPreviewMode> zoningPreviewComponentLookup;
            [ReadOnly] public ComponentLookup<ZoningRestoreMode> zoningRestoreComponentLookup;

            public ComponentLookup<ZoningInfoUpdated> zoningInfoUpdateComponentLookup;
            public EntityCommandBuffer entityCommandBuffer;
            public ComponentLookup<Updated> updatedLookup;

            public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
            {
                NativeArray<Block> blocks = chunk.GetNativeArray(ref blockComponentTypeHandle);
                NativeArray<Entity> entities = chunk.GetNativeArray(entityTypeHandle);
                BufferAccessor<Cell> cellBufs = chunk.GetBufferAccessor(ref bufferTypeHandle);
                NativeArray<ValidArea> validAreas = chunk.GetNativeArray(ref validAreaComponentTypeHandle);

                for (int i = 0; i < entities.Length; i++)
                {
                    Entity entity = entities[i];

                    if (!ownerComponentLookup.HasComponent(entity))
                    {
                        continue;
                    }

                    Owner owner = ownerComponentLookup[entity];

                    if (!curveComponentLookup.HasComponent(owner.m_Owner))
                    {
                        entityCommandBuffer.RemoveComponent<ZoningInfoUpdated>(entity);
                        continue;
                    }

                    Curve curve = curveComponentLookup[owner.m_Owner];
                    Block block = blocks[i];
                    DynamicBuffer<Cell> cells = cellBufs[i];
                    ValidArea validArea = validAreas[i];

                    bool isLeftSide = BlockUtils.isBlockOnLeft(block, curve);
                    bool hasPreview = zoningPreviewComponentLookup.TryGetComponent(owner.m_Owner, out ZoningPreviewMode preview);
                    bool hasRestore = zoningRestoreComponentLookup.TryGetComponent(owner.m_Owner, out ZoningRestoreMode restore);
                    bool hasZoningInfo = zoningInfoComponentLookup.TryGetComponent(owner.m_Owner, out ZoningInfo zi);

                    bool applyDepth = false;
                    int targetDepth = block.m_Size.y;

                    if (hasPreview)
                    {
                        targetDepth = isLeftSide ? preview.depths.x : preview.depths.y;
                        applyDepth = true;
                    }
                    else if (hasRestore)
                    {
                        targetDepth = isLeftSide ? restore.depths.x : restore.depths.y;
                        applyDepth = true;
                    }
                    else if (hasZoningInfo)
                    {
                        targetDepth = BlockUtils.getDepthForMode(isLeftSide, zi.zoningMode);
                        applyDepth = true;
                    }

                    if (applyDepth &&
                        !BlockUtils.shouldProtectDepthReduction(
                            targetDepth,
                            ref cells,
                            ref block,
                            ref validArea,
                            protectOccupiedCells,
                            protectZonedCells))
                    {
                        BlockUtils.applyBlockDepth(targetDepth, ref validArea, ref block);
                        entityCommandBuffer.SetComponent(entity, validArea);
                        entityCommandBuffer.SetComponent(entity, block);
                    }

                    if (hasRestore)
                    {
                        entityCommandBuffer.RemoveComponent<ZoningRestoreMode>(owner.m_Owner);
                    }

                    // One-shot tag: remove so the block is not reprocessed every frame.
                    entityCommandBuffer.RemoveComponent<ZoningInfoUpdated>(entity);
                }
            }
        }

        private struct UpdateZoneData : IJobChunk
        {
            [ReadOnly] public ZoningMode zoningMode;
            [ReadOnly] public bool protectOccupiedCells;
            [ReadOnly] public bool protectZonedCells;

            [ReadOnly] public EntityTypeHandle entityTypeHandle;
            public ComponentTypeHandle<Block> blockComponentTypeHandle;
            public ComponentTypeHandle<ValidArea> validAreaComponentTypeHandle;
            public ComponentTypeHandle<Deleted> deletedTypeHandle;
            public BufferTypeHandle<Cell> bufferTypeHandle;

            [ReadOnly] public ComponentLookup<Owner> ownerComponentLookup;
            [ReadOnly] public ComponentLookup<Curve> curveComponentLookup;
            [ReadOnly] public ComponentLookup<ZoningInfo> zoningInfoComponentLookup;
            [ReadOnly] public ComponentLookup<Game.Tools.Temp> tempLookup;
            [ReadOnly] public ComponentLookup<Applied> appliedLookup;

            public EntityCommandBuffer entityCommandBuffer;
            [ReadOnly] public bool suppressTempRoadZoning;
            [ReadOnly] public NativeParallelHashMap<float2, Entity> entitiesByStartPoint;
            [ReadOnly] public NativeParallelHashMap<float2, Entity> entitiesByEndPoint;

            public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
            {
                NativeArray<Block> blocks = chunk.GetNativeArray(ref blockComponentTypeHandle);
                NativeArray<Entity> entities = chunk.GetNativeArray(entityTypeHandle);
                BufferAccessor<Cell> cellBufs = chunk.GetBufferAccessor(ref bufferTypeHandle);
                NativeArray<ValidArea> validAreas = chunk.GetNativeArray(ref validAreaComponentTypeHandle);

                // Deleted blocks are ignored for sizing.
                if (chunk.Has(ref deletedTypeHandle))
                {
                    return;
                }

                for (int i = 0; i < blocks.Length; i++)
                {
                    Entity entity = entities[i];

                    if (!ownerComponentLookup.HasComponent(entity))
                    {
                        continue;
                    }

                    Owner owner = ownerComponentLookup[entity];

                    if (!curveComponentLookup.HasComponent(owner.m_Owner))
                    {
                        continue;
                    }

                    Curve curve = curveComponentLookup[owner.m_Owner];

                    if (suppressTempRoadZoning && tempLookup.HasComponent(owner.m_Owner))
                    {
                        continue;
                    }

                    // Default: use current UI mode for brand new curves.
                    ZoningInfo zi = new ZoningInfo { zoningMode = zoningMode };

                    // Inheritance rules:
                    // - If curve is not yet Applied: read existing ZoningInfo if present.
                    // - If curve is Applied: attempt to infer mode from matching deleted curves (split/replace),
                    //   then fall back to existing mode if present.
                    if (!appliedLookup.HasComponent(owner.m_Owner))
                    {
                        zi = zoningInfoComponentLookup.HasComponent(owner.m_Owner)
                            ? zoningInfoComponentLookup[owner.m_Owner]
                            : DefaultZI();
                    }
                    else
                    {
                        if (entitiesByStartPoint.TryGetValue(curve.m_Bezier.a.xz, out Entity s) &&
                            entitiesByEndPoint.TryGetValue(curve.m_Bezier.d.xz, out Entity e))
                        {
                            if (s == e && zoningInfoComponentLookup.HasComponent(s))
                            {
                                zi = zoningInfoComponentLookup[s];
                            }
                            else if (curveComponentLookup.HasComponent(s) && curveComponentLookup.HasComponent(e))
                            {
                                ZoningInfo sZI = zoningInfoComponentLookup.HasComponent(s)
                                    ? zoningInfoComponentLookup[s]
                                    : DefaultZI();
                                ZoningInfo eZI = zoningInfoComponentLookup.HasComponent(e)
                                    ? zoningInfoComponentLookup[e]
                                    : DefaultZI();

                                zi = sZI.Equals(eZI) ? sZI : DefaultZI();
                            }
                        }
                        else if (entitiesByEndPoint.TryGetValue(curve.m_Bezier.d.xz, out Entity eOnly) &&
                                 zoningInfoComponentLookup.HasComponent(eOnly))
                        {
                            zi = zoningInfoComponentLookup[eOnly];
                        }
                        else if (entitiesByStartPoint.TryGetValue(curve.m_Bezier.a.xz, out Entity sOnly) &&
                                 zoningInfoComponentLookup.HasComponent(sOnly))
                        {
                            zi = zoningInfoComponentLookup[sOnly];
                        }

                        // Strongest precedence: explicit ZoningInfo already on the current owner.
                        if (zoningInfoComponentLookup.HasComponent(owner.m_Owner))
                        {
                            zi = zoningInfoComponentLookup[owner.m_Owner];
                        }
                    }

                    Block block = blocks[i];
                    DynamicBuffer<Cell> cells = cellBufs[i];
                    ValidArea validArea = validAreas[i];

                    bool isLeftSide = BlockUtils.isBlockOnLeft(block, curve);
                    int targetDepth = BlockUtils.getDepthForMode(isLeftSide, zi.zoningMode);

                    if (BlockUtils.shouldProtectDepthReduction(
                            targetDepth,
                            ref cells,
                            ref block,
                            ref validArea,
                            protectOccupiedCells,
                            protectZonedCells))
                    {
                        continue;
                    }

                    BlockUtils.applyBlockDepth(targetDepth, ref validArea, ref block);
                    entityCommandBuffer.SetComponent(entity, validArea);
                    entityCommandBuffer.SetComponent(entity, block);

                    // ZoningInfo is persisted on the owner (curve/edge entity) so future blocks inherit it.
                    AddOrSetZoningInfo(entityCommandBuffer, zoningInfoComponentLookup, owner.m_Owner, zi);
                }
            }
        }

        private struct CollectDeletedCurves : IJobChunk
        {
            public ComponentTypeHandle<Owner> ownerTypeHandle;
            public ComponentTypeHandle<Deleted> deletedTypeHandle;

            [ReadOnly] public ComponentLookup<Curve> curveLookup;
            [ReadOnly] public ComponentLookup<Deleted> deletedLookup;

            public NativeParallelHashMap<float2, Entity> curvesByStartPoint;
            public NativeParallelHashMap<float2, Entity> curvesByEndPoint;

            public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
            {
                NativeArray<Owner> owners = chunk.GetNativeArray(ref ownerTypeHandle);

                if (!chunk.Has(ref deletedTypeHandle))
                {
                    return;
                }

                for (int i = 0; i < owners.Length; i++)
                {
                    Owner owner = owners[i];

                    if (curveLookup.HasComponent(owner.m_Owner))
                    {
                        Curve curve = curveLookup[owner.m_Owner];
                        curvesByStartPoint.TryAdd(curve.m_Bezier.a.xz, owner.m_Owner);
                        curvesByEndPoint.TryAdd(curve.m_Bezier.d.xz, owner.m_Owner);
                    }
                }
            }
        }

        private struct DisposeHashMaps : IJob
        {
            public NativeParallelHashMap<float2, Entity> toDispose1;
            public NativeParallelHashMap<float2, Entity> toDispose2;

            public void Execute( )
            {
                toDispose1.Dispose();
                toDispose2.Dispose();
            }
        }
    }
}
