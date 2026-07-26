using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

namespace Pospec
{
    public class InteractiveLight : MonoBehaviour
    {
        [Header("Light Settings")]
        [SerializeField] private Light2D targetLight;
        [SerializeField] private float targetIntensity = 1f;
        [SerializeField] private float fadeDuration = 0.08f;

        [Header("Audio Settings")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip switchSound;

        [Header("Feedback Settings")]
        [Tooltip("Camera shake intensity to simulate physical interaction")]
        [SerializeField] private CameraManager.ShakeType shakeSeverity = CameraManager.ShakeType.Small;

        private Coroutine fadeCoroutine;

        // call this method from your UI Button's OnClick event
        public void TurnOnLight()
        {
            if (targetLight == null) return;

            // stop any ongoing fade if the button is spammed
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            fadeCoroutine = StartCoroutine(FadeInLightFast());
        }

        private IEnumerator FadeInLightFast()
        {
            // 1. play interaction sound
            if (audioSource != null && switchSound != null)
            {
                audioSource.PlayOneShot(switchSound);
            }

            // 2. trigger camera shake to simulate human touch
            if (CameraManager.Instance != null)
            {
                CameraManager.Instance.Shake(shakeSeverity);
            }

            // 3. wake up the light and set initial state
            targetLight.gameObject.SetActive(true);
            targetLight.enabled = true;
            targetLight.intensity = 0f;

            // 4. fast fade in
            float time = 0f;
            while (time < fadeDuration)
            {
                time += Time.deltaTime;
                targetLight.intensity = Mathf.Lerp(0f, targetIntensity, time / fadeDuration);
                yield return null;
            }

            targetLight.intensity = targetIntensity;
        }
    }
}