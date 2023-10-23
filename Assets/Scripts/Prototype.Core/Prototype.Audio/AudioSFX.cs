using Prototype.SO;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

namespace Prototype.Audio
{
    /// <summary>
    /// Audio setting for playing <see cref="AudioSource">
    /// </summary>
    [CreateAssetMenu(menuName = "Prototype/AudioSFX")]
    public class AudioSFX : AssetWithGuid
    {
        [MinMaxSlider(0, 1)]
        public Vector2 volumeRange = new Vector2(1, 1);

        [MinMaxSlider(0, 1)]
        public Vector2 pinch = new Vector2(1, 1);

        public AudioClip clip;
        public AudioMixerGroup mixer;

        /// <summary>
        /// Setup and play AudioSource
        /// </summary>
        /// <param name="source"></param>
        public void Play(AudioSource source)
        {
            source.clip = clip;
            source.volume = UnityEngine.Random.Range(volumeRange.x, volumeRange.y);
            source.pitch = UnityEngine.Random.Range(pinch.x, pinch.y);

            source.outputAudioMixerGroup = mixer;
            source.Play();
        }

        public void Play()
        {
            AudioSourcePool.GetInstance().Pool.Get(out var source);

            Play(source);
        }
    }
}
