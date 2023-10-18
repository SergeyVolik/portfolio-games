using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Prototype
{
    [DisallowMultipleComponent]
    public class LookAtAuthoring : MonoBehaviour
    {
        public Transform target;

        void OnEnable() { }

        class Baker : Baker<LookAtAuthoring>
        {
            public override void Bake(LookAtAuthoring authoring)
            {
                if (!authoring.enabled)
                    return;

                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new LookAtC { 
                     target = GetEntity(authoring.target, TransformUsageFlags.Dynamic)
                });
            }
        }
    }

    public struct LookAtC : IComponentData, IEnableableComponent
    {
        public Entity target;
    }

    public partial struct LookAtSystem : ISystem
    {
        public void OnCreate(ref SystemState state) { }

        public void OnUpdate(ref SystemState state)
        {
            foreach (var (lt, e) in SystemAPI.Query<LookAtC>().WithEntityAccess())
            {
                if (!SystemAPI.HasComponent<LocalToWorld>(lt.target))
                    continue;

                LookAt(ref state, e, SystemAPI.GetComponentRO<LocalToWorld>(lt.target).ValueRO.Position, math.up());
            }
        }

        void LookAt(ref SystemState state, Entity e, float3 target, float3 worldUp)
        {

            LocalTransform transform = SystemAPI.GetComponentRO<LocalTransform>(e).ValueRO;
            float3 selfPos = SystemAPI.GetComponentRO<LocalToWorld>(e).ValueRO.Position;
            target = target - selfPos;
            quaternion rotation = quaternion.LookRotationSafe(target, worldUp);
            SystemAPI.SetComponent(e, transform.WithRotation(rotation));
        }
    }
}