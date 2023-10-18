using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Prototype
{
    [DisallowMultipleComponent]
    public class InstantiateGameObjectsAndBindToEntityAuthoring : MonoBehaviour
    {
        
        public GameObject[] prefabs;
        public Transform bindPos;

        void OnEnable() { }

        class Baker : Baker<InstantiateGameObjectsAndBindToEntityAuthoring>
        {
            public override void Bake(InstantiateGameObjectsAndBindToEntityAuthoring authoring)
            {
                if (!authoring.enabled)
                    return;

                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponentObject(entity, new CreatePrefabInstanceAndBind
                {
                    prefabs = authoring.prefabs,
                    bindPos = GetEntity(authoring.bindPos, TransformUsageFlags.Dynamic)
                });
            }
        }
    }

    public class CreatePrefabInstanceAndBind : IComponentData
    {
        public GameObject[] prefabs;
        public Entity bindPos;
    }

    [UpdateInGroup(typeof(BindSystemGroup))]
    public partial class CreatePrefabAndBindSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (item, e) in SystemAPI.Query<CreatePrefabInstanceAndBind>().WithEntityAccess())
            {
                var ltw = SystemAPI.GetComponentRO<LocalToWorld>(item.bindPos);
              

                foreach (var prefab in item.prefabs)
                {
                    var instance = GameObject.Instantiate(prefab);

                    instance.BindToEntity(item.bindPos, EntityManager, ecb);
                    instance.transform.position = ltw.ValueRO.Position;

                }

                ecb.RemoveComponent<CreatePrefabInstanceAndBind>(e);

            }

            ecb.Playback(EntityManager);
            ecb.Dispose();

        }
    }
}

