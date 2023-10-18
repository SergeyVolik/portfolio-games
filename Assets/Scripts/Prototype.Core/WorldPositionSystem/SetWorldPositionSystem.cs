using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Prototype
{
    /// <summary>
    /// Set World Space position and rotation System
    /// </summary>
    [UpdateInGroup(typeof(TransformSystemGroup))]
    [UpdateBefore(typeof(LocalToWorldSystem))]
    public partial struct SetWorldPositionSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state) { }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

            var parentLookup = SystemAPI.GetComponentLookup<Parent>(isReadOnly: true);
            var ltwLookup = SystemAPI.GetComponentLookup<LocalToWorld>(isReadOnly: true);


            foreach (var (swp, swpe, e) in SystemAPI.Query<
                RefRO<SetWorldPositionAndRotationC>,
                EnabledRefRW<SetWorldPositionAndRotationC>>()
                .WithAll<LocalTransform, LocalToWorld>()
                .WithEntityAccess())
            {

                swpe.ValueRW = false;

                SetWorldPositionWithParent(e, swp.ValueRO.position, swp.ValueRO.rotation, ecb, parentLookup, ltwLookup);
            }

            foreach (var (swp, link, e) in SystemAPI.Query<RefRO<SetWorldPositionAndRotationC>, RefRO<SetWorldPositionLinkC>>().WithNone<CooldownC>().WithEntityAccess())
            {
                SetWorldPositionWithParent(link.ValueRO.e, swp.ValueRO.position, swp.ValueRO.rotation, ecb, parentLookup, ltwLookup);
                ecb.DestroyEntity(e);
            }
        }

        public static void SetWorldPositionWithParent(Entity e, float3 position, quaternion rotation, EntityCommandBuffer ecb, ComponentLookup<Parent> parentLookup, ComponentLookup<LocalToWorld> ltwLookup)
        {
            if (!rotation.IsValid())
            {
                rotation = quaternion.identity;
            }

            if (parentLookup.HasComponent(e))
            {
                Entity parent = parentLookup.GetRefRO(e).ValueRO.Value;
                float4x4 parentL2W = ltwLookup.GetRefRO(parent).ValueRO.Value;
                float4x4 invParentL2W = math.inverse(parentL2W);
                position = invParentL2W.TransformPoint(position);
                rotation = invParentL2W.TransformRotation(rotation);
            }

            ecb.SetComponent(e, LocalTransform.FromPositionRotation(position, rotation));
        }
        public static void SetWorldPositionWithoutParent(Entity e, float3 position, quaternion rotation, EntityCommandBuffer ecb)
        {
            ecb.SetComponent(e, LocalTransform.FromPositionRotation(position, rotation));
        }
    }
}