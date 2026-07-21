using UnityEngine;
using UnityEngine.UI;

namespace Pospec.VolumeSettings
{
    public class SliderVolumeSetter : VolumeSetter
    {
        [SerializeField] private Slider slider;

        private void Start()
        {
            slider.onValueChanged.AddListener(SetVolume);
        }

        private void Reset()
        {
            slider = GetComponentInChildren<Slider>();
        }

        private void OnValidate()
        {
            if (slider == null)
                return;
            slider.minValue = 0.0001f;
            slider.maxValue = 1;
            slider.wholeNumbers = false;
            slider.interactable = true;
        }

        private void OnDestroy()
        {
            slider.onValueChanged.RemoveListener(SetVolume);
        }

        public override void UpdateUI(float volume)
        {
            slider.value = volume;
        }
    }
}
