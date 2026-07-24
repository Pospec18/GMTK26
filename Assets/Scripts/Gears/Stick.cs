using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Pospec
{
    public class Stick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [HideInInspector] public float angularSpeed;
        [HideInInspector] public float distFromPrev;
        public List<Gear> gears;
        public CircleCollider2D pointer;
        private bool isDragging;

        public float rotationNormalized() => transform.localEulerAngles.z / 360.0f;

        private List<Color> colors = new List<Color>()
        {
            Color.red, Color.green, Color.blue,
        };

        private void Start()
        {
            for (int i = 0; i < colors.Count; i++)
            {
                Color c = colors[i];
                c.a = 0.5f;
                colors[i] = c;
            }
        }

        public void UpdateStick(Stick prev)
        {
            transform.Rotate(Vector3.forward * angularSpeed * Time.deltaTime);
            if (prev != null)
                transform.localPosition = prev.transform.localPosition + Vector3.down * distFromPrev;

            float draggingModif = isDragging ? 0.5f : 1.0f;

            for (int i = 0; i < gears.Count; i++)
                if (gears[i] != null)
                    gears[i].GetComponent<SpriteRenderer>().color = colors[i] * draggingModif;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isDragging = true;
            Debug.Log("DOWN " + gameObject.name);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isDragging = false;
            Debug.Log("UP " + gameObject.name);
        }
    }
}
