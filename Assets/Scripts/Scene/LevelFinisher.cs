using UnityEngine;
using UnityEngine.SceneManagement; 

namespace Pospec
{
 public class LevelFinisher : MonoBehaviour
{
    public string clockSceneName = "ClockScene"; 
    public int nextLevelNumber = 3; 

    // Adding this line creates a button in the Inspector!
    [ContextMenu("DEBUG: Finish Level Now")]
    public void FinishLevel()
    {
        PlayerPrefs.SetInt("CurrentLevel", nextLevelNumber);
        SceneManager.LoadScene(clockSceneName);
    }
}   
}