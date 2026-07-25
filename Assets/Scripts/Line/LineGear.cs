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
        private int level = -1;

        public List<LineGear> connectedTo = new List<LineGear>();

        public ConnectionType connectionToParent;
<<<<<<< Updated upstream
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
=======
        private LineGear parent = null;
        private Cell cell = null;

        public void SetCell(Cell cell)
        {
            this.cell = cell;
        }

        public bool ShareSameCell(LineGear other)
        {
            return this.cell != null && this.cell == other.cell;
>>>>>>> Stashed changes
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

            foreach (var connection in connectedTo)
            {
                if (connection != parent)
                {
                    connection.UpdateAngularSpeed(this);
                }
            }
        }

        private void LateUpdate()
        {
<<<<<<< Updated upstream
            if (isDragging)
                FollowPointer();

            if (placedThisFrame)
            {
                placedThisFrame = false;
                PlaceToCell(cell);
            }
=======
            if (DiscreteTime.instance == null)
                return;
>>>>>>> Stashed changes

            transform.Rotate(Vector3.forward * angularSpeed * DiscreteTime.instance.DeltaTime);
        }

        public int GetLevel()
        {
            return level;
        }

        public void SetLevel(int level)
        {
            this.level = level;
        }

        public void AddConnection(LineGear gear)
        {
            connectedTo.Add(gear);
        }

        public void UpdateParent(LineGear parent)
        {
            this.parent = parent;


            UpdateAngularSpeed(parent);

            // recursively update all siblings
            foreach (var connection in connectedTo)
            {
                if (connection != parent)
                {
                    if (ShareSameCell(connection))
                    {
                        connection.connectionToParent = ConnectionType.Stick;
                    }
                    else
                    {
                        connection.connectionToParent = ConnectionType.Teeth;
                    }
                    connection.UpdateParent(this);

                }
            }
        }

        public bool IsIdle()
        {
            return connectionToParent == ConnectionType.Teeth && angularSpeed == 0.0f;
        }
    }

    public enum ConnectionType { Stick, Line, Teeth };
}
