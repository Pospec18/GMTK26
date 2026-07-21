using UnityEngine;

public class ExitGame : MonoBehaviour
{
#if UNITY_WEBGL
    public void Awake()
    {
        Destroy(gameObject);
    }
#endif

    public void Exit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#endif
    }
}
