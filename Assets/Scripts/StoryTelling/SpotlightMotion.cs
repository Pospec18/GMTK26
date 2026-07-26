using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Pospec
{
    public class SpotlightMotion : MonoBehaviour
    {
        [Header("Target Light")]
        [SerializeField] private Light2D spotlight;

        [Header("Movement Path (Figure 8)")]
        [Tooltip("Horizontal radius of the figure 8.")]
        [SerializeField] private float width = 3f;
        [Tooltip("Vertical radius of the figure 8.")]
        [SerializeField] private float height = 1.5f;
        [Tooltip("Base speed of the movement.")]
        [SerializeField] private float speed = 1f;

        [Header("Human Jitter / Noise")]
        [Tooltip("How much the light strays from the perfect figure 8 path.")]
        [SerializeField] private float noisePositionAmount = 0.4f;
        [Tooltip("Speed of the position jitter.")]
        [SerializeField] private float noiseSpeed = 1.5f;

        [Header("Intensity & Radius Flickering")]
        [SerializeField] private bool enableLightFlicker = true;
        [SerializeField] private float baseIntensity = 1.2f;
        [SerializeField] private float intensityNoiseAmount = 0.15f;

        [Header("Rotation Delay / Sway")]
        [Tooltip("Adds a subtle tilting effect as the spotlight moves.")]
        [SerializeField] private float maxTiltAngle = 5f;

        private Vector3 startPosition;
        private float noiseOffsetX;
        private float noiseOffsetY;

        private void Start()
        {
            if (spotlight == null)
            {
                spotlight = GetComponent<Light2D>();
            }

            startPosition = transform.localPosition;

            // randomize noise seeds so multiple lights don't move identically
            noiseOffsetX = Random.Range(0f, 100f);
            noiseOffsetY = Random.Range(100f, 200f);
        }

        private void Update()
        {
            float time = Time.time * speed;

            // 1. PERFECT FIGURE 8 (Lemniscate of Gerono / Bernoulli)
            // x = cos(t), y = sin(2t) / 2
            float rawX = Mathf.Cos(time) * width;
            float rawY = (Mathf.Sin(time * 2f) / 2f) * height;

            // 2. PERLIN NOISE FOR HUMAN HAND JITTER
            float noiseTime = Time.time * noiseSpeed;
            float offsetX = (Mathf.PerlinNoise(noiseTime + noiseOffsetX, 0f) - 0.5f) * 2f * noisePositionAmount;
            float offsetY = (Mathf.PerlinNoise(0f, noiseTime + noiseOffsetY) - 0.5f) * 2f * noisePositionAmount;

            // Apply combined position
            Vector3 targetPos = startPosition + new Vector3(rawX + offsetX, rawY + offsetY, 0f);
            transform.localPosition = targetPos;

            // 3. SUBTLE TILT / ROTATION BASED ON MOVEMENT
            // Tilts slightly in the direction it's moving on the X axis
            float tilt = -Mathf.Sin(time) * maxTiltAngle;
            transform.localRotation = Quaternion.Euler(0f, 0f, tilt);

            // 4. INTENSITY & PULSE NOISE
            if (enableLightFlicker && spotlight != null)
            {
                float intensityNoise = (Mathf.PerlinNoise(noiseTime * 2f, noiseTime * 2f) - 0.5f) * 2f * intensityNoiseAmount;
                spotlight.intensity = Mathf.Max(0f, baseIntensity + intensityNoise);
            }
        }

        // Visualize the approximate path in the Editor Scene View
        private void OnDrawGizmosSelected()
        {
            Vector3 center = Application.isPlaying ? startPosition : transform.localPosition;
            Gizmos.color = Color.yellow;

            Vector3 prevPoint = center + new Vector3(width, 0f, 0f);
            int steps = 50;

            for (int i = 1; i <= steps; i++)
            {
                float t = (i / (float)steps) * Mathf.PI * 2f;
                float x = Mathf.Cos(t) * width;
                float y = (Mathf.Sin(t * 2f) / 2f) * height;

                Vector3 currentPoint = center + new Vector3(x, y, 0f);
                Gizmos.DrawLine(prevPoint, currentPoint);
                prevPoint = currentPoint;
            }
        }
    }
}