using UnityEngine;
using UnityEngine.SceneManagement; 

namespace Pospec
{
    public class LevelFinisher : MonoBehaviour
    {
        [Header("UI Settings")]
        public GameObject winScreenUI; // Drag your Win Screen Panel here

        [Header("Level Settings")]
        public string clockSceneName = "ClockScene"; 
        public int nextLevelNumber = 3; 

        private void Start()
        {
            // Ensure the win screen is completely hidden when the level starts
            if (winScreenUI != null)
            {
                winScreenUI.SetActive(false);
            }
        }

        // 1. THIS HAPPENS WHEN THE PLAYER BEATS THE LEVEL
        // Developers will trigger this from their gameplay scripts
        [ContextMenu("DEBUG: 1. Finish Level (Show UI)")]
        public void FinishLevel()
        {
            Debug.Log("Finish Level was triggered!"); // <--- ADD THIS LINE

            if (winScreenUI != null)
            {
                Debug.Log("Win screen is assigned, turning it on now."); // <--- ADD THIS LINE
                winScreenUI.SetActive(true);
            }
            else
            {
                Debug.LogWarning("Wait, the winScreenUI slot is empty in the Inspector!"); // <--- ADD THIS LINE
            }
        }

        // 2. THIS HAPPENS WHEN THE PLAYER CLICKS "CONTINUE"
        // Connect your UI Button to this function
        [ContextMenu("DEBUG: 2. Continue To Clock Scene")]
        public void ContinueToClockScene()
        {

            // Save progress and load the clock transition
            PlayerPrefs.SetInt("CurrentLevel", nextLevelNumber);
            SceneManager.LoadScene(clockSceneName);
        }
    }   
}