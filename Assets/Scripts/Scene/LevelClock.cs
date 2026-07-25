using UnityEngine;
using UnityEngine.UI; // Required for the UI Image components
using System.Collections;
using UnityEngine.SceneManagement;

namespace Pospec
{
    public class LevelClock : MonoBehaviour
    {
        [Header("Progress Map")]
        public Image[] levelIcons;
        public Color lockedColor = Color.gray;
        public Color unlockedColor = Color.white;

        [Header("Clock Arms")]
        public Transform clockArm;
        public Transform hourArm;

        [Header("Animation Settings")]
        public float animationDuration = 4f;
        public float degreesPerLevel = 30f;
        public float startOffset = 185f;
        public int finalLevelNumber = 5;

        [Header("Audio")]
        public AudioSource audioSource;
        public AudioClip normalTickSound;
        public AudioClip finalLevelSound;
        public AudioClip iconColoringSound;

        [Header("Scene Transition Settings")]
        public float delayBeforeTransition = 0.5f;

        private int levelToLoad;

        void Start()
        {
            // Enable lowpass filter right when the clock cutscene starts
            if (AmbientManager.Instance != null)
            {
                AmbientManager.Instance.SetCutsceneMode(true);
            }

            levelToLoad = PlayerPrefs.GetInt("CurrentLevel", 3);

            float startAngle = ((levelToLoad - 1) * degreesPerLevel) + startOffset;
            float targetAngle = (levelToLoad * degreesPerLevel) + startOffset;

            StartCoroutine(AnimateClockArm(startAngle, targetAngle));
        }

        IEnumerator AnimateClockArm(float startAngle, float targetAngle)
        {
            bool isGameCompleted = (levelToLoad > finalLevelNumber);
            int newlyCompletedIndex = levelToLoad - 2;

            // 1. SETUP ICONS (Before animation starts)
            for (int i = 0; i < levelIcons.Length; i++)
            {
                if (i < newlyCompletedIndex)
                {
                    // Previously completed levels are fully colored
                    levelIcons[i].color = unlockedColor;
                }
                else
                {
                    // Uncompleted levels (including the one about to be colored) are gray
                    levelIcons[i].color = lockedColor;
                }
            }

            // 2. TRIGGER AUDIO
            if (audioSource != null)
            {
                if (isGameCompleted)
                {
                    audioSource.PlayOneShot(finalLevelSound);
                }
                else
                {
                    audioSource.PlayOneShot(normalTickSound);
                }
                // NEW: Play the coloring sound if an icon is about to change color
                if (iconColoringSound != null && newlyCompletedIndex >= 0 && newlyCompletedIndex < levelIcons.Length)
                {
                    audioSource.PlayOneShot(iconColoringSound);
                }
            }

            // 3. ANIMATE CLOCK AND COLOR SIMULTANEOUSLY
            float elapsedTime = 0f;
            Vector3 initialHourEuler = hourArm != null ? hourArm.eulerAngles : Vector3.zero;

            while (elapsedTime < animationDuration)
            {
                float t = elapsedTime / animationDuration;
                t = t * t * (3f - 2f * t); // Smoothstep

                // Spin the clock
                float currentAngle = Mathf.Lerp(startAngle, targetAngle, t);
                clockArm.eulerAngles = new Vector3(0, 0, -currentAngle);

                // Spin the hour arm if it's the finale
                if (isGameCompleted && hourArm != null)
                {
                    float hourAngle = Mathf.Lerp(0f, 15f, t);
                    hourArm.eulerAngles = new Vector3(0, 0, initialHourEuler.z - hourAngle);
                }

                // Fade the newly completed icon's color
                if (newlyCompletedIndex >= 0 && newlyCompletedIndex < levelIcons.Length)
                {
                    levelIcons[newlyCompletedIndex].color = Color.Lerp(lockedColor, unlockedColor, t);
                }

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(delayBeforeTransition);

            // 5. LOAD NEXT SCENE
            string nextSceneName = isGameCompleted ? "MainMenu" : "Level" + levelToLoad;

            if (FadeManager.instance != null)
            {
                FadeManager.instance.LoadScene(nextSceneName);
            }
            else
            {
                // Fallback: disable cutscene mode manually if FadeManager is missing
                if (AmbientManager.Instance != null)
                {
                    AmbientManager.Instance.SetCutsceneMode(false);
                }
                SceneManager.LoadScene(nextSceneName);
            }
        }
    }
}