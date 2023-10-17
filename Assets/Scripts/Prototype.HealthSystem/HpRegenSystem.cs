using Unity.Entities;
using Unity.Mathematics;

namespace Prototype.HealthSystem
{
    public partial struct HpRegenSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (reg, e) in SystemAPI.Query<HealthRegenC>().WithNone<HealthRegenProcessingC>()
                         .WithEntityAccess())
            {
                ecb.AddComponent<HealthRegenProcessingC>(e);
            }

            var delta = SystemAPI.Time.DeltaTime;

            foreach (var (hpReg, healthTemp, healthC, e) in SystemAPI
                         .Query<HealthRegenC, RefRW<HealthRegenProcessingC>, HealthC>()
                         .WithNone<IsDeadTagC, HasFullHpT>().WithEntityAccess())
            {
                var lastDelta = healthTemp.ValueRO.delta;
                lastDelta += delta;

                var regen = lastDelta * hpReg.hpPerSec;
                if (regen < 1)
                {
                    healthTemp.ValueRW.delta = lastDelta;
                    continue;
                }

                lastDelta = (regen - (int)regen) / hpReg.hpPerSec;
                healthTemp.ValueRW.delta = lastDelta;

                var currentHealth = healthC;

                currentHealth.health = math.clamp(currentHealth.health + (int)regen, 0, healthC.healthMax);
                ecb.SetComponent(e, currentHealth);
            }
        }
    }
}