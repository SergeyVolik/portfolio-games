using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace Prototype.Parallax
{
    [WorldSystemFilter(WorldSystemFilterFlags.BakingSystem)]
    [BurstCompile]
    public partial struct BakeParallaxSystem : ISystem
    {
        public void OnCreate(ref SystemState state) { }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var (parallaxArray, speed) in SystemAPI.Query<DynamicBuffer<TempParallaxObjectsBuff>, SetParallaxSpeedCommand>().WithAll<ParallaxRoot>())
            {

                foreach (var parallaxTempData in parallaxArray)
                {
                    if (!parallaxTempData.enableParallax)
                        continue;

                    ecb.AddComponent<ParallaxFactor>(parallaxTempData.entity, new ParallaxFactor
                    {
                        Value = parallaxTempData.parallaxFactor
                    });

                    ecb.AddComponent<ParallaxSpeed>(parallaxTempData.entity, new ParallaxSpeed
                    {
                        Value = speed.Value
                    });

                    ecb.AddComponent<ParallaxMoveVector>(parallaxTempData.entity, new ParallaxMoveVector
                    {
                        Value = new Unity.Mathematics.float3(parallaxTempData.moveVector.x, parallaxTempData.moveVector.y, 0) 
                    });

                    ecb.AddComponent<ParallaxObject>(parallaxTempData.entity, new ParallaxObject
                    {
                        prevTeleportPos = SystemAPI.GetComponent<LocalTransform>(parallaxTempData.entity).Position,
                       
                    });

                    if (!parallaxTempData.disableTeleport)
                    {
                        ecb.AddComponent<TeleportData>(parallaxTempData.entity, new TeleportData
                        {
                            teleportOffset = parallaxTempData.teleportOffset,
                            teleportDistance = parallaxTempData.teleportDistance
                        });
                    }
                }
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
