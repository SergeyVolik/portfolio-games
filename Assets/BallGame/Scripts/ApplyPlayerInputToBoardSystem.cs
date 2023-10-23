using Unity.Entities;
using UnityEngine;

namespace SV.BallGame
{
    public partial class ApplyPlayerInputToBoardSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float horizontaMoveInput = 0;
            float verticalMoveInput = 0;
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                horizontaMoveInput = -1f;
            }
            else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                horizontaMoveInput = 1f;
            }

            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            {
                verticalMoveInput = 1f;
            }
            else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            {
                verticalMoveInput = -1f;
            }

            foreach (var item in SystemAPI.Query<RefRW<ShipInputC>>().WithAll<ReadPlayerInput>())
            {
                item.ValueRW.horizontaMoveInput = horizontaMoveInput;
                item.ValueRW.verticalMoveInput = verticalMoveInput;
            }
        }
    }
}
