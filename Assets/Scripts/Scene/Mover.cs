using DG.Tweening;
using UnityEngine;

namespace Pospec
{
    public class Mover : MonoBehaviour
    {
        public Vector3 from;
        public Vector3 to;
        public Ease ease;
        public Link link;

        private AudioSource m_MyAudioSource;
        private Vector3 m_LastPosition;

        // prevents audio artifacts
        private const float MovementThreshold = 0.00001f;

        void Start()
        {
            m_MyAudioSource = GetComponent<AudioSource>();
            // Initialize the last position to the starting position
            m_LastPosition = transform.position;
        }

        public void Update()
        {
            transform.position = DOVirtual.EasedValue(from, to, link.value, ease);

            // Calculate if the object has moved enough to be considered "moving"
            bool isMoving = Vector3.Distance(transform.position, m_LastPosition) > MovementThreshold;

            if (isMoving)
            {
                if (!m_MyAudioSource.isPlaying)
                {
                    m_MyAudioSource.Play();
                }
            }
            else
            {
                if (m_MyAudioSource.isPlaying)
                {
                    m_MyAudioSource.Pause();
                }
            }

            // Save the current position for the next frame's check
            m_LastPosition = transform.position;
        }
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(from, 0.1f);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(to, 0.1f);
        }
    }
}
