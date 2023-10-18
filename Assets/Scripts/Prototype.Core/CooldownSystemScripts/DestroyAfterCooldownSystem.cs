using Unity.Entities;

namespace Prototype
{
    public struct DestroyAfterCooldownActionC : IBufferElementData
    {
        public Entity entity;
    }

    [UpdateInGroup(typeof(AfterCooldownSystemGroup))]
    public partial struct DestroyAfterCooldownSystem : ISystem
    {
        public void OnCreate(ref SystemState state) { }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (items, e) in SystemAPI.Query<DynamicBuffer<DestroyAfterCooldownActionC>>().WithNone<CooldownC>().WithEntityAccess())
            {
                foreach (var item in items)
                {
                    ecb.AddComponent<DestroyNextFrameC>(item.entity);
                }

                ecb.AddComponent<DestroyNextFrameC>(e);
            }
        }
    }
}