using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Physics.Stateful;
using Prototype.HealthSystem;

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


    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial struct ProjectileInteractionSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {

        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (triggers, proj, e) in SystemAPI.Query<DynamicBuffer<StatefulTriggerEvent>, RefRO<ProjectileC>>().WithAll<ProjectileC, HasPhysicsEvents>().WithEntityAccess())
            {
                foreach (var item in triggers)
                {
                    if (item.State == StatefulEventState.Enter)
                    {
                        var otherE = item.EntityA == e ? item.EntityB : item.EntityA;

                        if (SystemAPI.HasBuffer<ReceiveDamageB>(otherE))
                        {
                            ecb.DestroyEntity(e);

                            ecb.AppendToBuffer(otherE, new ReceiveDamageB()
                            {
                                damage = proj.ValueRO.damage,
                                attacker = e
                            });

                            ecb.SetComponentEnabled<ReceiveDamageB>(otherE, true);
                            Debug.Log("ProjectileInteractionSystem");
                            break;
                        }
                    }
                }

            }
        }
    }

}
