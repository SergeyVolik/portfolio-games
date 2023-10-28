using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Prototype.Parallax
{
    [DisallowMultipleComponent]
    public class ParallaxRotationDebugAuthoring : MonoBehaviour
    {
        public float rotationSpeed;
        public float moveSpeedChangeFactor;

        void OnEnable() { }

        class Baker : Baker<ParallaxRotationDebugAuthoring>
        {
            public override void Bake(ParallaxRotationDebugAuthoring authoring)
            {
                if (!authoring.enabled)
                    return;

                var entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new ParallaxRotationDebug
                {
                    moveSpeedChangeFactor = authoring.moveSpeedChangeFactor,
                    rotationSpeed = authoring.rotationSpeed
                });
            }
        }
    }

    public struct ParallaxRotationDebug : IComponentData
    {
        public float rotationSpeed;
        public float moveSpeedChangeFactor;
    }




    public partial struct ParallaxDebugSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {

        }

        public void OnUpdate(ref SystemState state)
        {

            float vertivalInput = 0;
            float horInput = 0;
            if (Input.GetKey(KeyCode.W))
            {
                vertivalInput = 1f;
            }
            if (Input.GetKey(KeyCode.S))
            {
                vertivalInput = -1f;
            }

            if (Input.GetKey(KeyCode.A))
            {
                horInput = -1f;
            }
            if (Input.GetKey(KeyCode.D))
            {
                horInput = 1f;
            }

            var deltaTime = SystemAPI.Time.DeltaTime;
            foreach (var (debugData, e) in SystemAPI.Query<ParallaxRotationDebug>().WithAll<ParallaxRoot>().WithEntityAccess())
            {


                var speedChanged = vertivalInput * deltaTime * debugData.moveSpeedChangeFactor;

                if (speedChanged != 0)
                {
                    var speed = SystemAPI.GetComponentRW<SetParallaxSpeedCommand>(e);
                    speed.ValueRW.Value += vertivalInput * deltaTime * debugData.moveSpeedChangeFactor;
                    SystemAPI.SetComponentEnabled<SetParallaxSpeedCommand>(e, true);
                }

                if (horInput != 0)
                {
                    var qut = quaternion.AxisAngle(math.forward(), deltaTime * math.radians(debugData.rotationSpeed));
                    var moveVec = SystemAPI.GetComponentRW<SetMoveVectorCommand>(e);

                    var vec3 = new float3(moveVec.ValueRW.Value.x, moveVec.ValueRW.Value.y, 0);
                    var vec = math.mul(qut, vec3);
                    moveVec.ValueRW.Value = new float2(vec.x, vec.y);
                    SystemAPI.SetComponentEnabled<SetMoveVectorCommand>(e, true);
                }
            }
        }
    }
}
