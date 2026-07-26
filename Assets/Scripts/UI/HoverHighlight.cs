using UnityEngine;
using DG.Tweening;

namespace Pospec
{
    public class HoverHighlight : MonoBehaviour
    {
        [Header("Highlight Settings")]
        public float scaleMultiplier = 1.12f;
        public float transitionTime = 0.1f;
        public Ease easeType = Ease.OutQuad;

        [Header("Z-Offset (Lift effect)")]
        public float zOffset = -0.5f;

        [Header("Shadow Settings")]
        public Transform shadowTransform;
        public Vector3 shadowLiftOffset = new Vector3(0.1f, -0.1f, 0f);
        public float shadowScaleMultiplier = 1.08f;

        // Reference na vaše kolečko/script (nastavte v Inspector nebo Awake)
        [Header("References")]
        [SerializeField] private LineGear gear;

        private Vector3 m_OriginalScale;
        private Vector3 m_TargetScale;

        private Vector3 m_ShadowOriginalPos;
        private Vector3 m_ShadowTargetPos;
        private Vector3 m_ShadowOriginalScale;
        private Vector3 m_ShadowTargetScale;

        private bool m_IsHovered;
        private bool m_IsCurrentlyLifted;

        void Awake()
        {
            if (gear == null) gear = GetComponent<LineGear>();

            m_OriginalScale = transform.localScale;
            m_TargetScale = m_OriginalScale * scaleMultiplier;

            if (shadowTransform != null)
            {
                m_ShadowOriginalPos = shadowTransform.localPosition;
                m_ShadowTargetPos = m_ShadowOriginalPos + shadowLiftOffset;

                m_ShadowOriginalScale = shadowTransform.localScale;
                m_ShadowTargetScale = m_ShadowOriginalScale * shadowScaleMultiplier;
            }
        }

        /// <summary>Changes the size the gear returns to when it is not hovered, for gears that
        /// are drawn bigger because of the layer they sit on.</summary>
        public void SetBaseScale(Vector3 scale)
        {
            m_OriginalScale = scale;
            m_TargetScale = m_OriginalScale * scaleMultiplier;

            if (!m_IsCurrentlyLifted)
            {
                transform.DOKill();
                transform.localScale = m_OriginalScale;
            }
        }

        void Update()
        {
            // Chceme být "zvednutí", pokud na nás myš hoveruje NEBO pokud kolečko právě dragujeme
            bool shouldBeLifted = m_IsHovered || (gear != null && gear.isDragging);

            if (shouldBeLifted != m_IsCurrentlyLifted)
            {
                m_IsCurrentlyLifted = shouldBeLifted;
                ApplyLiftState(m_IsCurrentlyLifted);
            }
        }

        void OnMouseEnter()
        {
            // Unity zavolá OnMouseEnter jen když je collider zapnutý
            m_IsHovered = true;
        }

        void OnMouseExit()
        {
            m_IsHovered = false;
        }

        public void ForceResetHover()
        {
            m_IsHovered = false;
        }

        private void ApplyLiftState(bool lift)
        {
            transform.DOKill();
            Vector3 targetScale = lift ? m_TargetScale : m_OriginalScale;
            transform.DOScale(targetScale, transitionTime).SetEase(easeType);

            // Posun na Z-ose
            Vector3 targetPos = transform.localPosition;
            targetPos.z = lift ? (targetPos.z + zOffset) : (targetPos.z - zOffset);
            // Pro jistotu pouzijeme vzdy relativni posun vůči zakladu, nebo nastavte pevne Z:
            transform.DOLocalMoveZ(lift ? zOffset : 0f, transitionTime).SetEase(easeType);

            // Stín
            if (shadowTransform != null)
            {
                shadowTransform.DOKill();
                Vector3 targetShadowPos = lift ? m_ShadowTargetPos : m_ShadowOriginalPos;
                Vector3 targetShadowScale = lift ? m_ShadowTargetScale : m_ShadowOriginalScale;

                shadowTransform.DOLocalMove(targetShadowPos, transitionTime).SetEase(easeType);
                shadowTransform.DOScale(targetShadowScale, transitionTime).SetEase(easeType);
            }
        }

        void OnDisable()
        {
            m_IsHovered = false;
            m_IsCurrentlyLifted = false;

            transform.DOKill();
            transform.localScale = m_OriginalScale;

            if (shadowTransform != null)
            {
                shadowTransform.DOKill();
                shadowTransform.localPosition = m_ShadowOriginalPos;
                shadowTransform.localScale = m_ShadowOriginalScale;
            }
        }

        void OnDestroy()
        {
            transform.DOKill();
            if (shadowTransform != null) shadowTransform.DOKill();
        }
    }
}