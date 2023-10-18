using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Prototype
{
    /// <summary>
    /// Input for <see cref="ResetPhysicObejctLocalPositionSystem">
    /// Store positions and rotations of an entity children. 
    /// </summary>
    public struct PhysicsChildrenResetBuffer : IBufferElementData
    {
        public Entity entity;
        public float3 offset;
        public quaternion rotation;
    }

    /// <summary>
    /// Input for <see cref="ResetPhysicObejctLocalPositionSystem">
    /// </summary>
    public struct ExecutePhysicsResetEvent : IComponentData, IEnableableComponent
    {
        public int skipFrames;
    }

    /// <summary>
    /// Reset Local Positions for unparended physics bodies
    /// </summary>
    [CreateAfter(typeof(TransformSystemGroup))]
    public partial struct ResetPhysicObejctLocalPositionSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ExecutePhysicsResetEvent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (items, resetEn, resetData, ltw) in
                SystemAPI.Query<DynamicBuffer<PhysicsChildrenResetBuffer>,
                EnabledRefRW<ExecutePhysicsResetEvent>,
                RefRW<ExecutePhysicsResetEvent>,
                RefRO<LocalToWorld>>().WithOptions(EntityQueryOptions.IncludeDisabledEntities))
            {

                if (resetData.ValueRW.skipFrames > 0)
                {
                    resetData.ValueRW.skipFrames--;
                    continue;
                }

                resetEn.ValueRW = false;

                foreach (var item in items)
                {

                    if (SystemAPI.HasComponent<LocalTransform>(item.entity))
                    {
                        ecb.SetWorldPositionAndRotation(item.entity, ltw.ValueRO.Position + item.offset, item.rotation);

                        SetWorldPosition(ref state, item.entity, ltw.ValueRO.Position + item.offset, item.rotation);
                    }

                }
            }
        }

        void SetWorldPosition(ref SystemState state, Entity e, float3 position, quaternion rotation)
        {
            if (SystemAPI.HasComponent<Parent>(e))
            {
                Entity parent = SystemAPI.GetComponent<Parent>(e).Value;
                float4x4 parentL2W = SystemAPI.GetComponent<LocalToWorld>(parent).Value;
                float4x4 invParentL2W = math.inverse(parentL2W);
                position = invParentL2W.TransformPoint(position);
                rotation = invParentL2W.TransformRotation(rotation);

            }

            SystemAPI.SetComponent(e, LocalTransform.FromPositionRotation(position, rotation));
        }
    }
}