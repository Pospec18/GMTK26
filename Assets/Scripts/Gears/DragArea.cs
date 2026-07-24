using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Pospec
{
    public class DragArea : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public SpriteRenderer sr;
        public BoxCollider2D col;
        private bool isHovering;
        private Column column;
        private int id;

        private void Start()
        {
            sr.color = Color.white * 0.2f;
        }

        public void Setup(Column column, int id)
        {
            this.column = column;
            this.id = id;
        }

        private void Update()
        {
            if (Input.GetMouseButtonUp(0) && isHovering)
            {
                column.Swap(id);
                sr.color = Color.white * 0.2f;
                isHovering = false;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isHovering = true;
            sr.color = Color.white * 0.8f;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isHovering = false;
            sr.color = Color.white * 0.2f;
        }

        public void Reposition(Vector3 position, float distFromPrev)
        {
            transform.localPosition = position + Vector3.down * distFromPrev / 2 + Vector3.back;
            Vector3 size = transform.localScale;
            size.y = distFromPrev - 0.1f;
            transform.localScale = size;
        }

        public void Active(bool isActive)
        {
            gameObject.SetActive(isActive);
        }
    }
}
