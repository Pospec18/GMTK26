using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Pospec
{
    public class LineGear : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public float angularSpeed;
        public float radius;
        public bool canMove;

        private int level = -1;

        public List<LineGear> connectedTo = new List<LineGear>();

        public ConnectionType connectionToParent;
        public Cell cell;
        public CircleCollider2D col;
        private LineGear parent = null;
        public SpriteRenderer sr;

        public int normalSortingLayer;
        public int holdingSortingLayer;

        public bool isDragging;
        private bool placedThisFrame;
        private Vector3 grabOffset;
        private Camera dragCamera;
        private Cell originCell;
        private Grid dragGrid;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!canMove)
                return;

            isDragging = true;
            dragGrid = cell ? cell.grid : FindAnyObjectByType<Grid>();
            dragGrid.SelectGear(this);

            // the gear leaves the grid as soon as we grab it, so it is not driven by
            // anything while it is in our hand
            originCell = cell;
            DetachFromGrid();

            dragCamera = eventData.pressEventCamera != null ? eventData.pressEventCamera : Camera.main;
            if (dragCamera != null)
            {
                Vector3 screen = eventData.position;
                screen.z = dragCamera.WorldToScreenPoint(transform.position).z;
                grabOffset = transform.position - dragCamera.ScreenToWorldPoint(screen);
            }

            Color c = sr.color;
            c.a = 0.35f;
            sr.color = c;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!canMove)
                return;

            isDragging = false;
            placedThisFrame = true;
            Grid grid = dragGrid ? dragGrid : (cell ? cell.grid : FindAnyObjectByType<Grid>());
            grid.DeselectGear();

            Color c = sr.color;
            c.a = 1.0f;
            sr.color = c;
        }

        private void DetachFromGrid()
        {
            List<LineGear> wasConnectedTo = new List<LineGear>(connectedTo);

            if (cell)
                cell.RemoveGear(this);

            ClearConnections();
            SetLevel(-1);
            cell = null;

            // gears that were driven stop, gears spinning on their own keep their speed
            if (parent != null)
            {
                parent = null;
                angularSpeed = 0.0f;
            }

            foreach (var other in wasConnectedTo)
            {
                if (other)
                    other.StopDrivenBy(this);
            }
        }

        // whatever this gear used to drive has lost its source of rotation
        private void StopDrivenBy(LineGear removedGear)
        {
            if (parent != removedGear)
                return;

            parent = null;
            angularSpeed = 0.0f;

            foreach (var connection in new List<LineGear>(connectedTo))
            {
                if (connection)
                    connection.StopDrivenBy(this);
            }
        }

        private void ReturnToOriginCell()
        {
            if (originCell.TryPlaceGearOnTop(this))
                originCell.LinkGears(this);
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
            int idx = cell.gears.IndexOf(this);
            sr.sortingOrder = idx;
            transform.position = cell.transform.position + Vector3.forward * idx;
        }

        public void SetCell(Cell cell)
        {
            this.cell = cell;
        }

        public bool ShareSameCell(LineGear other)
        {
            return this.cell != null && this.cell == other.cell;
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
            col.enabled = Grid.Instance.SelectedGear == null;
            if (col.enabled && cell)
                col.enabled = cell.gears[cell.gears.Count - 1] == this; // only top gear can be moved

            if (isDragging)
            {
                sr.sortingLayerID = SortingLayer.layers[holdingSortingLayer].id;
                FollowPointer();
            }
            else
                sr.sortingLayerID = SortingLayer.layers[normalSortingLayer].id;

            if (placedThisFrame)
            {
                placedThisFrame = false;

                // the drop did not land on a cell, so the gear goes back where it came from
                if (cell == null && originCell != null)
                    ReturnToOriginCell();

                originCell = null;
                if (cell != null)
                    PlaceToCell(cell);
            }

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

        public void ClearConnections()
        {
            foreach (var other in connectedTo)
            {
                if (other)
                    other.connectedTo.Remove(this);
            }

            connectedTo.Clear();
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
