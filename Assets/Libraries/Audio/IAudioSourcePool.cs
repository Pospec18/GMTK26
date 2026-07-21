using UnityEngine;

namespace Pospec.Audio
{
    public interface IAudioSourcePool
    {
        public void Play(AudioEvent audioEvent);
        public void PlayFrom(AudioEvent audioEvent, Vector3 pos);
    }
}
