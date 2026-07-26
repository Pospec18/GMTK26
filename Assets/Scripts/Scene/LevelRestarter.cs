using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pospec
{
    public class LevelRestarter : MonoBehaviour
    {
        [Header("Audio Settings")]
        public AudioSource audioSource;
        public AudioClip clickSound;

        public void RestartLevel()
        {
            // play click sound if assigned
            if (audioSource != null && clickSound != null)
            {
                audioSource.PlayOneShot(clickSound);
            }

            // get the name of the currently active scene
            string currentSceneName = SceneManager.GetActiveScene().name;

            // use FadeManager for a smooth transition if it exists in the scene
            if (FadeManager.instance != null)
            {
                FadeManager.instance.LoadScene(currentSceneName);
            }
            else
            {
                // fallback to instant reload if FadeManager is missing
                SceneManager.LoadScene(currentSceneName);
            }
        }
    }
}