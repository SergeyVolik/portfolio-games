using Prototype.ECS.Runtime;
using Unity.Entities;
using UnityEngine;

namespace Prototype.ECS.Baking
{

    [DisallowMultipleComponent]
    public class SeekerTargetAuthoring : MonoBehaviour
    {
        public Transform targetTransform;
        void OnEnable() { }

        class Baker : Baker<SeekerTargetAuthoring>
        {
            public override void Bake(SeekerTargetAuthoring authoring)
            {
                if (!authoring.enabled)
                    return;

                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new SeekerTargetC
                {
                    target = GetEntity(authoring.targetTransform == null ? authoring.transform : authoring.targetTransform, TransformUsageFlags.Dynamic)
                });
            }
        }
    }
}

namespace Prototype.ECS.Runtime
{
    /// <summary>
    /// Input for <see cref="SeekClosestTargetSystem">
    /// </summary>
    public struct SeekerTargetC : IComponentData
    {
        public Entity target;
    }
}