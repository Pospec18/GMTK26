using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Pospec
{
public class LevelClock : MonoBehaviour
{
    [Header("Clock Arms")]
    public Transform clockArm;
    public Transform hourArm;

    [Header("Animation Settings")]
    public float animationDuration = 2f;
    public float degreesPerLevel = 30f; 
    public float startOffset = 185f; 
    public int finalLevelNumber = 5;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip normalTickSound;
    public AudioClip finalLevelSound;

    private int levelToLoad; 

    void Start()
    {
        levelToLoad = PlayerPrefs.GetInt("CurrentLevel", 3);
        
        float startAngle = ((levelToLoad - 1) * degreesPerLevel) + startOffset;
        float targetAngle = (levelToLoad * degreesPerLevel) + startOffset;
        
        StartCoroutine(AnimateClockArm(startAngle, targetAngle));
    }

    IEnumerator AnimateClockArm(float startAngle, float targetAngle)
    {
        // Check if the player just finished the last level
        bool isGameCompleted = (levelToLoad > finalLevelNumber);

        // Play the correct sound
        if (audioSource != null)
        {
            if (isGameCompleted)
            {
                audioSource.PlayOneShot(finalLevelSound);
            }
            else
            {
                audioSource.PlayOneShot(normalTickSound);
            }
        }

        float elapsedTime = 0f;
        
        // Save the starting rotation of the hour arm if it exists
        Vector3 initialHourEuler = hourArm != null ? hourArm.eulerAngles : Vector3.zero;

        while (elapsedTime < animationDuration)
        {
            float t = elapsedTime / animationDuration;
            t = t * t * (3f - 2f * t); 
            
            // Animate the main arm normally
            float currentAngle = Mathf.Lerp(startAngle, targetAngle, t);
            clockArm.eulerAngles = new Vector3(0, 0, -currentAngle);
            
            // If it is the final level, slowly move the hour arm by 30 degrees
            if (isGameCompleted && hourArm != null)
            {
                float hourAngle = Mathf.Lerp(0f, 15f, t);
                hourArm.eulerAngles = new Vector3(0, 0, initialHourEuler.z - hourAngle);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        
        // Load the next scene
        if (isGameCompleted)
        {
            // Change "MainMenu" to whatever your end-game scene is named
            SceneManager.LoadScene("MainMenu"); 
        }
        else
        {
            SceneManager.LoadScene("Level" + levelToLoad); 
        }
    }
}
}