using DOTS.Dispatcher.Runtime;
using Prototype.HealthSystem;
using Unity.Collections;
using Unity.Entities;


namespace Prototype
{
    public partial struct CP5_ApplyDamageSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            var eventCB = SystemAPI.GetSingleton<DispatcherSystem.Singleton>()
              .CreateEventBuffer(state.WorldUnmanaged);

            foreach (var (receiveDamageBuffer, entity) in SystemAPI
                         .Query<DynamicBuffer<ReceiveDamageB>>().WithEntityAccess())
            {
                if (!SystemAPI.HasComponent<HealthC>(entity))
                    continue;

                var healthData = SystemAPI.GetComponentRO<HealthC>(entity);
                var health = healthData.ValueRO.health;

                // filter damages that greater then health
                var receiveDamageCalculatedBuffer = new NativeList<ReceiveDamageB>(Allocator.Temp);

                foreach (var damage in receiveDamageBuffer)
                {
                    if (damage.damage > health)
                    {
                        var damageCalculated = health;
                        receiveDamageCalculatedBuffer.Add(new ReceiveDamageB()
                        {
                            attacker = damage.attacker, damage = damageCalculated
                        });

                        health -= damageCalculated;
                    }
                    else
                    {
                        receiveDamageCalculatedBuffer.Add(damage);

                        health -= damage.damage;
                    }
                }

                receiveDamageBuffer.CopyFrom(receiveDamageCalculatedBuffer.AsArray());

                eventCB.PostEvent(entity, new HealthC { health = health, healthMax = healthData.ValueRO.healthMax });

            }
        }
    }
}