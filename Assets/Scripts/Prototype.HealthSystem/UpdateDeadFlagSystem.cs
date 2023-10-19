using DOTS.Dispatcher.Runtime;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace Prototype.HealthSystem
{
    /// <summary>
    /// Update <see cref="IsDeadTagC"> after each <see cref="HealthC"> changes
    /// </summary>
    [UpdateInGroup(typeof(HealthSystemGroup))]
    [UpdateAfter(typeof(ApplyDamageSystem))]
    public partial struct UpdateDeadFlagSystem : ISystem
    {
        private EntityQuery query;

        public void OnCreate(ref SystemState state)
        {
            query = SystemAPI.QueryBuilder().WithAll<HealthC>().Build();
            query.AddChangedVersionFilter(typeof(HealthC));
            state.RequireForUpdate(query);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            var eventCB = SystemAPI.GetSingleton<DispatcherSystem.Singleton>()
              .CreateEventBuffer(state.WorldUnmanaged);

            foreach (var (health, e) in SystemAPI.Query<HealthC>().WithChangeFilter<HealthC>().WithNone<IsDeadTagC>()
                         .WithEntityAccess())
            {
                if (!SystemAPI.HasComponent<IsDeadTagC>(e))
                    continue;

                if (health.health > 0)
                {
                    continue;
                }

                Debug.Log("Death Event");
                ecb.SetComponentEnabled<IsDeadTagC>(e, true);
                eventCB.PostEvent<DeadEventC>(e);
            }
        }
    }
}