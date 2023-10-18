using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Prototype
{
    /// <summary>
    /// 
    /// </summary>
    public class BindedGameObjectsToEntityC : ICleanupComponentData, IDisposable
    {
        public Entity entity;
        public List<GameObject> instances;

        public void Dispose()
        {
            foreach (var instance in instances)
            {
                if (instance)
                {
                    //PrototypeDebug.Log($"Destory Binded Object {instance.name}");
                    GameObject.Destroy(instance);
                }
            }
        }
    }

}