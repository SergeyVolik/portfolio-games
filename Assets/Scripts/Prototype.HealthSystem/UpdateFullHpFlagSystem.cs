using Unity.Burst;
using Unity.Entities;

namespace Prototype.HealthSystem
{
    /// <summary>
    /// Update <see cref="HasFullHpT"> after each <see cref="HealthC"> event
    /// </summary>
    public partial struct UpdateFullHpFlagSystem : ISystem
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
            foreach (var (health, e) in SystemAPI.Query<HealthC>().WithChangeFilter<HealthC>().WithEntityAccess())
            {
                if (SystemAPI.HasComponent<HasFullHpT>(e))
                    SystemAPI.SetComponentEnabled<HasFullHpT>(e, health.IsFullHp());
            }
        }
    }
}