using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Prototype
{
    public static class ECBCoreExt
    {
        public static void AddComponentAndEnable<T>(this EntityCommandBuffer ecb, Entity e, T health) where T : unmanaged, IComponentData, IEnableableComponent
        {
            ecb.AddComponent<T>(e, health);          
            ecb.SetComponentEnabled<T>(e, true);
        }

        public static void SetWorldPosition(this EntityCommandBuffer ecb, Entity e, float3 position)
        {
            ecb.AddComponentAndEnable(e, new SetWorldPositionAndRotationC { 
                 position = position
            });
           
        }

        public static void SetupLifetime(this EntityCommandBuffer ecb, Entity e, LifetimeC lifetime)
        {
            ecb.AddComponent(e, lifetime);
            ecb.AddComponent<CurrentLifetimeC>(e);
        }

        public static void SetCooldown(this EntityCommandBuffer ecb, Entity e, CooldownC cooldown)
        {
            ecb.AddComponentAndEnable(e, cooldown);
        }

        public static void ActivateCooldown(this EntityCommandBuffer ecb, Entity e, float cooldown)
        {
            ecb.AddComponentAndEnable(e, new CooldownC { 
                 duration = cooldown
            });
        }
        public static void EnableEntityFromSystem(this EntityCommandBuffer ecb, Entity e, int skipFrames = 0)
        {
            var disEntity = ecb.CreateEntity();

            ecb.AddComponent(disEntity, new EnableEntityActionC
            {
                entity = e,
                skipFrames = skipFrames
            });
        }

        public static void DisableEntityFromSystem(this EntityCommandBuffer ecb, Entity e, int skipFrames = 0)
        {
            var disEntity = ecb.CreateEntity();

            ecb.AddComponent(disEntity, new DisableEntityActionC
            {
                entity = e,
                skipFrames = skipFrames
            });
        }

        public static void DestroyFromSystem(this EntityCommandBuffer ecb, Entity e)
        {
            ecb.AddComponent(e, new DestroyNextFrameC());
        }

        public static void EnableEntityChildren(this EntityCommandBuffer ecb, EntityManager entityManager, Entity parent)
        {
            ecb.RemoveComponent<Disabled>(parent);
            if (entityManager.HasBuffer<Child>(parent))
                foreach (var child in entityManager.GetBuffer<Child>(parent))
                    EnableEntityChildren(ecb, entityManager, child.Value);
        }

        public static void DisableEntityChildren(this EntityCommandBuffer ecb, EntityManager entityManager, Entity parent)
        {
            ecb.AddComponent<Disabled>(parent);
            if (entityManager.HasBuffer<Child>(parent))
                foreach (var child in entityManager.GetBuffer<Child>(parent))
                    DisableEntityChildren(ecb, entityManager, child.Value);
        }
    }
}