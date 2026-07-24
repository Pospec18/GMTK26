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
        public Column column;
        public int id;
        private bool isDragging;

        public float rotationNormalized() => transform.localEulerAngles.z / 360.0f;

        private List<Color> colors = new List<Color>()
        {
            new Color(0.59f, 0.44f, 0.20f, 0), Color.green, Color.blue,
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

        public void Setup(Column column, int id)
        {
            this.column = column;
            this.id = id;
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

        public void MoveToColumn(Column newColumn, int newId)
        {
            column.RemoveAt(id);
            column = newColumn;
            id = newId;
            column.AddAt(this, id);
            transform.parent = column.transform;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (id == 0 || id == column.LastStickId)
            {
                return;
            }

            isDragging = true;
            DragManager.instance.SelectStick(this);
            Debug.Log("DOWN " + gameObject.name);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isDragging = false;
            DragManager.instance.DeselectStick();
            Debug.Log("UP " + gameObject.name);
        }
    }
}
