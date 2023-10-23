using Unity.Entities;
using UnityEngine;

namespace Prototype
{
    [DisallowMultipleComponent]
    public class InstantiateAndBindHealthBarAuthoring : MonoBehaviour
    {
        public HealthBarUI prefab;
        public Transform spawnPoint;

        void OnEnable() { }

        class Baker : Baker<InstantiateAndBindHealthBarAuthoring>
        {
            public override void Bake(InstantiateAndBindHealthBarAuthoring authoring)
            {
                if (!authoring.enabled)
                    return;

                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponentObject(entity, new InstantiateAndBindHealthBarC
                {
                    prefab = authoring.prefab,
                    spawnPoint = GetEntity(authoring.spawnPoint, TransformUsageFlags.Dynamic)
                });
            }
        }
    }

    public class InstantiateAndBindHealthBarC : IComponentData
    {
        public Entity spawnPoint;
        public HealthBarUI prefab;
    }

}
