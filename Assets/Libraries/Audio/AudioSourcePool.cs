using System;
using UnityEngine;
using UnityEngine.Pool;

namespace Pospec.Audio
{
    /// <summary>
    /// Pool of AudioSources for reusable use of AudioSources.
    /// </summary>
    public class AudioSourcePool : MonoBehaviour, IAudioSourcePool
    {
        public PoollableAudioSource sourcePrefab;
        public bool mainSFXPool = false;

        private ObjectPool<PoollableAudioSource> _pool;
        private ObjectPool<PoollableAudioSource> Pool
        {
            get
            {
                if (_pool == null)
                    _pool = new ObjectPool<PoollableAudioSource>(CreatePoolItem, OnTakeFromPool, OnReturnedToPool, OnDestroyedPoolObject);
                return _pool;
            }
        }

        private void Awake()
        {
            if (mainSFXPool)
                AudioSourcePoolLocator.Register(this);
        }

        private PoollableAudioSource CreatePoolItem()
        {
            return Instantiate(sourcePrefab, transform).Setup(_pool);
        }

        private void OnTakeFromPool(PoollableAudioSource source)
        {
            source.gameObject.SetActive(true);
        }

        private void OnReturnedToPool(PoollableAudioSource source)
        {
            source.gameObject.SetActive(false);
        }

        private void OnDestroyedPoolObject(PoollableAudioSource source)
        {
            Destroy(source.gameObject);
        }

        public void Play(AudioEvent audioEvent)
        {
            if (audioEvent == null)
                return;

            try
            {
                Pool.Get().Play(audioEvent);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public void PlayFrom(AudioEvent audioEvent, Vector3 pos)
        {
            if (audioEvent == null)
                return;

            try
            {
                Pool.Get().PlayFrom(audioEvent, pos);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}
