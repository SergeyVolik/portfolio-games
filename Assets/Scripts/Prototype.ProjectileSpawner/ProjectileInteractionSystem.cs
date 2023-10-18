using Prototype.HealthSystem;
using Unity.Entities;
using Unity.Physics.Stateful;
using Unity.Physics.Systems;
using UnityEngine;

namespace Prototype.ProjectileSpawner
{
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateAfter(typeof(StatefulCollisionEventBufferSystem))]
    public partial struct ProjectileInteractionSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            var query = SystemAPI.QueryBuilder().WithAll<ProjectileC, HasPhysicsEvents>().Build();
            state.RequireForUpdate(query);
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (triggers, proj, e) in SystemAPI.Query<DynamicBuffer<StatefulCollisionEvent>, RefRO<ProjectileC>>().WithAll<ProjectileC, HasPhysicsEvents>().WithEntityAccess())
            {
                foreach (var item in triggers)
                {
                    if (item.State == StatefulEventState.Enter)
                    {
                        var otherE = item.EntityA == e ? item.EntityB : item.EntityA;

                        if (SystemAPI.HasBuffer<ReceiveDamageB>(otherE))
                        {
                            Debug.Log($"Add damage {proj.ValueRO.damage}");
                            ecb.AddDamage(otherE, new ReceiveDamageB()
                            {
                                damage = proj.ValueRO.damage,
                                attacker = e
                            });
                            break;
                        }
                    }
                }

            }
        }
    }
}
