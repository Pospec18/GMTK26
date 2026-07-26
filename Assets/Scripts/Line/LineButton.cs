using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Pospec
{
    public class LineButton : MonoBehaviour, IPointerDownHandler
    {
        public Action action;

        public void OnClick(Action action)
        {
            this.action = action;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            action?.Invoke();
        }
    }
}
