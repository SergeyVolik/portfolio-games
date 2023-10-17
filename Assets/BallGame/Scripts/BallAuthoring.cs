using Unity.Entities;
using UnityEngine;

namespace SV.BallGame
{

    [RequireComponent(typeof(CachePhysicsVelocityAuthoring))]
    [DisallowMultipleComponent]
    public class BallAuthoring : MonoBehaviour
    {

        void OnEnable() { }

        class Baker : Baker<BallAuthoring>
        {
            public override void Bake(BallAuthoring authoring)
            {
                if (!authoring.enabled)
                    return;

                var entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new BallDataC { });
            }
        }
    }

    public struct BallDataC : IComponentData
    {
        public float force;
    }
}