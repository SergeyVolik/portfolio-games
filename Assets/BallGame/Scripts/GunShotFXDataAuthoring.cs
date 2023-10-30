using Prototype;
using Prototype.Audio;
using Prototype.ProjectileSpawner;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace SV.BallGame
{

    [DisallowMultipleComponent]
    public class GunShotFXDataAuthoring : MonoBehaviour
    {
        public ParticleSystem particleEffect;
        public AudioSFX sfx;

        void OnEnable() { }

        class Baker : Baker<GunShotFXDataAuthoring>
        {
            public override void Bake(GunShotFXDataAuthoring authoring)
            {
                if (!authoring.enabled)
                    return;

                var entity = GetEntity(TransformUsageFlags.None);

                AddComponentObject(entity, new GunShotFXDataC
                {
                    particleEffect = GetEntity(authoring.particleEffect, TransformUsageFlags.Dynamic),
                    sfx = authoring.sfx,
                });
            }
        }
    }

    public class GunShotFXDataC : IComponentData
    {
        public Entity particleEffect;
        public AudioSFX sfx;

    }

    public partial class PlayFXAfterShotSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(World.Unmanaged);
            foreach (var data in SystemAPI.Query<GunShotFXDataC>().WithAll<ExecuteProjectileSpawn>().WithChangeFilter<ExecuteProjectileSpawn>())
            {
                if (data.sfx)
                {
                    var playE = ecb.CreateEntity();
                    ecb.AddComponent<PlaySFXCommandC>(playE, new PlaySFXCommandC
                    {
                        sfxSettingGuid = data.sfx.guid
                    });
                }
                if (SystemAPI.HasComponent<PlayC>(data.particleEffect))
                {
                    SystemAPI.SetComponentEnabled<PlayC>(data.particleEffect, true);
                }
            }
        }
    }

}
