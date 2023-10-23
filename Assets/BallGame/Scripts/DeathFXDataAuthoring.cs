using Prototype;
using Prototype.Audio;
using Prototype.HealthSystem;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace SV.BallGame
{

    [DisallowMultipleComponent]
    public class DeathFXDataAuthoring : MonoBehaviour
    {
        public ParticlePoolSO pool;
        public AudioSFX sfx;

        void OnEnable() { }

        class Baker : Baker<DeathFXDataAuthoring>
        {
            public override void Bake(DeathFXDataAuthoring authoring)
            {
                if (!authoring.enabled)
                    return;

                var entity = GetEntity(TransformUsageFlags.None);

                AddComponentObject(entity, new DeathFXDataC
                {
                    pool = authoring.pool,
                    sfx = authoring.sfx,
                });
            }
        }
    }

    public class DeathFXDataC : IComponentData
    {
        public ParticlePoolSO pool;
        public AudioSFX sfx;

    }



    public partial class PlayFXAfterDeathSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            foreach (var (data, worldMat) in SystemAPI.Query<DeathFXDataC, LocalToWorld>().WithAll<DeadEventC>())
            {
                data.sfx?.Play();
                data.pool.Pool.Get(out var particle);

                particle.Play();
                particle.transform.position = worldMat.Position;
            }
        }
    }

}
