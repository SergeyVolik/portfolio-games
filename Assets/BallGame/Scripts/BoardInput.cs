using Unity.Entities;

namespace SV.BallGame
{

    public struct BoardInputC : IComponentData
    {
        public float horizontaMoveInput;
        public bool spawnBall;
    }
}
