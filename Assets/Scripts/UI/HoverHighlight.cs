using UnityEngine;
using DG.Tweening;

namespace Pospec
{
    public class HoverHighlight : MonoBehaviour
    {
        [Header("Highlight Settings")]
        [Tooltip("Scale multiplier when hovered (e.g. 1.15 = 15% larger)")]
        public float scaleMultiplier = 1.15f;
        public float transitionTime = 0.2f;
        public Ease easeType = Ease.OutQuad;

        private Vector3 m_OriginalScale;
        private Vector3 m_TargetScale;

        void Awake()
        {
            // Cache original scale early before any modifications happen
            m_OriginalScale = transform.localScale;
            m_TargetScale = m_OriginalScale * scaleMultiplier;
        }

        void OnMouseEnter()
        {
            // Kill active tween to prevent conflict if fast hovering occurs
            transform.DOKill();
            transform.DOScale(m_TargetScale, transitionTime).SetEase(easeType);
        }

        void OnMouseExit()
        {
            // Safely return back to cached original scale
            transform.DOKill();
            transform.DOScale(m_OriginalScale, transitionTime).SetEase(easeType);
        }

        void OnDisable()
        {
            // Reset scale and kill tween if object is hidden or disabled mid-hover
            transform.DOKill();
            transform.localScale = m_OriginalScale;
        }

        void OnDestroy()
        {
            // Clean up running DOTween sequences on destroy
            transform.DOKill();
        }
    }
}