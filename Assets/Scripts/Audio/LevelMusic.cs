using UnityEngine;

namespace Pospec
{
    public class LevelMusic : MonoBehaviour
    {
        [Header("Music Settings")]
        public AudioClip sceneMusic;
        public float fadeDuration = 1.5f;

        void Start()
        {
            // Tell the persistent AudioManager to play this clip
            if (AudioManager.instance != null && sceneMusic != null)
            {
                AudioManager.instance.PlayMusic(sceneMusic, fadeDuration);
            }
            else if (AudioManager.instance == null)
            {
                Debug.LogWarning("AudioManager not found in the scene! Start the game from the title scene.");
            }
        }
    }
}