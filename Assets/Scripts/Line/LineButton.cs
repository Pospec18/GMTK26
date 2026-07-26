using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Pospec
{
    public class LineButton : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public Action action;
        private bool isInteractible = true;
        public bool IsInteractible
        {
            get
            {
                return isInteractible;
            }
            set
            {
                isInteractible = value;
                image.color = isInteractible ? normalCol : disabledCol;
            }
        }
        public Image image;
        public Color normalCol;
        public Color highlitedCol;
        public Color disabledCol;

        public void OnClick(Action action)
        {
            this.action = action;
        }

        private void Update()
        {
            if (!IsInteractible)
                image.color = disabledCol;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!IsInteractible)
                return;
            action?.Invoke();
            image.color = normalCol;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!IsInteractible)
                return;
            image.color = highlitedCol;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!IsInteractible)
                return;
            image.color = normalCol;
        }
    }
}
