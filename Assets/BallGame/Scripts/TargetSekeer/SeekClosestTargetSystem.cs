using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Prototype.ECS.Runtime
{

    /// <summary>
    /// Seek closest target to SeekerC component owner
    /// </summary>
    public partial struct SeekClosestTargetSystem : ISystem
    {
        private EntityQuery query;

        public partial struct SeekClosestTargetJob : IJobEntity
        {
            [ReadOnly]
            public NativeArray<Entity> targetEntities;

            [ReadOnly]
            public NativeArray<SeekerTargetC> targets;

            public EntityCommandBuffer ecb;

            [ReadOnly]
            public ComponentLookup<LocalToWorld> ltwLookup;

            [BurstCompile]
            public void Execute(Entity e, in LocalToWorld worldPosC, in SeekerC seekerC)
            {
                float minDistance = seekerC.maxSeekDistance;

                var seekerPos = worldPosC.Position;

                SeekerDataC data = default;

                bool hasTarget = false;

                for (int i = 0; i < targets.Length; i++)
                {
                  
                    var item = targets[i];                  

                    var targetPosC = ltwLookup[item.target].Position;

                    var sqrDist = math.distancesq(seekerPos, targetPosC);

                    if (sqrDist < minDistance)
                    {
                        minDistance = sqrDist;
                        data.closestTarget = targetEntities[i];
                        data.targetPos = targetPosC;
                        hasTarget = true;
                    }
                }

                ecb.SetComponentEnabled<SeekerDataC>(e, hasTarget);
                ecb.SetComponent<SeekerDataC>(e, data);
            }

        }

        public void OnCreate(ref SystemState state)
        {
            query = SystemAPI.QueryBuilder().WithAll<SeekerTargetC>().Build();

            state.RequireForUpdate<SeekerC>();
        }

        
        public void OnUpdate(ref SystemState state)
        {
            DrawDebugLine(ref state);

            var targets = query.ToComponentDataArray<SeekerTargetC>(Allocator.TempJob);
            var targetEntities = query.ToEntityArray(Allocator.TempJob);


            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
            var job = new SeekClosestTargetJob()
            {
                ecb = ecb,
                ltwLookup = SystemAPI.GetComponentLookup<LocalToWorld>(isReadOnly: true),
                targetEntities = targetEntities,
                targets = targets

            };

            state.Dependency = job.Schedule(state.Dependency);

            targetEntities.Dispose(state.Dependency);
            targets.Dispose(state.Dependency);
        }

        private void DrawDebugLine(ref SystemState state)
        {
            foreach (var (seekerC, posC) in SystemAPI.Query<SeekerDataC, RefRO<LocalToWorld>>())
            {
                Debug.DrawLine(posC.ValueRO.Position, seekerC.targetPos, Color.green);
            }
        }
    }
}