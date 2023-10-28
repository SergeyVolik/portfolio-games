using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SV.BallGame
{
    public partial class ApplyPlayerInputToBoardSystem : SystemBase
    {
        private PlayerControlls input;

        protected override void OnCreate()
        {
            base.OnCreate();
            input = new PlayerControlls();
            input.Enable();
        }
        protected override void OnUpdate()
        {
        
            var inputVec = input.Gameplay.Move.ReadValue<Vector2>();
            

            foreach (var item in SystemAPI.Query<RefRW<ShipInputC>>().WithAll<ReadPlayerInput>())
            {
                item.ValueRW.horizontaMoveInput = inputVec.x;
                item.ValueRW.verticalMoveInput = inputVec.y;
            }
        }
    }
}
