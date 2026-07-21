using Pospec.Saving;
using UnityEngine;

namespace Pospec.ScreenSettings
{
    public class ScreenSettingsData : SaveData
    {
        public DetailLevel ResolutionLevel;
        public bool FullScreen;

        public ScreenSettingsData() : this(DetailLevel.Max, Screen.fullScreen) { }

        public ScreenSettingsData(DetailLevel resolutionLevel, bool fullScreen) : base(1)
        {
            ResolutionLevel = resolutionLevel;
            FullScreen = fullScreen;
        }
    }

    public enum DetailLevel { Low = 3, Middle = 2, High = 1, Max = 0 }
}
