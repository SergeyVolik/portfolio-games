using Unity.Entities;
using UnityEngine;

namespace SV.BallGame
{
    public partial class ApplyPlayerInputToBoardSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float horizontaMoveInput = 0;
            bool spawnBall = false;

            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                horizontaMoveInput = -1f;
            }
            else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                horizontaMoveInput = 1f;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                spawnBall = true;
            }

            foreach (var item in SystemAPI.Query<RefRW<BoardInputC>>().WithAll<ReadPlayerInput>())
            {
                item.ValueRW.spawnBall = spawnBall;
                item.ValueRW.horizontaMoveInput = horizontaMoveInput;
            }
        }
    }
}
