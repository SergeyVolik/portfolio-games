using Prototype.HealthSystem;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Stateful;
using Unity.Physics.Systems;
using UnityEngine;

namespace SV.BallGame
{
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateAfter(typeof(StatefulCollisionEventBufferSystem))]
    public partial struct BallCollisionSystem : ISystem
    {

        public void OnCreate(ref SystemState state)
        {
            
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);


            foreach (var (collisions, vel, ballData, cacheVel, e) in SystemAPI.Query<DynamicBuffer<StatefulCollisionEvent>, RefRO<PhysicsVelocity>, BallDataC, CacheVelC>().WithAll<HasPhysicsEvents, BallDataC>().WithEntityAccess())
            {

                foreach (var collision in collisions)
                {

                    if (collision.State == StatefulEventState.Enter)
                    {

                        var otherColEntity = collision.EntityA == e ? collision.EntityB : collision.EntityA;

                        if (SystemAPI.HasBuffer<ReceiveDamageB>(otherColEntity))
                        {                          
                            ecb.AddDamage(otherColEntity, new ReceiveDamageB
                            {
                                damage = ballData.damage,
                                attacker = e,
                            });
                        }

                        //ecb.SetComponent<PhysicsVelocity>(e, new PhysicsVelocity { Linear = math.normalize(vel.ValueRO.Linear) * ballData.force });

                    }
                }
            }
        }
    }
}
