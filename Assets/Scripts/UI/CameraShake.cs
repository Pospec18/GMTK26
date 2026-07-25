using UnityEngine;
using DG.Tweening;

namespace Pospec
{
    public class CameraShake : MonoBehaviour
    {
        public static CameraShake Instance { get; private set; }

        [Header("Default Shake Settings")]
        public float defaultDuration = 0.2f;
        public float defaultStrength = 0.5f;
        public int defaultVibrato = 20;
        public float defaultRandomness = 90f;

        private Vector3 m_OriginalPosition;

        void Awake()
        {
            // Singleton pattern setup
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            m_OriginalPosition = transform.localPosition;
        }

        /// <summary>
        /// Triggers camera shake with default inspector settings.
        /// </summary>
        public void Shake()
        {
            Shake(defaultDuration, defaultStrength, defaultVibrato, defaultRandomness);
        }

        /// <summary>
        /// Triggers custom camera shake with specific values.
        /// </summary>
        public void Shake(float duration, float strength, int vibrato = 20, float randomness = 90f)
        {
            // Stop previous shake to prevent offset buildup
            transform.DOKill();
            transform.localPosition = m_OriginalPosition;

            // Perform shake and guarantee return to original position
            transform.DOShakePosition(duration, strength, vibrato, randomness)
                .OnComplete(() => transform.localPosition = m_OriginalPosition);
        }

        void OnDisable()
        {
            // Clean up tween and reset position if disabled mid-shake
            transform.DOKill();
            transform.localPosition = m_OriginalPosition;
        }
    }
}