using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace Prototype
{
    public class EntityLifetimeAuthoring : MonoBehaviour
    {
        public float lifetime;

        class Baker : Baker<EntityLifetimeAuthoring>
        {
            public override void Bake(EntityLifetimeAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new LifetimeC { value = authoring.lifetime });
            }
        }
    }
}