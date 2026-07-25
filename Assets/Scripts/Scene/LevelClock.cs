using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Pospec
{
    public class LevelClock : MonoBehaviour
    {
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
            }

            float elapsedTime = 0f;
            Vector3 initialHourEuler = hourArm != null ? hourArm.eulerAngles : Vector3.zero;

            while (elapsedTime < animationDuration)
            {
                float t = elapsedTime / animationDuration;
                t = t * t * (3f - 2f * t); // Smoothstep

                float currentAngle = Mathf.Lerp(startAngle, targetAngle, t);
                clockArm.eulerAngles = new Vector3(0, 0, -currentAngle);

                if (isGameCompleted && hourArm != null)
                {
                    float hourAngle = Mathf.Lerp(0f, 20f, t);
                    hourArm.eulerAngles = new Vector3(0, 0, initialHourEuler.z - hourAngle);
                }

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(delayBeforeTransition);

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