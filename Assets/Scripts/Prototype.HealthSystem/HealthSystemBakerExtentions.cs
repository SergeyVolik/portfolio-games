using Unity.Entities;
using UnityEngine;

namespace Prototype.HealthSystem
{
    public static class HealthSystemBakerExtentions
    {
        public static void SetupDamageableEntity<T>(this Baker<T> baker, Entity e, HealthC healthData)
            where T : MonoBehaviour
        {
            baker.AddComponent(e, healthData);
            baker.AddComponent<HasFullHpT>(e);
            baker.SetComponentEnabled<HasFullHpT>(e, healthData.healthMax == healthData.health);
            baker.AddBuffer<ReceiveDamageB>(e);
            baker.SetComponentEnabled<ReceiveDamageB>(e, false);
            baker.AddComponent<DeadEventC>(e);
            baker.AddComponent<IsDeadTagC>(e);
            baker.SetComponentEnabled<IsDeadTagC>(e, false);
            baker.SetComponentEnabled<DeadEventC>(e, false);
        }

        public static void SetupRegeneration<T>(this Baker<T> baker, Entity entity, HealthRegenC healthData)
            where T : MonoBehaviour
        {
            baker.AddComponent<HealthRegenC>(entity, healthData);
            baker.AddComponent<HealthRegenProcessingC>(entity);
        }

        public static void SetupRegeneration<T>(this Baker<T> baker, Entity entity) where T : MonoBehaviour
        {
            baker.AddComponent<HealthRegenC>(entity);
            baker.AddComponent<HealthRegenProcessingC>(entity);
        }
    }
}