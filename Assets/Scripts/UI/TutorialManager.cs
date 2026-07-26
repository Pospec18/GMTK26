using UnityEngine;

namespace Pospec
{
    public class TutorialManager : MonoBehaviour
    {
        [Header("UI Settings")]
        public GameObject tutorialPanel;

        [Header("Audio Settings")]
        public AudioSource audioSource;
        public AudioClip clickSound;

        private void Start()
        {
            if (tutorialPanel != null)
            {
                tutorialPanel.SetActive(true);
            }
        }

        public void CloseTutorial()
        {
            PlayClickSound();

            if (tutorialPanel != null)
            {
                tutorialPanel.SetActive(false);
            }
        }

        public void OpenTutorial()
        {
            PlayClickSound();

            if (tutorialPanel != null)
            {
                tutorialPanel.SetActive(true);
            }
        }

        private void PlayClickSound()
        {
            if (audioSource != null && clickSound != null)
            {
                audioSource.PlayOneShot(clickSound);
            }
        }
    }
}