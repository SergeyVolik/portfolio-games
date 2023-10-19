using Prototype.SO;
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
        [Range(0, 1)]
        public float volume;

        public AudioClip clip;
        public AudioMixerGroup mixer;

        /// <summary>
        /// Setup and play AudioSource
        /// </summary>
        /// <param name="source"></param>
        public void Play(AudioSource source)
        {
            source.clip = clip;
            source.volume = volume;
            source.outputAudioMixerGroup = mixer;
            source.Play();
        }
    }
}
