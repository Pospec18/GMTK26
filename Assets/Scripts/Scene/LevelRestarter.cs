using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pospec
{
    public class LevelRestarter : MonoBehaviour
    {
        public void RestartLevel()
        {
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