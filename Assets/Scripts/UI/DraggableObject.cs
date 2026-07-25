using UnityEngine;
using DG.Tweening;

namespace Pospec
{
    [RequireComponent(typeof(Collider2D))]
    public class DraggableObject : MonoBehaviour
    {
        public enum ObjectSize { Small = 0, Medium = 1, Large = 2 }

        [Header("Object Configuration")]
        [SerializeField] private ObjectSize objectSize = ObjectSize.Medium;

        [Header("Audio Settings")]
        [SerializeField] private AudioSource audioSource;

        [System.Serializable]
        public struct SoundPair
        {
            public string name; // Pro přehlednost v Inspektoru (např. "Small Sounds")
            public AudioClip pickupSound;
            public AudioClip dropSound;
        }

        [Tooltip("Přesně 3 páry zvuků: [0] = Small, [1] = Medium, [2] = Large")]
        [SerializeField] private SoundPair[] soundPairs = new SoundPair[3];

        [Header("Juice / Squash & Stretch")]
        [SerializeField] private bool useSquashAndStretch = true;
        [SerializeField] private float squashAmount = 0.85f;
        [SerializeField] private float stretchAmount = 1.15f;
        [SerializeField] private float squashDuration = 0.15f;

        private Camera m_MainCamera;
        private Vector3 m_Offset;
        private Vector3 m_OriginalScale;
        private bool m_IsDragging = false;

        void Start()
        {
            m_MainCamera = Camera.main;
            m_OriginalScale = transform.localScale;

            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
                if (audioSource == null)
                {
                    audioSource = gameObject.AddComponent<AudioSource>();
                }
            }

            if (CameraManager.Instance == null)
            {
                Debug.LogWarning($"DraggableObject na {gameObject.name} nemůže najít CameraManager.Instance!");
            }
        }

        void OnMouseDown()
        {
            m_IsDragging = true;

            // Přehrání PICKUP zvuku přesně podle zvolené velikosti
            SoundPair currentPair = GetCurrentSoundPair();
            PlaySound(currentPair.pickupSound);

            // Reset scale tweenu
            transform.DOKill();
            transform.localScale = m_OriginalScale;

            Vector3 mouseWorldPos = GetMouseWorldPos();
            m_Offset = transform.position - mouseWorldPos;
        }

        void OnMouseDrag()
        {
            if (!m_IsDragging) return;
            transform.position = GetMouseWorldPos() + m_Offset;
        }

        void OnMouseUp()
        {
            if (!m_IsDragging) return;

            m_IsDragging = false;

            // 1. Přehrání DROP zvuku podle zvolené velikosti
            SoundPair currentPair = GetCurrentSoundPair();
            PlaySound(currentPair.dropSound);

            // 2. Mapování ShakeType přímo podle velikosti
            if (CameraManager.Instance != null)
            {
                CameraManager.ShakeType shakeType = GetShakeTypeForSize(objectSize);
                CameraManager.Instance.Shake(shakeType);
            }

            // 3. Squash & Stretch efekt
            if (useSquashAndStretch)
            {
                PlayImpactJuice();
            }
        }

        private SoundPair GetCurrentSoundPair()
        {
            int index = (int)objectSize;

            if (soundPairs != null && index >= 0 && index < soundPairs.Length)
            {
                return soundPairs[index];
            }

            return default;
        }

        private void PlaySound(AudioClip clip)
        {
            if (clip != null && audioSource != null)
            {
                audioSource.pitch = Random.Range(0.98f, 1.02f); // Jemná variace pitch pro přirozenost
                audioSource.PlayOneShot(clip);
            }
        }

        private CameraManager.ShakeType GetShakeTypeForSize(ObjectSize size)
        {
            switch (size)
            {
                case ObjectSize.Small: return CameraManager.ShakeType.Small;
                case ObjectSize.Medium: return CameraManager.ShakeType.Medium;
                case ObjectSize.Large: return CameraManager.ShakeType.Large;
                default: return CameraManager.ShakeType.Small;
            }
        }

        private void PlayImpactJuice()
        {
            transform.DOKill();

            Sequence squashSequence = DOTween.Sequence();

            Vector3 squashedScale = new Vector3(m_OriginalScale.x * stretchAmount, m_OriginalScale.y * squashAmount, m_OriginalScale.z);
            Vector3 stretchedScale = new Vector3(m_OriginalScale.x * squashAmount, m_OriginalScale.y * stretchAmount, m_OriginalScale.z);

            squashSequence.Append(transform.DOScale(squashedScale, squashDuration * 0.5f).SetEase(Ease.OutQuad))
                          .Append(transform.DOScale(stretchedScale, squashDuration * 0.3f).SetEase(Ease.InOutQuad))
                          .Append(transform.DOScale(m_OriginalScale, squashDuration * 0.2f).SetEase(Ease.OutBack));
        }

        private Vector3 GetMouseWorldPos()
        {
            Vector3 mousePoint = Input.mousePosition;
            mousePoint.z = Mathf.Abs(m_MainCamera.transform.position.z - transform.position.z);
            return m_MainCamera.ScreenToWorldPoint(mousePoint);
        }

        void OnDisable()
        {
            transform.DOKill();
            transform.localScale = m_OriginalScale;
        }
    }
}