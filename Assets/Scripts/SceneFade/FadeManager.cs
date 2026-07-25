using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

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
            if (fadeCanvasGroup != null)
            {
                fadeCanvasGroup.alpha = 1f;
                fadeCanvasGroup.blocksRaycasts = false;
                fadeCanvasGroup.DOFade(0f, defaultFadeDuration).SetUpdate(true);
            }
        }

        public void LoadScene(string sceneName, float customDuration = -1f)
        {
            if (fadeCanvasGroup == null) return;

            float duration = customDuration > 0f ? customDuration : defaultFadeDuration;

            fadeCanvasGroup.blocksRaycasts = true;

            fadeCanvasGroup.DOFade(1f, duration).SetUpdate(true).OnComplete(() =>
            {
                SceneManager.LoadScene(sceneName);

                // Revert audio filter back to normal when entering the new scene
                if (AmbientManager.Instance != null)
                {
                    AmbientManager.Instance.SetCutsceneMode(false);
                }

                fadeCanvasGroup.DOFade(0f, duration).SetUpdate(true).OnComplete(() =>
                {
                    fadeCanvasGroup.blocksRaycasts = false;
                });
            });
        }
    }
}