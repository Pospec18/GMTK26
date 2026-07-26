using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Pospec
{
    public class LineButton : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public Action action;
        public bool isInteractible = true;
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
            if (!isInteractible)
                image.color = disabledCol;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!isInteractible)
                return;
            action?.Invoke();
            image.color = normalCol;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!isInteractible)
                return;
            image.color = highlitedCol;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!isInteractible)
                return;
            image.color = normalCol;
        }
    }
}
