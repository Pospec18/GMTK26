using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pospec
{
    public class LevelFinisher : MonoBehaviour
    {
        [Header("UI Settings")]
        public GameObject winScreenUI;

        [Header("Standard Level Progression")]
        public string clockSceneName = "ClockScene";
        public int nextLevelNumber = 3;

        [Header("Custom Scene Option")]
        [Tooltip("If checked, 'ContinueToClockScene' will ignore ClockScene and load the Custom Scene Name below instead.")]
        public bool useCustomScene = false;
        public string customSceneName = "MainMenu";

        private void Start()
        {
            // Ensure the win screen is hidden when the level starts
            if (winScreenUI != null)
            {
                winScreenUI.SetActive(false);
            }
        }

        // 1. THIS HAPPENS WHEN THE PLAYER BEATS THE LEVEL
        [ContextMenu("DEBUG: 1. Finish Level (Show UI)")]
        public void FinishLevel()
        {
            if (winScreenUI != null)
            {
                winScreenUI.SetActive(true);
            }
        }

        // 2. THIS HAPPENS WHEN THE PLAYER CLICKS "CONTINUE"
        [ContextMenu("DEBUG: 2. Continue To Clock Scene")]
        public void ContinueToClockScene()
        {
            if (useCustomScene)
            {
                LoadSceneWithFade(customSceneName);
            }
            else
            {
                // Save progress and load the clock transition
                PlayerPrefs.SetInt("CurrentLevel", nextLevelNumber);
                LoadSceneWithFade(clockSceneName);
            }
        }

        // 3. LOAD ANY CUSTOM SCENE BY NAME
        // You can also call this directly from a UI Button by passing a string
        public void LoadCustomScene(string sceneName)
        {
            if (!string.IsNullOrEmpty(sceneName))
            {
                LoadSceneWithFade(sceneName);
            }
        }

        private void LoadSceneWithFade(string sceneToLoad)
        {
            if (FadeManager.instance != null)
            {
                FadeManager.instance.LoadScene(sceneToLoad);
            }
            else
            {
                SceneManager.LoadScene(sceneToLoad);
            }
        }
    }
}