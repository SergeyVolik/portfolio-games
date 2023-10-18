using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Prototype
{
    public class ParticlePool<T> : BasePool<T, ParticleSystem> where T : MonoBehaviour
    {
        public ParticleSystem particlePrefab;
        protected override ParticleSystem CreatePooledItem()
        {
            var particle = Instantiate(particlePrefab);
            particle.gameObject.SetActive(false);
          
            particle.gameObject.AddComponent<OnParticleStoppedEvent>().OnParticleStopped += () =>
            {
                Pool.Release(particle);
            };

            return particle;
        }

        protected override void OnDestroyPoolObject(ParticleSystem source)
        {

            Destroy(source.gameObject);
        }

        protected override void OnReturnedToPool(ParticleSystem source)
        {
            source.gameObject.SetActive(false);
        }

        protected override void OnTakeFromPool(ParticleSystem source)
        {
            source.gameObject.SetActive(true);

        }
    }

    public class SpawnParticlePool : ParticlePool<SpawnParticlePool> { }

    public struct ActivateSpawnParticleAtPosCommand : IComponentData
    {
        public float3 postion;
    }

    public partial class ActivateSpawnParticleSystem : SystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            RequireForUpdate<ActivateSpawnParticleAtPosCommand>();
        }
        protected override void OnUpdate()
        {
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(World.Unmanaged);

            foreach (var (part, e) in SystemAPI.Query<ActivateSpawnParticleAtPosCommand>().WithEntityAccess())
            {
                ecb.DestroyEntity(e);
                var particle = SpawnParticlePool.GetInstance().Pool.Get();
                particle.transform.position = part.postion;
                particle.Play();
            }
        }
    }
}
