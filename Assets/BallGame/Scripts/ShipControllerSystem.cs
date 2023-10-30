using Prototype.ECS.Runtime;
using Prototype.ProjectileSpawner;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace SV.BallGame
{
    public partial struct ShipControllerSystem : ISystem
    {
        public void OnCreate(ref SystemState state) { }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
            var delta = SystemAPI.Time.DeltaTime;
            foreach (var (input, boardData, lTrans, guns, e) in SystemAPI.Query<ShipInputC, ShipControllerDataC, RefRW<LocalTransform>, DynamicBuffer<ShipGunsBuff>>().WithEntityAccess())
            {
                var vector = math.normalizesafe(new Unity.Mathematics.float3(input.horizontaMoveInput, 0, input.verticalMoveInput));
                var inputVec = vector * boardData.moveSpeed;
                ecb.SetComponent<PhysicsVelocity>(e, new PhysicsVelocity { Linear = inputVec });

                bool IsEnabled = SystemAPI.IsComponentEnabled<SeekerDataC>(e);

                foreach (var item in guns)
                {
                    SystemAPI.SetComponentEnabled<ProjectileSpawnerC>(item.gun, IsEnabled);
                }

                if (IsEnabled)
                {
                    var seekerData = SystemAPI.GetComponentRO<SeekerDataC>(e);

                    var lookAtRot = LookAt(lTrans.ValueRO.Position, seekerData.ValueRO.targetPos, math.up());
                    lTrans.ValueRW.Rotation = math.slerp(lTrans.ValueRW.Rotation, lookAtRot, delta * boardData.rotationSpeed);
                }
                else
                {
                    if (input.horizontaMoveInput == 0 && input.verticalMoveInput == 0)
                        continue;

                    lTrans.ValueRW.Rotation = math.slerp(lTrans.ValueRW.Rotation, quaternion.LookRotationSafe(vector, math.up()), delta * boardData.rotationSpeed);
                }
            }
        }

        quaternion LookAt(float3 selfPosition, float3 targetPosition, float3 worldUp)
        {  
            float3 selfPos = selfPosition;
            targetPosition = targetPosition - selfPos;
            return quaternion.LookRotationSafe(targetPosition, worldUp);
        }
    }
}
