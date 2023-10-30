using Prototype.ECS.Runtime;
using Sirenix.OdinInspector;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Prototype.ECS.Baking
{

    /// <summary>
    /// Seeker object baker components: (<see cref="SeekerC"> <see cref="SeekerDataC">)
    /// </summary>
    [DisallowMultipleComponent]
    public class SeekerAuthoring : MonoBehaviour
    {
        [InfoBox("@\"Real distance: \" + this.GetRealDistance()")]
        public float maxSeekSqrDistance = 100;
        void OnEnable() { }

        public float GetRealDistance() => math.sqrt(maxSeekSqrDistance);
        class Baker : BakerForEnabledComponent<SeekerAuthoring>
        {
            public override void BakeIfEnabled(SeekerAuthoring authoring)
            {
                

                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<SeekerC>(entity, new SeekerC
                {
                     maxSeekDistance = authoring.maxSeekSqrDistance
                });

                AddComponent(entity, new SeekerDataC
                {

                });

                SetComponentEnabled<SeekerDataC>(entity, false);
            }
        }
    } 
}

namespace Prototype.ECS.Runtime
{
    /// <summary>
    /// Input for <see cref="SeekClosestTargetSystem">
    /// Main seeker component
    /// </summary>
    public struct SeekerC : IComponentData, IEnableableComponent
    {
        public float maxSeekDistance;
    }

    /// <summary>
    /// Input for <see cref="SeekClosestTargetSystem">
    /// Temp seeker data. Enabled if has target
    /// </summary>
    public struct SeekerDataC : IComponentData, IEnableableComponent
    {
        public Entity closestTarget;
        public float3 targetPos;

    }
}
