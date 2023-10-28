using Prototype;
using Prototype.Parallax;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace SV.BallGame
{

    [DisallowMultipleComponent]
    public class BindParallaxToPlayerShip : MonoBehaviour
    {

        void OnEnable() { }

        class Baker : Baker<BindParallaxToPlayerShip>
        {
            public override void Bake(BindParallaxToPlayerShip authoring)
            {
                if (!authoring.enabled)
                    return;

                var entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new BindParallaxToPlayerShipC { });
            }
        }
    }

    public struct BindParallaxToPlayerShipC : IComponentData
    {

    }


    public partial struct BindParallaxToPlayerShipSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CameraTarget>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var targetE = SystemAPI.GetSingletonEntity<CameraTarget>();
            var targetPos = SystemAPI.GetComponent<LocalTransform>(targetE).Position;

            foreach (var (lTrans, e) in SystemAPI.Query<RefRW<LocalTransform>>().WithAll<ParallaxRoot, BindParallaxToPlayerShipC>().WithEntityAccess())
            {
                lTrans.ValueRW.Position = targetPos;

                if (SystemAPI.HasComponent<PhysicsVelocity>(targetE))
                {
                    var vel = SystemAPI.GetComponent<PhysicsVelocity>(targetE);

                    SystemAPI.SetComponent<SetParallaxSpeedCommand>(e, new SetParallaxSpeedCommand
                    {
                        Value = math.length(vel.Linear)
                    });
                    SystemAPI.SetComponentEnabled<SetParallaxSpeedCommand>(e, true);
                    SystemAPI.SetComponent<SetMoveVectorCommand>(e, new SetMoveVectorCommand
                    {
                        Value = new float2(vel.Linear.x, vel.Linear.z)
                    });
                    SystemAPI.SetComponentEnabled<SetMoveVectorCommand>(e, true);

                }
            }
        }
    }

}
