using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Prototype
{
    [DisallowMultipleComponent]
    public class CameraTargetAuthoring : MonoBehaviour
    {
        public GameObject target;
        class Baker : Baker<CameraTargetAuthoring>
        {
            public override void Bake(CameraTargetAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<CameraTarget>(entity, new CameraTarget
                {
                    entity = GetEntity(authoring.target, TransformUsageFlags.Dynamic)
                });
            }
        }
    }

    public struct CameraTarget : IComponentData
    {
        public Entity entity;
    }

   
}
