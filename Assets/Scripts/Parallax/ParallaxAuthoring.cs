using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Prototype.Parallax
{
    public enum OffsetType
    {
        None,
        SpriteBased
    }
    public enum MoveMode
    {
        Horizontal,
        Vertical,
    }

    [System.Serializable]
    public class ParallaxObjData
    {
        public OffsetType offsetType;
        public GameObject obj;
        public float parallaxValue;
        public bool enable = true;
        public float parallaxDistanceToTeleport;
        public float3 teleportOffset;
        public bool disableTeleport;
    }

    [DisallowMultipleComponent]
    public class ParallaxAuthoring : MonoBehaviour
    {
        public float speed;
        public Vector3 moveVector;
        public ParallaxObjData[] paralaxObjects;

        void OnEnable() { }

        class Baker : Baker<ParallaxAuthoring>
        {
            public override void Bake(ParallaxAuthoring authoring)
            {
                if (!authoring.enabled)
                    return;

                var entity = GetEntity(TransformUsageFlags.Dynamic);
                var buffer = AddBuffer<TempParallaxObjectsBuff>(entity);
                var bufferObjects = AddBuffer<ParallaxObjects>(entity);

                foreach (var item in authoring.paralaxObjects)
                {
                    if (!item.enable)
                        continue;

                    var parallaxObjE = GetEntity(item.obj, TransformUsageFlags.Dynamic);

                    if (parallaxObjE == Entity.Null)
                        continue;

                    buffer.Add(new TempParallaxObjectsBuff
                    {
                        entity = parallaxObjE,
                        parallaxFactor = item.parallaxValue,
                        moveVector = authoring.moveVector,
                        teleportDistance = item.parallaxDistanceToTeleport,
                        enableParallax = item.enable,
                        teleportOffset = item.teleportOffset,
                    });

                    bufferObjects.Add(new ParallaxObjects
                    {
                        entity = parallaxObjE,
                    });

                }
                AddComponent(entity, new ParallaxRoot { });

                AddComponent(entity, new SetParallaxSpeedCommand
                {
                    Value = authoring.speed,
                });
            }
        }
    }

    public struct ParallaxRoot : IComponentData
    {

    }

    [TemporaryBakingType]
    public struct TempParallaxObjectsBuff : IBufferElementData
    {
        public Entity entity;
        public float parallaxFactor;
        public float teleportDistance;
        public float3 teleportOffset;
        public bool disableTeleport;
        public float3 moveVector;
        public bool enableParallax;
    }

    public struct ParallaxObjects : IBufferElementData
    {
        public Entity entity;
    }

    public struct ParallaxObject : IComponentData
    {
        public float3 startPos;
        public float3 teleportOffset;
        public float3 moveVector;
        public float teleportDistance;
    }

    public struct ParallaxFactor : IComponentData
    {
        public float Value;
    }

    public struct SetParallaxSpeedCommand : IComponentData, IEnableableComponent
    {
        public float Value;
    }

    public struct ParallaxSpeed : IComponentData
    {
        public float Value;
    }

    [UpdateInGroup(typeof(ParallaxSystemGroup))]
    public partial struct ParallaxMoveSystem : ISystem
    {
        public void OnCreate(ref SystemState state) { }

        public void OnUpdate(ref SystemState state)
        {
            var dletaTime = SystemAPI.Time.DeltaTime;

            foreach (var (trans, factor, speed, obj) in SystemAPI.Query<RefRW<LocalTransform>, ParallaxFactor, ParallaxSpeed, RefRO<ParallaxObject>>())
            {
                trans.ValueRW.Position += obj.ValueRO.moveVector * dletaTime * factor.Value * speed.Value;
            }
        }
    }

    [UpdateInGroup(typeof(ParallaxSystemGroup))]
    [UpdateAfter(typeof(ParallaxMoveSystem))]
    public partial struct ParallaxTeleportSystem : ISystem
    {
        public void OnCreate(ref SystemState state) { }

        public void OnUpdate(ref SystemState state)
        {
            var dletaTime = SystemAPI.Time.DeltaTime;

            foreach (var (trans, factor, speed) in SystemAPI.Query<RefRW<LocalTransform>, ParallaxObject, ParallaxSpeed>())
            {
                var pos = trans.ValueRO.Position;
                if (math.distancesq(pos, factor.startPos) > factor.teleportDistance * factor.teleportDistance)
                {
                    trans.ValueRW.Position += factor.teleportOffset * math.sign(speed.Value);
                }
            }
        }
    }

}

