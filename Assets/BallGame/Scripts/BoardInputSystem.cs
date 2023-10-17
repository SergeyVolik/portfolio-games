using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

namespace SV.BallGame
{
    public partial struct BoardInputSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {

        }

        public void OnUpdate(ref SystemState state)
        {
            var delta = SystemAPI.Time.DeltaTime;

            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (input, boardData, e) in SystemAPI.Query<BoardInputC, BoardC>().WithEntityAccess()) 
            {
                var vel = input.horizontaMoveInput * boardData.moveSpeed;
                ecb.SetComponent<PhysicsVelocity>(e, new PhysicsVelocity { Linear = new Unity.Mathematics.float3(vel, 0, 0) });

                if (input.spawnBall)
                {
                    var ballInstance = ecb.Instantiate(boardData.ballPrefab);

                    var spawnPos = SystemAPI.GetComponentRO<LocalToWorld>(boardData.ballSpawnPoint).ValueRO.Position;
                    ecb.AddComponent<BallDataC>(ballInstance, new BallDataC { 
                         force = boardData.ballSpeed
                    });
                    ecb.SetComponent<LocalTransform>(ballInstance, LocalTransform.FromPosition(spawnPos));
                    ecb.SetComponent<PhysicsVelocity>(ballInstance, new PhysicsVelocity { Linear = new Unity.Mathematics.float3(0, boardData.ballSpeed, 0) });
                }
            }
        }
    }
}
