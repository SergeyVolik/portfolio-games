using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Physics.Stateful;
using Prototype.HealthSystem;
using Unity.Physics.Systems;

namespace Prototype
{

    [DisallowMultipleComponent]
    public class ProjectileAuthoring : MonoBehaviour
    {
        public int damage;
        
        void OnEnable() { }

        class Baker : Baker<ProjectileAuthoring>
        {
            public override void Bake(ProjectileAuthoring authoring)
            {
                if (!authoring.enabled)
                    return;

                var entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new ProjectileC { 
                     damage = authoring.damage,
                });
            }
        }
    }

    public struct ProjectileC : IComponentData
    {
        public int damage;
    }

}
