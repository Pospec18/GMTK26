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
        private Camera dragCamera;
        private Vector3 grabOffset;

        public float rotationNormalized() => transform.localEulerAngles.z / 360.0f;

        private List<Color> colors = new List<Color>()
        {
            new Color(0.59f, 0.44f, 0.20f, 0), Color.green, Color.blue,
        };

        private void Start()
        {
            if (id == 0 || id == column.LastStickId)
            {
                pointer.enabled = false;
            }

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

        public void UpdateStick(Vector3 layoutLocalPos)
        {
            transform.Rotate(Vector3.forward * angularSpeed * DiscreteTime.instance.DeltaTime);
            if (isDragging)
                FollowPointer();
            else
                transform.localPosition = layoutLocalPos;

            float draggingModif = isDragging ? 0.5f : 1.0f;

            for (int i = 0; i < gears.Count; i++)
                if (gears[i] != null)
                    gears[i].GetComponent<SpriteRenderer>().color = colors[i] * draggingModif;
        }

        private void FollowPointer()
        {
            Camera cam = dragCamera != null ? dragCamera : Camera.main;
            if (cam == null)
                return;

            Vector3 screen = Input.mousePosition;
            screen.z = cam.WorldToScreenPoint(transform.position).z;
            Vector3 world = cam.ScreenToWorldPoint(screen) + grabOffset;
            world.z = transform.position.z;
            transform.position = world;
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

            dragCamera = eventData.pressEventCamera != null ? eventData.pressEventCamera : Camera.main;
            if (dragCamera != null)
            {
                Vector3 screen = eventData.position;
                screen.z = dragCamera.WorldToScreenPoint(transform.position).z;
                grabOffset = transform.position - dragCamera.ScreenToWorldPoint(screen);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isDragging = false;
            DragManager.instance.DeselectStick();
        }

        public void SetInitRotation(float valueNormalized)
        {
            transform.localRotation = Quaternion.AngleAxis(valueNormalized * 360.0f, Vector3.forward);
        }
    }
}
