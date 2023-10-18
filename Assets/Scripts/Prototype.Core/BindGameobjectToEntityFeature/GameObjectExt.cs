using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Prototype
{
    public static class GameObjectExt
    {

        public static void BindToEntity(this GameObject go, Entity e, EntityManager manager, EntityCommandBuffer ecb)
        {
            var isEntityDisabled = manager.HasComponent<Disabled>(e);

            List<GameObject> list = null;
            if (manager.HasComponent<BindedGameObjectsToEntityC>(e))
            {
                list = manager.GetComponentObject<BindedGameObjectsToEntityC>(e).instances;
            }
            else
            {

                list = new List<GameObject>();
                ecb.AddComponent(e, new BindedGameObjectsToEntityC
                {
                    instances = list,
                    entity = e,
                });

            }

            go.SetActive(!isEntityDisabled);

            list.Add(go);
        }

    }
}