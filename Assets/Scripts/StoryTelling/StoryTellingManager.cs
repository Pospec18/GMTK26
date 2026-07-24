using System.Collections;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;

namespace Pospec
{
    public class StoryManager : MonoBehaviour
    {
        [Header("UI Elements")]
        public TMP_Text storyTextDisplay;

        [Header("Story Settings")]
        [TextArea(3, 5)]
        public string[] storyLines;

        [Header("Timings")]
        public float fadeInDuration = 1f;
        public float displayDuration = 2f;
        public float fadeOutDuration = 1f;

        [Header("Scene Management")]
        public string nextSceneName;

        [Header("Controls")]
        public float doubleTapThreshold = 0.5f;

        [Header("Audio Effects")]
        public AudioLowPassFilter lowPassFilter;
        public float startFrequency = 22000f;
        public float targetFrequency = 1000f;

        private float m_LastSpaceTime = -100f;
        private Coroutine m_StoryCoroutine;
        private Tween m_LowPassTween;

        void Start()
        {
            // Ensure the text starts completely transparent
            Color startColor = storyTextDisplay.color;
            startColor.a = 0f;
            storyTextDisplay.color = startColor;

            // Setup and start the low pass filter tween
            if (lowPassFilter != null)
            {
                lowPassFilter.cutoffFrequency = startFrequency;

                // Calculate the total time the story will take
                float totalDuration = storyLines.Length * (fadeInDuration + displayDuration + fadeOutDuration);

                // Tween the frequency linearly over the total duration
                m_LowPassTween = DOVirtual.Float(startFrequency, targetFrequency, totalDuration, (value) =>
                {
                    lowPassFilter.cutoffFrequency = value;
                }).SetEase(Ease.Linear);
            }

            // Start the story sequence
            m_StoryCoroutine = StartCoroutine(PlayStorySequence());
        }

        void Update()
        {
            // Check for double spacebar press to skip
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (Time.time - m_LastSpaceTime < doubleTapThreshold)
                {
                    SkipStory();
                }

                m_LastSpaceTime = Time.time;
            }
        }

        private IEnumerator PlayStorySequence()
        {
            foreach (string line in storyLines)
            {
                // Update the text content
                storyTextDisplay.text = line;

                // Fade in
                yield return storyTextDisplay.DOFade(1f, fadeInDuration).WaitForCompletion();

                // Wait while the text is fully visible
                yield return new WaitForSeconds(displayDuration);

                // Fade out
                yield return storyTextDisplay.DOFade(0f, fadeOutDuration).WaitForCompletion();
            }

            // All texts have been shown, load the next scene
            GoToNextScene();
        }

        private void SkipStory()
        {
            // Stop the coroutine and any active tweens on the text to prevent errors
            if (m_StoryCoroutine != null)
            {
                StopCoroutine(m_StoryCoroutine);
            }

            storyTextDisplay.DOKill();

            // Kill the audio filter tween if it is running
            m_LowPassTween?.Kill();

            GoToNextScene();
        }

        private void GoToNextScene()
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}