using UnityEngine;

namespace Pospec.Audio
{
    /// <summary>
    /// Empty object, so there is always some IAudioSourcePool present.
    /// </summary>
    public class NullAudioSourcePool : IAudioSourcePool
    {
        public void Play(AudioEvent audioEvent)
        {
            Debug.LogWarning("No AudioSourcePool present");
        }

        public void PlayFrom(AudioEvent audioEvent, Vector3 pos)
        {
            Debug.LogWarning("No AudioSourcePool present");
        }
    }
}
