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
        public ParticlePoolSO pool;
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
                    pool = authoring.pool,
                    sfx = authoring.sfx,
                });
            }
        }
    }

    public class GunShotFXDataC : IComponentData
    {
        public ParticlePoolSO pool;
        public AudioSFX sfx;

    }



    //[UpdateAfter(typeof(ProjectileSpawnerExecuteSystem))]
    public partial class PlayFXAfterShotSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            foreach (var (data, worldMat) in SystemAPI.Query<GunShotFXDataC, LocalToWorld>().WithAll<ExecuteProjectileSpawn>().WithChangeFilter<ExecuteProjectileSpawn>())
            {
                data.sfx?.Play();

                if (data.pool)
                {
                    data.pool.Pool.Get(out var particle);

                    particle.Play();
                    particle.transform.position = worldMat.Position;
                }
            }
        }
    }

}
