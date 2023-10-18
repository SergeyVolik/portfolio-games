using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;

namespace Prototype
{
    public class MoveWithEntity : MonoBehaviour
    {
        public GameObject target;

        public bool copyPos = true;
        public bool copyRot;
        public bool copyScale;

        public float speed = 1000;
        
        void OnEnable()
        {
        }

        class Baker : Baker<MoveWithEntity>
        {
            public override void Bake(MoveWithEntity authoring)
            {
                if (!authoring.enabled)
                    return;

                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity,
                    new MoveWithEntityComponent
                    {
                        target = GetEntity(authoring.target, TransformUsageFlags.Dynamic),
                        copyPos = authoring.copyPos,
                        copyRot = authoring.copyRot,
                        copyScale = authoring.copyScale,
                        speed = authoring.speed
                    });
            }
        }
    }

    public struct MoveWithEntityComponent : IComponentData
    {
        public Entity target;
        public bool copyPos;
        public bool copyRot;
        public bool copyScale;

        public float speed;
    }

    [UpdateInGroup(typeof(BindSystemGroup))]
    public partial struct MoveWithEntitySystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var localToWorldLookup = SystemAPI.GetComponentLookup<LocalToWorld>(isReadOnly: true);

            float deltaTime = SystemAPI.Time.DeltaTime;

            var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);
            foreach (var (trans, mwe, e) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<MoveWithEntityComponent>>()
                         .WithEntityAccess())
            {
                if (mwe.ValueRO.target == Entity.Null)
                {
                    ecb.DestroyEntity(e);
                    continue;
                }

                var ltwFrom = localToWorldLookup.GetRefRO(mwe.ValueRO.target);
                var ltFrom = SystemAPI.GetComponentRO<LocalTransform>(mwe.ValueRO.target);

                if (mwe.ValueRO.copyPos)
                {
                    var pos = math.lerp(trans.ValueRW.Position, ltwFrom.ValueRO.Position,
                        deltaTime * mwe.ValueRO.speed);
                    trans.ValueRW.Position = pos;
                }

                if (mwe.ValueRO.copyRot)
                {
                    var rot = ltwFrom.ValueRO.Rotation;
                    trans.ValueRW.Rotation = rot;
                }

                if (mwe.ValueRO.copyScale)
                {
                    var scale = ltFrom.ValueRO.Scale;
                    trans.ValueRW.Scale = scale;
                }
            }
        }
    }
}