using UnityEngine;
using System.Collections;

namespace Pospec
{
    public class SplashScreen : MonoBehaviour
    {
        [Header("Scene Transition")]
        public string nextSceneName = "MainMenu";

        [Header("Timings")]
        public float displayTime = 3f;

        void Start()
        {
            // Start the waiting coroutine as soon as the scene loads
            StartCoroutine(WaitAndLoad());
        }

        private IEnumerator WaitAndLoad()
        {
            // Wait for the specified amount of seconds
            yield return new WaitForSeconds(displayTime);

            // Transition to the next scene using our FadeManager
            if (FadeManager.instance != null)
            {
                FadeManager.instance.LoadScene(nextSceneName);
            }
            else
            {
                // Fallback just in case FadeManager is missing
                Debug.LogWarning("FadeManager not found! Loading scene directly.");
                UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
            }
        }
    }
}