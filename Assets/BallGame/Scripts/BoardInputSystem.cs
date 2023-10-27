using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace SV.BallGame
{
    public partial struct ShipControllerSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {

        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (input, boardData, e) in SystemAPI.Query<ShipInputC, ShipControllerDataC>().WithEntityAccess())
            {
               
                var inputVec = math.normalizesafe(new Unity.Mathematics.float3(input.horizontaMoveInput, 0, input.verticalMoveInput)) * boardData.moveSpeed;
              
                ecb.SetComponent<PhysicsVelocity>(e, new PhysicsVelocity { Linear = inputVec });
            }
        }
    }

    public partial struct ShipAnimationSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {

        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
            var deltaTIme = SystemAPI.Time.DeltaTime;

            foreach (var (input, animData, e) in SystemAPI.Query<ShipInputC, ShipAnimationDataC>().WithEntityAccess())
            {

                var inputVec = input.horizontaMoveInput;

                var lTrans = SystemAPI.GetComponentRW<LocalTransform>(animData.shipRoot);

               
                var targetRot = quaternion.AxisAngle(math.forward(), math.radians(-inputVec * animData.shipMaxAnimRot));

                lTrans.ValueRW.Rotation = math.slerp(lTrans.ValueRW.Rotation, targetRot, deltaTIme * animData.rotSpeed);
                ecb.SetComponent<PhysicsVelocity>(e, new PhysicsVelocity { Linear = inputVec });
            }
        }
    }
}
