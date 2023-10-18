using Unity.Burst;
using Unity.Entities;

namespace Prototype
{
    public partial class CooldownSystemGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(CooldownSystemGroup))]
    [UpdateAfter(typeof(CooldownTickSystem))]
    public partial class AfterCooldownSystemGroup : ComponentSystemGroup { }
    public struct CooldownC : IComponentData, IEnableableComponent
    {
        public float t;
        public float duration;
    }

    [UpdateInGroup(typeof(CooldownSystemGroup))]
    public partial struct CooldownTickSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state) { }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

            var delta = SystemAPI.Time.DeltaTime;
            state.Dependency = new CooldownJob
            {
                delta = delta,
                ecb = ecb.AsParallelWriter()
            }.ScheduleParallel(state.Dependency);
        }


        [BurstCompile]
        partial struct CooldownJob : IJobEntity
        {
            public float delta;
            public EntityCommandBuffer.ParallelWriter ecb;

            public void Execute(Entity e, [EntityIndexInQuery] int entityIndexInQuery, ref CooldownC cooldownC)
            {
                cooldownC.t += delta;

                if (cooldownC.t > cooldownC.duration)
                {
                    ecb.SetComponentEnabled<CooldownC>(entityIndexInQuery, e, false);
                }
            }
        }
    }
}