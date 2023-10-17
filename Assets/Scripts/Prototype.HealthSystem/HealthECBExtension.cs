using Unity.Entities;
using UnityEngine;

namespace Prototype.HealthSystem
{
    public static class HealthECBExtension
    {
        public static void SetupDamageableEntity<T>(this EntityCommandBuffer ecb, Entity e, HealthC healthData)
            where T : MonoBehaviour
        {
            ecb.AddComponent(e, healthData);

            ecb.AddBuffer<ReceiveDamageB>(e);
            ecb.SetComponentEnabled<ReceiveDamageB>(e, false);

            ecb.AddComponent<DeadEventC>(e);
            ecb.SetComponentEnabled<DeadEventC>(e, false);

            ecb.AddComponent<IsDeadTagC>(e);
            ecb.SetComponentEnabled<IsDeadTagC>(e, false);
        }

        public static void AddDamage(this EntityCommandBuffer ecb, Entity e, ReceiveDamageB damageData)
        {
            ecb.SetComponentEnabled<ReceiveDamageB>(e, true);
            ecb.AppendToBuffer(e, damageData);
        }
    }
}