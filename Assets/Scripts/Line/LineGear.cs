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

        public void OnPointerDown(PointerEventData eventData)
        {
            if (cell)
                cell.grid.SelectGear(this);
            else
                FindAnyObjectByType<Grid>().SelectGear(this);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (cell)
                cell.grid.DeselectGear();
            else
                FindAnyObjectByType<Grid>().DeselectGear();
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
            transform.Rotate(Vector3.forward * angularSpeed * DiscreteTime.instance.DeltaTime);
        }
    }

    public enum ConnectionType { Stick, Line, Teeth };
}
