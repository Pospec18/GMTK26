using TMPro;
using UnityEngine;

namespace Pospec
{
    /// <summary>
    /// Infobox telling the player the end gear is turning at the wrong speed.
    /// Drop the prefab into a level scene, PuzzleGrid picks it up on its own.
    /// </summary>
    public class WrongSpeedInfobox : MonoBehaviour
    {
        public GameObject panel;
        public TMP_Text text;

        private void Awake()
        {
            Hide();
        }

        public void Show(string message)
        {
            if (text)
                text.text = message;
            if (panel && !panel.activeSelf)
                panel.SetActive(true);
        }

        public void Hide()
        {
            if (panel && panel.activeSelf)
                panel.SetActive(false);
        }
    }
}
