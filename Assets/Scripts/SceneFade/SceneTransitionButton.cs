using UnityEngine;

namespace Pospec
{
    public class SceneTransitionButton : MonoBehaviour
    {
        [Header("Target Scene")]
        public string sceneToLoad;

        [Header("Optional Settings")]
        public float customFadeDuration = -1f;

        // This method will be called by the UI Button
        public void OnButtonPress()
        {
            // Call the Singleton FadeManager with the specified scene and duration
            FadeManager.instance.LoadScene(sceneToLoad, customFadeDuration);
        }
    }
}