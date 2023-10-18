using Unity.Entities;
using Unity.Mathematics;

namespace Prototype
{
    public static class SetWorldPositionExt
    {
        public static void SetWorldPosition(this EntityCommandBuffer ecb, Entity e, float3 position, float delay = 0f)
        {
            SetWorldPositionAndRotation(ecb, e, position, quaternion.identity, delay);

        }

        public static void SetWorldPositionAndRotation(this EntityCommandBuffer ecb, Entity e, float3 position, quaternion rotation, float delay = 0f)
        {
            if (delay != 0)
            {
                var setPosCOmmand = ecb.CreateEntity();

                ecb.AddComponent(setPosCOmmand, new SetWorldPositionAndRotationC
                {
                    position = position
                });

                ecb.AddComponent(setPosCOmmand, new SetWorldPositionLinkC
                {
                    e = e
                });



                ecb.AddComponentAndEnable(setPosCOmmand, new CooldownC
                {
                    duration = 0.1f
                });
            }
            else
            {

                ecb.AddComponent(e, new SetWorldPositionAndRotationC
                {
                    position = position,
                    rotation = rotation
                });

                ecb.SetComponentEnabled<SetWorldPositionAndRotationC>(e, true);
            }

        }
    }
}