using Unity.Entities;
using UnityEngine;

namespace Prototype
{
    public static class CoreECBExtension
    {
        public static void SetComponentAndEnable<T>(this EntityCommandBuffer ecb, Entity e, T data)
            where T : unmanaged, IComponentData, IEnableableComponent
        {
            ecb.SetComponent<T>(e, data);
            ecb.SetComponentEnabled<T>(e, true);
        }     
    }
}