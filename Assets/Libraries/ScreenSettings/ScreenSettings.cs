using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Pospec.Saving;

namespace Pospec.ScreenSettings
{
    /// <summary>
    /// Settings for fullscreen and resolution.
    /// </summary>
    public class ScreenSettings : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown resolutionsDropdown;
        [SerializeField] private Toggle fullScreenToggle;

        private bool prevFullscreen = false;
        private bool fullscreenSettingsChanged = false;

        private const string saveKey = "ScreenSettings";

        private static ScreenSettingsData _data;
        public static ScreenSettingsData Data
        {
            get
            {
                if (_data == null)
                {
                    if (!SaveManager.Load(saveKey, out _data))
                    {
                        _data = new ScreenSettingsData();
                    }
                }
                return _data;
            }
        }

        private static List<Resolution> _resolutions;
        public static List<Resolution> Resolutions
        {
            get
            {
                if (_resolutions == null)
                {
                    _resolutions = new List<Resolution>();
                    try
                    {
                        Resolution[] allRes = Screen.resolutions;
                        AspectRatio ratio = new AspectRatio(allRes[allRes.Length - 1]);

                        for (int i = allRes.Length - 1; i >= 0; i--)
                        {
                            if (ratio.CorrespondsTo(allRes[i]))
                                _resolutions.Add(allRes[i]);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("Error while setting native resolutions: " + ex.Message);
                    }
                }
                return _resolutions;
            }
        }

        private void Start()
        {
            try
            {
                SetFullScreen(Data.FullScreen);
                SetupResolutionDropdown();
                SetResolution(Data.ResolutionLevel);
            }
            catch (Exception ex)
            {
                Debug.LogError("Error while applying settings: " + ex.Message);
            }

            SetupUI();
        }

        private void SetupUI()
        {
            if (fullScreenToggle != null)
            {
                fullScreenToggle.isOn = Data.FullScreen;
                fullScreenToggle.onValueChanged.AddListener(SetFullScreen);
            }
            if (resolutionsDropdown != null)
            {
                resolutionsDropdown.value = (int)Data.ResolutionLevel;
                resolutionsDropdown.onValueChanged.AddListener(SetResolution);
            }
        }

        private void OnValidate()
        {
            if (fullScreenToggle != null)
            {
                fullScreenToggle.interactable = true;
            }

            if (resolutionsDropdown != null)
            {
                SetupResolutionDropdown();
                resolutionsDropdown.interactable = true;
            }
        }

        private void SetupResolutionDropdown()
        {
            if (resolutionsDropdown == null)
                return;

            resolutionsDropdown.ClearOptions();
            if (Resolutions.Count == 0)
            {
                resolutionsDropdown.RefreshShownValue();
                return;
            }

            List<string> resText = new List<string>();
            foreach (DetailLevel detail in Enum.GetValues(typeof(DetailLevel)))
            {
                Resolution res = GetResolution(detail);
                resText.Add($"{detail} ({res.width} x {res.height})");
            }

            resolutionsDropdown.AddOptions(resText);
            resolutionsDropdown.RefreshShownValue();
        }

        private void OnDestroy()
        {
            if (fullScreenToggle != null)
            {
                fullScreenToggle.onValueChanged.RemoveListener(SetFullScreen);
            }
            if (resolutionsDropdown != null)
            {
                resolutionsDropdown.onValueChanged.RemoveListener(SetResolution);
            }
        }

        private void Awake()
        {
            prevFullscreen = Screen.fullScreen;
        }

        private void LateUpdate()
        {
            if (prevFullscreen != Screen.fullScreen && !fullscreenSettingsChanged)
            {
                prevFullscreen = Screen.fullScreen;
                //fullScreenToggle.isOn = prevFullscreen;
            }
            fullscreenSettingsChanged = false;
        }

        public void SetFullScreen(bool fullScreen)
        {
            Screen.fullScreen = fullScreen;
            Data.FullScreen = fullScreen;
            prevFullscreen = fullScreen;
            fullscreenSettingsChanged = true;
            ValueChanged();
        }

        public void SetResolution(DetailLevel detailLevel)
        {
            SetResolution((int)detailLevel);
        }

        private void SetResolution(int detailLevel)
        {
#if UNITY_WEBGL
#elif UNITY_ANDROID
#else
            if (resolutionsDropdown == null || Resolutions.Count == 0)
                return;

            Debug.Log("Changing resolution to " + detailLevel.ToString());

            Resolution current = GetResolution(detailLevel);
            Screen.SetResolution(current.width, current.height, Screen.fullScreen);
            Data.ResolutionLevel = (DetailLevel)detailLevel;
            ValueChanged();
#endif
        }

        public static Resolution GetResolution(DetailLevel detail)
        {
            return GetResolution((int)detail);
        }

        public static Resolution GetResolution(int detail)
        {
            if (Resolutions.Count == 0)
                return Screen.currentResolution;

            int i = detail * (Resolutions.Count - 1) / (int)DetailLevel.Low;
            return Resolutions[i];
        }

        private void ValueChanged()
        {
            SaveManager.SaveShared(_data, saveKey);
        }
    }
}
