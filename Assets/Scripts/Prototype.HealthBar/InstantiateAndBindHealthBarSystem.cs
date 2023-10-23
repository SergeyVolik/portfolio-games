using Prototype.ECS.UI;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Prototype
{

    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class InstantiateAndBindHealthBarSystem : SystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            RequireForUpdate<InstantiateAndBindHealthBarC>();
        }
        protected override void OnUpdate()
        {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (item, e) in SystemAPI.Query<InstantiateAndBindHealthBarC>().WithEntityAccess())
            {
                var instance = GameObject.Instantiate(item.prefab);
                ecb.AddComponent(e, new UILookAtCameraC { value = instance.transform });
                ecb.AddComponent(e, instance);
                instance.gameObject.BindToEntity(item.spawnPoint, EntityManager, ecb);
                instance.gameObject.SetActive(false);
                ecb.RemoveComponent<InstantiateAndBindHealthBarC>(e);
                var ltw = SystemAPI.GetComponentRO<LocalToWorld>(item.spawnPoint);
                instance.transform.position = ltw.ValueRO.Position;
            }

            ecb.Playback(EntityManager);
            ecb.Dispose();
        }
    }
}