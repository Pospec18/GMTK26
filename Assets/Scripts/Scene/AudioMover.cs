using UnityEngine;

namespace Pospec
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioMover : MonoBehaviour
    {
        public Link link;
        [Header("Audio Settings")]
        public AudioClip clip;
        [Header("Inertia & Momentum Controls")]
        [Tooltip("How quickly velocity catches up to your input. Lower = heavier feel/more momentum.")]
        [Range(1f, 30f)]
        public float responsiveness = 8f;

        [Tooltip("How smoothly the pitch adjusts to current speed. Prevents audio artifacts.")]
        [Range(1f, 30f)]
        public float pitchSmoothing = 12f;

        [Tooltip("Maximum allowed playback speed/pitch multiplier.")]
        public float maxPitch = 3.0f;

        private AudioSource audioSource;

        // Internal tracking variables
        private float currentNormalizedTime;
        private float currentVelocity;
        private float smoothPitch;

        void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.clip = clip;
            audioSource.loop = true;
            audioSource.playOnAwake = false;
            audioSource.Play();
        }

        void Update()
        {
            if (clip == null || clip.length <= 0) return;

            // 1. Smoothly interpolate current position toward target position with momentum
            // MathDamp / SmoothDamp acts like a physical spring with mass
            float previousTime = currentNormalizedTime;
            currentNormalizedTime = Mathf.SmoothDamp(
                currentNormalizedTime,
                link.value,
                ref currentVelocity,
                1f / responsiveness
            );

            // 2. Calculate the actual physical speed (rate of change per second)
            float frameDelta = currentNormalizedTime - previousTime;
            float actualSpeed = frameDelta / Time.deltaTime;

            // 3. Smooth the calculated speed into pitch to prevent sharp audio clicks
            smoothPitch = Mathf.Lerp(smoothPitch, actualSpeed, Time.deltaTime * pitchSmoothing);

            // Clamp to avoid extreme audio distortion
            float finalPitch = Mathf.Clamp(smoothPitch, -maxPitch, maxPitch);

            // 4. Apply pitch to AudioSource (Pitch inherently scales speed + frequency)
            audioSource.pitch = finalPitch;

            // 5. Sync the internal playback position with the calculated smoothed position
            float targetTimeInSeconds = Mathf.Repeat(currentNormalizedTime * clip.length, clip.length);

            // Only resync the playback head if the drift is noticeable (> 50ms)
            if (Mathf.Abs(audioSource.time - targetTimeInSeconds) > 0.05f)
            {
                audioSource.time = targetTimeInSeconds;
            }
        }
    }
}
