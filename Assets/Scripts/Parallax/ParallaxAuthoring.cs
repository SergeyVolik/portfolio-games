using Sirenix.OdinInspector;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Prototype.Parallax
{
    public enum OffsetType
    {
        None,
        SpriteRenderer,
        Custom
    }

    [System.Serializable]
    public class ParallaxObjData
    {
        public OffsetType offsetType;
        public GameObject obj;
        public float parallaxValue;
        public bool enable = true;

        [ShowIf("@this.offsetType == OffsetType.Custom")]
        public float parallaxDistanceToTeleport;
        [ShowIf("@this.offsetType == OffsetType.Custom")]
        public float teleportOffset;
    }

    [DisallowMultipleComponent]
    public class ParallaxAuthoring : MonoBehaviour
    {

        public float speed;
        public Vector2 moveVector;

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

                    float teleportDistance = item.parallaxDistanceToTeleport;
                    float teleportOffset = item.teleportOffset;

                    if (item.offsetType == OffsetType.SpriteRenderer)
                    {
                        var sprite = item.obj.GetComponent<SpriteRenderer>();

                        if (sprite != null)
                        {
                            teleportOffset = sprite.sprite.texture.height / sprite.sprite.pixelsPerUnit * sprite.transform.localScale.x;
                            teleportDistance = teleportOffset;
                        }
                        else
                        {
                            Debug.LogError("SpriteRenderer doesn't exist!");
                        }
                    }



                    buffer.Add(new TempParallaxObjectsBuff
                    {
                        entity = parallaxObjE,
                        parallaxFactor = item.parallaxValue,
                        teleportDistance = teleportDistance,
                        enableParallax = item.enable,
                        teleportOffset = teleportOffset,
                        disableTeleport = item.offsetType == OffsetType.None,
                         moveVector = authoring.moveVector
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

                AddComponent(entity, new SetMoveVectorCommand
                {
                    Value = authoring.moveVector,
                });
            }
        }
    }

    public struct ParallaxRoot : IComponentData
    {
        public float2 moveVector;
    }

    [TemporaryBakingType]
    public struct TempParallaxObjectsBuff : IBufferElementData
    {
        public Entity entity;
        public float parallaxFactor;
        public float teleportDistance;
        public float teleportOffset;
        public bool disableTeleport;
        public bool enableParallax;
        public float2 moveVector;
    }

    public struct ParallaxObjects : IBufferElementData
    {
        public Entity entity;
    }

    public struct ParallaxObject : IComponentData
    {
        public float3 prevTeleportPos;
    }

    public struct TeleportData : IComponentData
    {
        public float teleportOffset;
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

    public struct SetMoveVectorCommand : IComponentData, IEnableableComponent
    {
        public float2 Value;
    }

    public struct ParallaxSpeed : IComponentData
    {
        public float Value;
    }

    public struct ParallaxMoveVector : IComponentData
    {
        public float3 Value;
    }

    [UpdateInGroup(typeof(ParallaxSystemGroup))]
    public partial struct ParallaxMoveSystem : ISystem
    {
        public void OnCreate(ref SystemState state) { }

        public void OnUpdate(ref SystemState state)
        {
            var dletaTime = SystemAPI.Time.DeltaTime;

            foreach (var (trans, factor, speed, moveVector) in SystemAPI.Query<RefRW<LocalTransform>, ParallaxFactor, ParallaxSpeed, ParallaxMoveVector>())
            {
                trans.ValueRW.Position +=  dletaTime * factor.Value * -speed.Value * math.normalizesafe(moveVector.Value);
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
          
            foreach (var (trans, factor, teleportData, speed) in SystemAPI.Query<RefRW<LocalTransform>, ParallaxObject, TeleportData, ParallaxSpeed>())
            {
                var pos = trans.ValueRO.Position;

                var diffX = pos.x - factor.prevTeleportPos.x;
                var diffY = pos.y - factor.prevTeleportPos.y;
                if (math.abs(diffX) > teleportData.teleportDistance)
                {
                    trans.ValueRW.Position.x -= diffX;                   
                }
             if (math.abs(diffY) > teleportData.teleportDistance)
                {
                    trans.ValueRW.Position.y -= diffY;
                }
            }
        }
    }

}

