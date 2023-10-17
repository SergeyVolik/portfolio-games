using Unity.Entities;
using UnityEngine;

namespace Prototype.HealthSystem
{
    public struct DestroyAfterDeath : IComponentData { }

    public partial struct DestroyAfterDeathSystem : ISystem
    {
        public void OnCreate(ref SystemState state) { }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (c, entity) in SystemAPI.Query<DestroyAfterDeath>().WithAll<DeadEventC>().WithEntityAccess())
            {
                ecb.DestroyEntity(entity);
                Debug.Log("DestroyAfterDeath");

            }
        }
    }
}