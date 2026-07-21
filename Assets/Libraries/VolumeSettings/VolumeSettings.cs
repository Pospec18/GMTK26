using Pospec.Saving;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Pospec.VolumeSettings
{
    public class VolumeSettings : MonoBehaviour
    {
        [SerializeField] private AudioMixer audioMixer;

        [SerializeField] private List<VolumeSetter> volumeSetters = new List<VolumeSetter>();
        private const string saveKey = "AudioVolumes";

        private AudioSettingsData _data;
        private AudioSettingsData Data
        {
            get
            {
                if (_data == null)
                {
                    if (!SaveManager.Load(saveKey, out _data))
                    {
                        List<string> channels = new List<string>();
                        foreach (var s in volumeSetters)
                            channels.Add(s.VolumeName);

                        _data = new AudioSettingsData(1, channels);
                    }
                }
                return _data;
            }
        }

        private void Start()
        {
            try
            {
                foreach (var item in Data.channelVolumes)
                    audioMixer.SetFloat(item.Key, SliderToMixer(item.Value));
            }
            catch (Exception ex)
            {
                Debug.LogError("Error while applying settings: " + ex.Message);
            }
        }

        private void OnEnable()
        {
            foreach (var setter in volumeSetters)
            {
                setter.UpdateUI(Data.channelVolumes[setter.VolumeName]);
                setter.onChanged += volume => SetVolume(setter.VolumeName, volume);
            }
        }

        private void OnDisable()
        {
            foreach (var setter in volumeSetters)
            {
                setter.UpdateUI(Data.channelVolumes[setter.VolumeName]);
                setter.onChanged -= volume => SetVolume(setter.VolumeName, volume);
            }
        }

        public void SetVolume(string name, float volume)
        {
            audioMixer.SetFloat(name, SliderToMixer(volume));
            Data.channelVolumes[name] = volume;
            SaveManager.SaveShared(_data, saveKey);
        }

        private static float SliderToMixer(float sliderVal) => Mathf.Log10(sliderVal) * 20;

        [Serializable]
        private class AudioSettingsData : SaveData
        {
            public SerializableDictionary<string, float> channelVolumes = new SerializableDictionary<string, float>();

            public AudioSettingsData(int version, List<string> channels) : base(version)
            {
                foreach (var channel in channels)
                    channelVolumes[channel] = 1;
            }
        }
    }
}
