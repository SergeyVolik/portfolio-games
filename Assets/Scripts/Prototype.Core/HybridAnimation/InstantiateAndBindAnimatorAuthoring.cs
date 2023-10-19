using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Prototype
{
    [DisallowMultipleComponent]
    public class InstantiateAndBindAnimatorAuthoring : MonoBehaviour
    {
        public GameObject animatedModel;

        class Baker : Baker<InstantiateAndBindAnimatorAuthoring>
        {
            public override void Bake(InstantiateAndBindAnimatorAuthoring authoring)
            {
                if (!authoring.enabled)
                    return;

                if (authoring.animatedModel == null)
                {
                    Debug.LogError($"InstantiateAndBindAnimatorAuthoring animatedModel is null. {authoring.gameObject.name}");
                    return;
                }
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                var hasAnimator = authoring.animatedModel.TryGetComponent<Animator>(out var animator);

                if (hasAnimator == false)
                {
                    Debug.LogError($"InstantiateAndBindAnimatorAuthoring animatedModel doesn't have Animator. {authoring.gameObject.name}");
                    return;
                }

                AddComponentObject(entity, new InstantiateAnimatorCommandC
                {
                    prefab = animator
                });
            }
        }
    }

    /// <summary>
    /// Input for <see cref="InstantiateAnimatorAndBindSystem">
    /// </summary>
    public class InstantiateAnimatorCommandC : IComponentData
    {
        public Animator prefab;
    }

    /// <summary>
    /// Instantiate Unity MonoBehaviour animator and bind to an entity
    /// </summary>
    [UpdateInGroup(typeof(BindSystemGroup))]
    public partial class InstantiateAnimatorAndBindSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(World.Unmanaged);

            foreach (var (item, ltw, e) in SystemAPI.Query<InstantiateAnimatorCommandC, RefRO<LocalToWorld>>().WithEntityAccess())
            {
                var animator = Object.Instantiate(item.prefab);

                animator.transform.position = ltw.ValueRO.Position;
                
                animator.gameObject.BindToEntity(e, EntityManager, ecb);

                ecb.RemoveComponent<InstantiateAnimatorCommandC>(e);
                ecb.AddComponent(e, animator);
            }
        }
    }
}