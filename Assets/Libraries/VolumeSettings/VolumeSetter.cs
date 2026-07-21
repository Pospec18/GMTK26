using System;
using UnityEngine;

namespace Pospec.VolumeSettings
{
    public abstract class VolumeSetter : MonoBehaviour
    {
        [SerializeField] private string volumeName;

        public string VolumeName => volumeName;
        public event Action<float> onChanged;

        public abstract void UpdateUI(float volume);

        protected void SetVolume(float volume)
        {
            onChanged?.Invoke(volume);
        }
    }
}
