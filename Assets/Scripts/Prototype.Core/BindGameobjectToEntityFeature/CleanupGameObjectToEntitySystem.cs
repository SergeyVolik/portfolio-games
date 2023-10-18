using Unity.Entities;

namespace Prototype
{

    [UpdateInGroup(typeof(BindSystemGroup))]
    public partial class CleanupGameObjectToEntitySystem : SystemBase
    {
        protected override void OnUpdate()
        {

            var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(World.Unmanaged);

            foreach (var (comp, e) in SystemAPI.Query<BindedGameObjectsToEntityC>().WithNone<Simulate>().WithEntityAccess())
            {
                comp.Dispose();
                ecb.RemoveComponent<BindedGameObjectsToEntityC>(e);
            }
        }
    }
 
}