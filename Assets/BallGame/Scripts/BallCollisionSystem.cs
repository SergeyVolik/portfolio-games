using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Stateful;
using Unity.Physics.Systems;
using UnityEngine;

namespace SV.BallGame
{


    public partial struct SystemName : ISystem
    {
        public void OnCreate(ref SystemState state)
        {

        }

        public void OnUpdate(ref SystemState state)
        {

        }
    }

    [UpdateAfter(typeof(FixedStepSimulationSystemGroup))]
    public partial struct BallCollisionSystem : ISystem
    {

        public void OnCreate(ref SystemState state)
        {

        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);


            foreach (var (collisions, vel, ballData, cacheVel, e) in SystemAPI.Query<DynamicBuffer<StatefulCollisionEvent>, RefRW<PhysicsVelocity>, BallDataC, CacheVelC>().WithAll<HasPhysicsEvents, BallDataC>().WithEntityAccess())
            {

                foreach (var collision in collisions)
                {

                    if (collision.State == StatefulEventState.Enter)
                    {

                        var otherColEntity = collision.EntityA == e ? collision.EntityB : collision.EntityA;

                        if (SystemAPI.HasComponent<DestroyableObjC>(otherColEntity))
                        {
                            ecb.DestroyEntity(otherColEntity);
                        }

                        vel.ValueRW.Linear = math.normalize(cacheVel.vel) * ballData.force;

                    }
                }
            }
        }
    } 
}
