using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

//usage: FadeManager.instance.LoadScene("Level3", 2.5f);
// or without the float for default value


namespace Pospec
{
    public class FadeManager : MonoBehaviour
    {
        public static FadeManager instance;

        [Header("UI References")]
        public CanvasGroup fadeCanvasGroup;

        [Header("Settings")]
        public float defaultFadeDuration = 1f;

        void Awake()
        {
            // Singleton pattern to ensure only one FadeManager exists
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            // Automatically fade in when the game starts
            if (fadeCanvasGroup != null)
            {
                fadeCanvasGroup.alpha = 1f;
                fadeCanvasGroup.blocksRaycasts = false;
                fadeCanvasGroup.DOFade(0f, defaultFadeDuration).SetUpdate(true);
            }
        }

        // Method to call when you want to change scenes
        // customDuration is optional; if left empty, it uses the default duration
        public void LoadScene(string sceneName, float customDuration = -1f)
        {
            if (fadeCanvasGroup == null) return;

            // Use custom duration if provided, otherwise use default
            float duration = customDuration > 0f ? customDuration : defaultFadeDuration;

            // Block screen clicks while fading
            fadeCanvasGroup.blocksRaycasts = true;

            // Fade to black
            fadeCanvasGroup.DOFade(1f, duration).SetUpdate(true).OnComplete(() =>
            {
                // Load the new scene after the screen is fully black
                SceneManager.LoadScene(sceneName);

                // Fade back to transparent
                fadeCanvasGroup.DOFade(0f, duration).SetUpdate(true).OnComplete(() =>
                {
                    // Allow clicking again
                    fadeCanvasGroup.blocksRaycasts = false;
                });
            });
        }
    }
}