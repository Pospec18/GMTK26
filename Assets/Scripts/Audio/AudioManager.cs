using UnityEngine;
using DG.Tweening; // Using DOTween for smooth volume fading

namespace Pospec
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager instance;

        private AudioSource m_AudioSource;
        private float m_TargetVolume = 1f; // Default max volume

        void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(this.gameObject);

            m_AudioSource = GetComponent<AudioSource>();

            // Save the initial volume set in the inspector as the target volume
            m_TargetVolume = m_AudioSource.volume;
        }

        public void PlayMusic(AudioClip newClip, float fadeDuration = 1f)
        {
            if (newClip == null) return;

            // If the requested clip is already playing, do nothing (seamless transition)
            if (m_AudioSource.clip == newClip && m_AudioSource.isPlaying)
            {
                return;
            }

            // Stop any active volume tweens to prevent conflicts
            m_AudioSource.DOKill();

            if (m_AudioSource.isPlaying)
            {
                // Fade out current music, change clip, then fade in new music
                m_AudioSource.DOFade(0f, fadeDuration).OnComplete(() =>
                {
                    m_AudioSource.clip = newClip;
                    m_AudioSource.Play();
                    m_AudioSource.DOFade(m_TargetVolume, fadeDuration);
                });
            }
            else
            {
                // If nothing is playing, set clip, volume to 0, play and fade in
                m_AudioSource.clip = newClip;
                m_AudioSource.volume = 0f;
                m_AudioSource.Play();
                m_AudioSource.DOFade(m_TargetVolume, fadeDuration);
            }
        }
    }
}