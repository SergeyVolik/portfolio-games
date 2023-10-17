using Unity.Entities;
using UnityEngine;

namespace Unity.Physics.Stateful
{
    public struct HasPhysicsEvents : IComponentData, IEnableableComponent { }

    [DisallowMultipleComponent]
    public class HasPhysicsEventsAuthoring : MonoBehaviour
    {

        void OnEnable() { }

        class Baker : Baker<HasPhysicsEventsAuthoring>
        {
            public override void Bake(HasPhysicsEventsAuthoring authoring)
            {
                if (!authoring.enabled)
                    return;

                var entity = GetEntity(TransformUsageFlags.None);

                AddComponent<HasPhysicsEvents>(entity);
                SetComponentEnabled<HasPhysicsEvents>(entity, false);
            }
        }
    }
}
