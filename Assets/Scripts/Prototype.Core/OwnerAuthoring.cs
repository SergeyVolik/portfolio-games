using Unity.Entities;
using UnityEngine;

namespace Prototype
{
    public class OwnerAuthoring : MonoBehaviour
    {
        public GameObject owner;

        class Baker : Baker<OwnerAuthoring>
        {
            public override void Bake(OwnerAuthoring authoring)
            {
                if (!authoring.enabled)
                    return;

                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity,
                    new OwnerC { value = GetEntity(authoring.owner, TransformUsageFlags.Dynamic) });
            }
        }
    }

    public struct OwnerC : IComponentData
    {
        public Entity value;
    }
}