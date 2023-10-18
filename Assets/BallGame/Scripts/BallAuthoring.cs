using Unity.Entities;
using UnityEngine;

namespace SV.BallGame
{

    [RequireComponent(typeof(CachePhysicsVelocityAuthoring))]
    [DisallowMultipleComponent]
    public class BallAuthoring : MonoBehaviour
    {
        public float force = 10;
        public int damage = 10;
        void OnEnable() { }

        class Baker : Baker<BallAuthoring>
        {
            public override void Bake(BallAuthoring authoring)
            {
                if (!authoring.enabled)
                    return;

                var entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new BallDataC
                {
                    damage = authoring.damage,
                    force = authoring.force
                });
            }
        }
    }

    public struct BallDataC : IComponentData
    {
        public float force;
        public int damage;
    }
}