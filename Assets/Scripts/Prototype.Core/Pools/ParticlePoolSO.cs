using Sirenix.OdinInspector;
using UnityEngine;

namespace Prototype
{
    [CreateAssetMenu]
    public class ParticlePoolSO : BasePoolSO<ParticleSystem>
    {
        [AssetsOnly]
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
}
