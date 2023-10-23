using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace SV.BallGame
{
    public partial struct ShipInputSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {

        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (input, boardData, e) in SystemAPI.Query<ShipInputC, BoardC>().WithEntityAccess())
            {
               
                var inputVec = math.normalizesafe(new Unity.Mathematics.float3(input.horizontaMoveInput, 0, input.verticalMoveInput)) * boardData.moveSpeed;
              
                ecb.SetComponent<PhysicsVelocity>(e, new PhysicsVelocity { Linear = inputVec });
            }
        }
    }
}
