using UnityEngine;
using System.Collections;

namespace Pospec
{
public class LevelClock : MonoBehaviour
{
    public Transform clockArm;
    
    public float animationDuration = 1.5f;
    public float degreesPerLevel = 72f; 
    
    // This pushes the starting position half-way around the clock (to 6 o'clock)
    public float startOffset = 180f; 

    void Start()
    {
        int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        
        // We add the startOffset to both calculations
        float startAngle = ((currentLevel - 1) * degreesPerLevel) + startOffset;
        float targetAngle = (currentLevel * degreesPerLevel) + startOffset;
        
        StartCoroutine(AnimateClockArm(startAngle, targetAngle));
    }

    IEnumerator AnimateClockArm(float startAngle, float targetAngle)
    {
        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            float t = elapsedTime / animationDuration;
            t = t * t * (3f - 2f * t); 
            
            float currentAngle = Mathf.Lerp(startAngle, targetAngle, t);
            
            clockArm.eulerAngles = new Vector3(0, 0, -currentAngle);
            
            elapsedTime += Time.deltaTime;
            
            yield return null;
        }

        clockArm.eulerAngles = new Vector3(0, 0, -targetAngle);
    }
}
}