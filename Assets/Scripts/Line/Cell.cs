using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Pospec
{
    public class Cell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public List<LineGear> gear = new List<LineGear>();
        public Vector2Int pos { get; private set; }
        public SpriteRenderer sr;
        public Grid grid { get; private set; }
        private bool isHovering = false;

        public void Setup(Vector2Int pos, Grid grid)
        {
            this.pos = pos;
            this.grid = grid;
        }

        public bool TryPlaceGearOnTop(LineGear gear)
        {
            return true;
        }

        public void RemoveTopGear()
        {

        }

        private void Update()
        {
            if (!grid.SelectedGear)
            {
                sr.color = Color.white;
                return;
            }

            if (Input.GetMouseButtonUp(0) && isHovering)
            {
                isHovering = false;
                if (TryPlaceGearOnTop(grid.SelectedGear))
                    grid.SelectedGear.PlaceToCell(this);
            }
            sr.color = Color.white * (isHovering ? 0.8f : 0.4f);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (grid.SelectedGear != null)
                isHovering = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (grid.SelectedGear != null)
                isHovering = false;
        }
    }
}
