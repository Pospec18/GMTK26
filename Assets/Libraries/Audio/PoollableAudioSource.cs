using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

namespace Pospec.Audio
{
    /// <summary>
    /// AudioSource that works with AudioSourcePool for reusable AudioSources.
    /// When audio is played, return's itself to the pool.
    /// </summary>
    public class PoollableAudioSource : MonoBehaviour
    {
        [SerializeField] private AudioSource source;
        private IObjectPool<PoollableAudioSource> pool;
        
        public PoollableAudioSource Setup(IObjectPool<PoollableAudioSource> pool)
        {
            this.pool = pool;
            return this;
        }

        private void Reset()
        {
            source = GetComponent<AudioSource>();
        }

        public void Play(AudioEvent audioEvent)
        {
            source.spatialBlend = 0;
            if (audioEvent != null)
                audioEvent.Play(source);
            if (!source.loop && source.clip != null)
                StartCoroutine(ReturnToPool(source.clip.length));
        }

        public void PlayFrom(AudioEvent audioEvent, Vector3 pos)
        {
            source.spatialBlend = 1;
            transform.position = pos;
            if (audioEvent != null)
                audioEvent.Play(source);
            if (!source.loop && source.clip != null)
                StartCoroutine(ReturnToPool(source.clip.length));
        }

        private IEnumerator ReturnToPool(float clipLength)
        {
            yield return new WaitForSecondsRealtime(clipLength);
            if (pool != null)
                pool.Release(this);
        }
    }
}
