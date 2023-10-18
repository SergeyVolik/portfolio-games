using Unity.Entities;

namespace Prototype
{
    public struct DestroyNextFrameC : IComponentData
    {
        public int frameCount;
    }

    public partial struct DelayedDestroySystem : ISystem
    {
        const int FRAMES_WAIT_TO_DESTROY = 1;
        
        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (destroyData, entity) in SystemAPI.Query<RefRW<DestroyNextFrameC>>().WithEntityAccess())
            {
                destroyData.ValueRW.frameCount++;
                if (destroyData.ValueRO.frameCount > FRAMES_WAIT_TO_DESTROY)
                {
                    ecb.DestroyEntity(entity);
                }
            }
        }
    }
}