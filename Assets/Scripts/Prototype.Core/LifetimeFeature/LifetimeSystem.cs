using Unity.Burst;
using Unity.Entities;

namespace Prototype
{
    public partial struct LifetimeSystem : ISystem
    {
        [BurstCompile]
        public void OnCreated(ref SystemState state)
        {
            state.RequireForUpdate<LifetimeC>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            var delta = SystemAPI.Time.DeltaTime;

            foreach (var (lf, clf, e) in SystemAPI.Query<RefRO<LifetimeC>, RefRW<CurrentLifetimeC>>()
                         .WithEntityAccess())
            {
                clf.ValueRW.value += delta;

                if (clf.ValueRO.value > lf.ValueRO.value)
                {
                    ecb.DestroyEntity(e);
                }
            }

            foreach (var (l, e) in SystemAPI.Query<RefRO<LifetimeC>>().WithNone<CurrentLifetimeC>().WithEntityAccess())
            {
                ecb.AddComponent<CurrentLifetimeC>(e);
            }
        }
    }
}