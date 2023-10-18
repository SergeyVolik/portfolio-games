using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Prototype
{

    [DisallowMultipleComponent]
    public class PhysicsChildrenResetAuthoring : MonoBehaviour
    {
        void OnEnable() { }

        class Baker : Baker<PhysicsChildrenResetAuthoring>
        {
            public override void Bake(PhysicsChildrenResetAuthoring authoring)
            {
                if (!authoring.enabled)
                    return;

                var entity = GetEntity(TransformUsageFlags.Dynamic);

                var buffer = AddBuffer<PhysicsChildrenResetBuffer>(entity);

                var allTransforms = authoring.GetComponentsInChildren<Transform>(includeInactive: true);

                var rootTransform = authoring.transform;

                foreach (var item in allTransforms)
                {
                    if (item == rootTransform)
                        continue;

                    if (item.GetComponent<Collider>()
                        || item.GetComponent<Rigidbody>())
                    {

                        var currentTrans = item;
                      
                        buffer.Add(new PhysicsChildrenResetBuffer
                        {
                            offset = currentTrans.position - rootTransform.position,
                            rotation = currentTrans.rotation,
                            entity = GetEntity(item.gameObject, TransformUsageFlags.Dynamic)
                        });
                    }

                }
                AddComponent<ExecutePhysicsResetEvent>(entity);
                SetComponentEnabled<ExecutePhysicsResetEvent>(entity, false);

            }
        }
    }
}