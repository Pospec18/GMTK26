using UnityEngine;

namespace Pospec
{
    public class TutorialManager : MonoBehaviour
    {
        public GameObject tutorialPanel;

        private void Start()
        {
            if (tutorialPanel != null)
            {
                tutorialPanel.SetActive(true);
            }
        }

        public void CloseTutorial()
        {
            if (tutorialPanel != null)
            {
                tutorialPanel.SetActive(false);
            }
        }

        public void OpenTutorial()
        {
            if (tutorialPanel != null)
            {
                tutorialPanel.SetActive(true);
            }
        }
    }
}