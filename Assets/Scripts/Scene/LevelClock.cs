using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Pospec
{
    public class LevelClock : MonoBehaviour
    {
        [Header("Progress Map")]
        public Image[] levelIcons;
        [Tooltip("Assign your Light2D GameObjects here. Must match the order of levelIcons.")]
        public Light2D[] levelLights;
        public Color lockedColor = Color.gray;
        public Color unlockedColor = Color.white;

        [Header("Light Animation Settings")]
        public float targetLightIntensity = 1f;
        public float lightFadeDuration = 0.08f;
        [Tooltip("Delay in seconds before the light turns on and the spotlight sound plays.")]
        public float lightTurnOnDelay = 0.5f;

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
        [Tooltip("Sound that plays exactly when the level light turns on.")]
        public AudioClip spotLightSound;

        [Header("Scene Transition Settings")]
        public float delayBeforeTransition = 0.5f;

        private int levelToLoad;

        void Start()
        {
            // enable lowpass filter right when the clock cutscene starts
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

            // 1. SETUP ICONS & LIGHTS
            for (int i = 0; i < levelIcons.Length; i++)
            {
                bool hasLight = (levelLights != null && i < levelLights.Length && levelLights[i] != null);

                if (i < newlyCompletedIndex)
                {
                    levelIcons[i].color = unlockedColor;
                    if (hasLight)
                    {
                        // wake up the game object first
                        levelLights[i].gameObject.SetActive(true);
                        levelLights[i].enabled = true;
                        levelLights[i].intensity = targetLightIntensity;
                    }
                }
                else
                {
                    levelIcons[i].color = lockedColor;
                    if (hasLight)
                    {
                        levelLights[i].intensity = 0f;
                        levelLights[i].enabled = false;
                        // put the game object to sleep
                        levelLights[i].gameObject.SetActive(false);
                    }
                }
            }

            // 2. TRIGGER BASE AUDIO
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

            // 3. START FAST POP-ON LIGHT FOR NEWLY COMPLETED LEVEL (handles its own delay and sound)
            if (levelLights != null && newlyCompletedIndex >= 0 && newlyCompletedIndex < levelLights.Length)
            {
                Light2D activeLight = levelLights[newlyCompletedIndex];
                if (activeLight != null)
                {
                    StartCoroutine(FadeInLightFast(activeLight));
                }
            }

            // 4. ANIMATE CLOCK AND COLOR
            float elapsedTime = 0f;
            Vector3 initialHourEuler = hourArm != null ? hourArm.eulerAngles : Vector3.zero;

            while (elapsedTime < animationDuration)
            {
                float t = elapsedTime / animationDuration;
                t = t * t * (3f - 2f * t);

                float currentAngle = Mathf.Lerp(startAngle, targetAngle, t);
                clockArm.eulerAngles = new Vector3(0, 0, -currentAngle);

                if (isGameCompleted && hourArm != null)
                {
                    float hourAngle = Mathf.Lerp(0f, 15f, t);
                    hourArm.eulerAngles = new Vector3(0, 0, initialHourEuler.z - hourAngle);
                }

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
                if (AmbientManager.Instance != null)
                {
                    AmbientManager.Instance.SetCutsceneMode(false);
                }
                SceneManager.LoadScene(nextSceneName);
            }
        }

        private IEnumerator FadeInLightFast(Light2D light)
        {
            // wait for the configured delay before doing anything
            if (lightTurnOnDelay > 0f)
            {
                yield return new WaitForSeconds(lightTurnOnDelay);
            }

            // trigger the spotlight sound exactly as the light turns on
            if (audioSource != null && spotLightSound != null)
            {
                audioSource.PlayOneShot(spotLightSound);
            }

            // explicitly wake up the object before changing component values
            light.gameObject.SetActive(true);
            light.enabled = true;
            light.intensity = 0f;

            float time = 0f;
            while (time < lightFadeDuration)
            {
                time += Time.deltaTime;
                light.intensity = Mathf.Lerp(0f, targetLightIntensity, time / lightFadeDuration);
                yield return null;
            }

            light.intensity = targetLightIntensity;
        }
    }
}