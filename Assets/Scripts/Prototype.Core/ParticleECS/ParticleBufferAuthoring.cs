using Unity.Entities;
using UnityEngine;

namespace SV.BallGame
{

    [DisallowMultipleComponent]
    public class ParticleBufferAuthoring : MonoBehaviour
    {

        void OnEnable() { }

        class Baker : Baker<ParticleBufferAuthoring>
        {
            public override void Bake(ParticleBufferAuthoring authoring)
            {
                if (!authoring.enabled)
                    return;

                var entity = GetEntity(TransformUsageFlags.None);

                var buffer = AddBuffer<ParticleBuffer>(entity);


                foreach (var item in GetComponentsInChildren<ParticleSystem>())
                {
                    buffer.Add(new ParticleBuffer
                    {
                        particleEntity = GetEntity(item, TransformUsageFlags.Dynamic)
                    });
                }

                AddComponent<PlayC>(entity);
                SetComponentEnabled<PlayC>(entity, false);
            }
        }
    }

    public struct ParticleBuffer : IBufferElementData
    {
        public Entity particleEntity;
    }
    public struct PlayC : IComponentData, IEnableableComponent
    {
       
    }


    public partial class PlayParticlesSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            foreach (var (playEn, buff) in SystemAPI.Query<EnabledRefRW<PlayC>, DynamicBuffer<ParticleBuffer>>())
            {
                foreach (var item in buff)
                {
                    SystemAPI.ManagedAPI.GetComponent<ParticleSystem>(item.particleEntity).Play();
                }

                playEn.ValueRW = false;
            }
        }
    }
}
