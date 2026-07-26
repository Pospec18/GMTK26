using UnityEngine;

namespace Pospec
{
    public class SceneTransitionButton : MonoBehaviour
    {
        [Header("Target Scene")]
        public string sceneToLoad;

        [Header("Optional Settings")]
        public float customFadeDuration = -1f;

        [Header("UI Panels")]
        public GameObject settingsMenuPanel;
        public GameObject mainMenuPanel;

        private void Start()
        {
            // Forces the correct menus to show/hide the moment the game runs
            if (settingsMenuPanel != null) settingsMenuPanel.SetActive(false);
            if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        }

        public void OnButtonPress()
        {
            if (FadeManager.instance != null)
            {
                Debug.Log("Works play");
                FadeManager.instance.LoadScene(sceneToLoad, customFadeDuration);
            }
        }

        public void OpenSettingsMenu()
        {
            if (settingsMenuPanel != null) settingsMenuPanel.SetActive(true);
            if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        }

        public void CloseSettingsMenu()
        {
            if (settingsMenuPanel != null) settingsMenuPanel.SetActive(false);
            if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}