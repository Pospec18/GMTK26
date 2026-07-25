using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using DG.Tweening;

namespace Pospec
{
    public class AmbientManager : MonoBehaviour
    {
        public static AmbientManager Instance { get; private set; }

        [Header("Audio Setup")]
        public AudioMixerGroup mixerGroup;
        public AudioClip[] ambientTracks;

        [Header("Crossfade Settings")]
        public float crossfadeDuration = 3f;
        [Range(0f, 1f)] public float maxVolume = 1f;

        [Header("Mixer Lowpass Settings")]
        public AudioMixer mainMixer;
        public string lowpassParamName = "AmbienceLowpass";
        public float normalFrequency = 22000f; // Fully open filter
        public float cutsceneFrequency = 800f; // Muffled underwater effect
        public float lowpassTransitionTime = 1.5f;

        private AudioSource m_SourceA;
        private AudioSource m_SourceB;
        private bool m_IsPlayingA = true;
        private int m_LastPlayedIndex = -1;

        void Awake()
        {
            // Singleton pattern & DontDestroyOnLoad
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            SetupAudioSources();
        }

        void Start()
        {
            if (ambientTracks.Length > 0)
            {
                StartCoroutine(CrossfadeLoop());
            }
        }

        private void SetupAudioSources()
        {
            // Create two identical audio sources for crossfading
            m_SourceA = gameObject.AddComponent<AudioSource>();
            m_SourceB = gameObject.AddComponent<AudioSource>();

            m_SourceA.outputAudioMixerGroup = mixerGroup;
            m_SourceB.outputAudioMixerGroup = mixerGroup;

            m_SourceA.loop = false;
            m_SourceB.loop = false;

            m_SourceA.volume = 0f;
            m_SourceB.volume = 0f;
        }

        private IEnumerator CrossfadeLoop()
        {
            while (true)
            {
                // Determine active and inactive sources
                AudioSource activeSource = m_IsPlayingA ? m_SourceA : m_SourceB;
                AudioSource fadingOutSource = m_IsPlayingA ? m_SourceB : m_SourceA;

                // Select next random clip, ensuring it's different from the last one
                int nextIndex;
                do
                {
                    nextIndex = Random.Range(0, ambientTracks.Length);
                } while (nextIndex == m_LastPlayedIndex && ambientTracks.Length > 1);

                m_LastPlayedIndex = nextIndex;
                AudioClip nextClip = ambientTracks[nextIndex];

                // Prepare and play the active source
                activeSource.clip = nextClip;
                activeSource.Play();

                // Crossfade using DOTween
                activeSource.DOFade(maxVolume, crossfadeDuration);
                if (fadingOutSource.isPlaying)
                {
                    fadingOutSource.DOFade(0f, crossfadeDuration).OnComplete(() => fadingOutSource.Stop());
                }

                // Wait until the current clip is almost finished before starting the next crossfade
                float waitTime = nextClip.length - crossfadeDuration;

                // Safety check in case clip is shorter than crossfade duration
                if (waitTime <= 0) waitTime = nextClip.length / 2f;

                yield return new WaitForSeconds(waitTime);

                // Swap active sources for the next iteration
                m_IsPlayingA = !m_IsPlayingA;
            }
        }

        /// <summary>
        /// Transitions the ambience to a muffled sound for cutscenes.
        /// </summary>
        public void SetCutsceneMode(bool isCutscene)
        {
            if (mainMixer == null)
            {
                Debug.LogError("MainMixer is not assigned in the AmbientManager Inspector!");
                return;
            }

            float targetFreq = isCutscene ? cutsceneFrequency : normalFrequency;

            // Safely get the current frequency, fallback if it fails
            if (!mainMixer.GetFloat(lowpassParamName, out float currentFreq))
            {
                currentFreq = normalFrequency;
                Debug.LogWarning($"Parameter '{lowpassParamName}' not found in AudioMixer! Check if it is exposed and spelled correctly.");
            }

            // Kill any running transition to prevent overlapping tweens
            DOTween.Kill(this);

            // Smoothly interpolate the float value
            DOVirtual.Float(currentFreq, targetFreq, lowpassTransitionTime, (value) =>
            {
                mainMixer.SetFloat(lowpassParamName, value);
            }).SetId(this);
        }
    }
}