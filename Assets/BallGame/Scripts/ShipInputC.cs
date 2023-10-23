using Unity.Entities;

namespace SV.BallGame
{

    public struct ShipInputC : IComponentData
    {
        public float horizontaMoveInput;
        public float verticalMoveInput;
    }
}
