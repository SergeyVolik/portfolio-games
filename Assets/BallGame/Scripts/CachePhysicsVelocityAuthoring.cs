using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;

namespace SV.BallGame
{

    [DisallowMultipleComponent]
    public class CachePhysicsVelocityAuthoring : MonoBehaviour
    {

        void OnEnable() { }

        class Baker : Baker<CachePhysicsVelocityAuthoring>
        {
            public override void Bake(CachePhysicsVelocityAuthoring authoring)
            {
                if (!authoring.enabled)
                    return;

                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new CacheVelC { });
            }
        }
    }

    public struct CacheVelC : IComponentData
    {
        public float3 vel;
    }

    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial struct ChashedVelSystem : ISystem
    {
        public void OnCreate(ref SystemState state) { }

        public void OnUpdate(ref SystemState state)
        {
            foreach (var (chashedVel, pv) in SystemAPI.Query<RefRW<CacheVelC>, PhysicsVelocity>())
            {
                chashedVel.ValueRW.vel = pv.Linear;
            }
        }
    }
}