using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Pospec
{
    public class LineGear : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public float angularSpeed;
        public float radius;
        public List<LineGear> children;
        public ConnectionType connectionToParent;
        public Cell cell;
        public CircleCollider2D col;

        private bool isDragging;
        private bool placedThisFrame;
        private Vector3 grabOffset;
        private Camera dragCamera;

        public void OnPointerDown(PointerEventData eventData)
        {
            isDragging = true;
            col.enabled = false;
            if (cell)
                cell.grid.SelectGear(this);
            else
                FindAnyObjectByType<Grid>().SelectGear(this);

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
            col.enabled = true;
            placedThisFrame = true;
            if (cell)
                cell.grid.DeselectGear();
            else
                FindAnyObjectByType<Grid>().DeselectGear();
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

        public void PlaceToCell(Cell cell)
        {
            this.cell = cell;
            transform.position = cell.transform.position;
        }

        public void UpdateAngularSpeed(LineGear parent)
        {
            if (parent == null)
                return;

            switch (connectionToParent)
            {
                case ConnectionType.Stick:
                    angularSpeed = parent.angularSpeed;
                    break;
                case ConnectionType.Line:
                    angularSpeed = parent.radius * parent.angularSpeed / radius;
                    break;
                case ConnectionType.Teeth:
                    angularSpeed = -parent.radius * parent.angularSpeed / radius;
                    break;
                default:
                    break;
            }

            foreach (var child in children)
                child.UpdateAngularSpeed(this);
        }

        private void LateUpdate()
        {
            if (isDragging)
                FollowPointer();

            if (placedThisFrame)
            {
                placedThisFrame = false;
                PlaceToCell(cell);
            }

            transform.Rotate(Vector3.forward * angularSpeed * DiscreteTime.instance.DeltaTime);
        }
    }

    public enum ConnectionType { Stick, Line, Teeth };
}
