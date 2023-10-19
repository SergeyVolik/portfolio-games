using Prototype.Audio;
using UnityEngine;

namespace Prototype
{
    public class AudioSourcePool : BasePool<AudioSourcePool, AudioSource>
    {
        protected override AudioSource CreatePooledItem()
        {
            var go = new GameObject("sfx");
            var ps = go.AddComponent<AudioSource>();

            go.AddComponent<OnAudioFinishedEvent>().OnAudioFinished += () =>
            {
                Pool.Release(ps);
            };

            return ps;
        }

        // Called when an item is returned to the pool using Release
        protected override void OnReturnedToPool(AudioSource system)
        {
            system.gameObject.SetActive(false);
        }

        // Called when an item is taken from the pool using Get
        protected override void OnTakeFromPool(AudioSource system)
        {
            system.gameObject.SetActive(true);
        }

        // If the pool capacity is reached then any items returned will be destroyed.
        // We can control what the destroy behavior does, here we destroy the GameObject.
        protected override void OnDestroyPoolObject(AudioSource system)
        {
            Destroy(system.gameObject);
        }
    }
}