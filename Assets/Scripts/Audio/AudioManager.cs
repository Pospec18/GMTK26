using UnityEngine;

namespace Pospec
{
    public class AudioManager : MonoBehaviour
    {
        // Static instance holds the single active AudioManager
        public static AudioManager instance;

        void Awake()
        {
            // Check if an instance already exists and it's not this one
            if (instance != null && instance != this)
            {
                // Destroy the duplicate to prevent overlapping music
                Destroy(this.gameObject);
                return;
            }

            // Set this as the active instance
            instance = this;

            // Tell Unity not to destroy this object when loading a new scene
            DontDestroyOnLoad(this.gameObject);
        }
    }
}