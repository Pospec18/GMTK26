using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Pospec
{
    public class Cell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public List<LineGear> gears = new List<LineGear>();
        [HideInInspector] public Vector2Int pos;
        public SpriteRenderer sr;
        [HideInInspector] public Grid grid;
        private bool isHovering = false;

        public TmpCell.CellType cellType;

        [Header("Visual Settings")]
        public Color normalColor = new Color(1f, 1f, 1f, 0f); // Default invisible
        public Color holeColor = Color.black;

        [Header("Global Highlight (When Dragging)")]
        public Color highlightValidColor = new Color(1f, 1f, 1f, 0.1f); // Subtle highlight for all valid drop zones
        public Color highlightInvalidColor = new Color(1f, 0f, 0f, 0.05f); // Subtle red for all invalid drop zones
        public Color highlightSpinColor = new Color(0f, 1f, 0f, 0.1f); // Subtle green where the gear would also start spinning

        [Header("Hover Highlight (Under Cursor)")]
        public Color hoverValidColor = new Color(1f, 1f, 1f, 0.3f);
        public Color hoverInvalidColor = new Color(1f, 0f, 0f, 0.5f);
        public Color hoverSpinColor = new Color(0f, 1f, 0f, 0.5f);

        public float colorTransitionSpeed = 15f;

        private Color targetColor;

        private BoxCollider2D col;
        // the cell art is split over several child renderers (tile, shadow, overlay),
        // so hiding a cell means hiding all of them - not just sr
        private SpriteRenderer[] visuals;

        public void Setup(Vector2Int pos, Grid grid)
        {
            this.pos = pos;
            this.grid = grid;
            gameObject.name = $"Cell {pos.x} {pos.y}";

            visuals = GetComponentsInChildren<SpriteRenderer>(true);
            col = GetComponent<BoxCollider2D>();
            if (col)
            {
                float pad = (1.0f + grid.hoverPadding) / transform.localScale.x;
                col.size = new Vector2(pad, pad);
            }

            targetColor = normalColor;
            if (sr) sr.color = normalColor;
        }

        public void SetCellType(TmpCell.CellType type)
        {
            cellType = type;

            bool isHole = type == TmpCell.CellType.Hole;
            if (visuals != null)
            {
                foreach (var visual in visuals)
                    if (visual) visual.enabled = !isHole;
            }
            if (isHole && col) col.enabled = false;
        }

        public bool TryPlaceGearOnTop(LineGear gear)
        {
            if (!CanPlaceGear(gear, true))
                return false;

            PlaceGearOnTop(gear);
            return true;
        }

        public void PlaceGearOnTop(LineGear gear)
        {
            gear.SetLevel(gears.Count);
            gears.Add(gear);
            gear.SetCell(this);
        }

        public bool CanPlaceGear(LineGear gear)
        {
            return CanPlaceGear(gear, false);
        }

        private bool CanPlaceGear(LineGear gear, bool log)
        {
            if (!gear) return false;

            if (cellType != TmpCell.CellType.Placeable)
            {
                if (log) Debug.Log("Cell " + pos + " is a " + cellType + ", cannot place gear on top");
                return false;
            }

            if (gear.cell == this) return false;

            if (gears.Count >= grid.maxLayers)
            {
                if (log) Debug.Log("Cell " + pos + " is full, cannot place gear on top");
                return false;
            }

            int level = gears.Count;

            foreach (var cell in grid.cells)
            {
                if (cell == this) continue;

                foreach (var g in cell.gears)
                {
                    if (AreGearsColliding(gear, level, g, this))
                    {
                        if (log) Debug.Log("Gear is colliding with another gear in cell " + cell.pos + ", cannot place gear on top");
                        return false;
                    }
                }
            }

            List<LineGear> rotatingGears = GetGearsRotatingTogetherWith(gear, level);
            if (WouldFormLoop(rotatingGears, gear) || CountSpinningNeighborsOnLevel(gear, level) > 1)
            {
                if (log) Debug.Log("Gear would close a loop in cell " + pos + ", cannot place gear on top");
                return false;
            }

            return true;
        }

        /// <summary>
        /// How many spinning gears on the same level would the added gear touch?
        /// Gears stacked in this cell are on other levels, so they are skipped.
        /// </summary>
        private int CountSpinningNeighborsOnLevel(LineGear addedGear, int addedLevel)
        {
            int count = 0;

            foreach (var cell in grid.cells)
            {
                if (cell == this) continue;

                foreach (var gear in cell.gears)
                {
                    if (gear.angularSpeed == 0.0f) continue;

                    if (AreGearsRotatingTogether(gear, addedGear, this, addedLevel))
                        count++;
                }
            }

            return count;
        }

        /// <summary>
        /// Would the gear rotate after being placed on top of this cell?
        /// </summary>
        public bool WouldGearSpin(LineGear gear)
        {
            if (!gear) return false;

            if (gear.angularSpeed != 0.0f) return true;

            foreach (var touchingGear in GetGearsRotatingTogetherWith(gear, gears.Count))
            {
                if (touchingGear.angularSpeed != 0.0f)
                    return true;
            }

            return false;
        }

        public void RemoveGear(LineGear gear)
        {
            gears.Remove(gear);
        }

        public void ClearGears()
        {
            foreach (var gear in gears)
            {
                if (!gear) continue;

                if (Application.isPlaying)
                {
                    Destroy(gear.gameObject);
                }
                else
                {
                    DestroyImmediate(gear.gameObject);
                }
            }
            gears.Clear();
        }

        private bool AreGearsColliding(LineGear thisGear, int thisLevel, LineGear otherGear, Cell thisCell)
        {
            if (!thisGear || !otherGear) return false;
            if (thisGear == otherGear) return false;
            if (thisLevel != otherGear.GetLevel()) return false;

            float distance = Vector3.Distance(thisCell.transform.position, otherGear.cell.transform.position);
            if (distance < thisGear.radius + otherGear.radius - grid.graceCollisionOffset)
                return true;

            return false;
        }

        private bool AreGearsRotatingTogether(LineGear thisGear, LineGear otherGear, Cell otherCell, int otherLevel)
        {
            if (!thisGear || !otherGear) return false;
            if (thisGear == otherGear) return false;
            if (!thisGear.cell || !otherCell) return false;

            if (thisGear.cell == otherCell) return true;
            if (thisGear.GetLevel() != otherLevel) return false;

            float distance = Vector3.Distance(thisGear.cell.transform.position, otherCell.transform.position);
            float touchDistance = thisGear.radius + otherGear.radius;

            return distance <= touchDistance + grid.graceCollisionOffset
                && distance >= touchDistance - grid.graceCollisionOffset;
        }

        private List<LineGear> GetGearsRotatingTogetherWith(LineGear addedGear)
        {
            return GetGearsRotatingTogetherWith(addedGear, addedGear.GetLevel());
        }

        private List<LineGear> GetGearsRotatingTogetherWith(LineGear addedGear, int addedLevel)
        {
            List<LineGear> result = new List<LineGear>();

            foreach (var cell in grid.cells)
            {
                foreach (var gear in cell.gears)
                {
                    if (AreGearsRotatingTogether(gear, addedGear, this, addedLevel))
                    {
                        result.Add(gear);
                    }
                }
            }

            return result;
        }

        private bool WouldFormLoop(List<LineGear> touchingGears, LineGear addedGear)
        {
            if (touchingGears.Count < 2) return false;

            HashSet<LineGear> reached = new HashSet<LineGear>();
            Stack<LineGear> toVisit = new Stack<LineGear>();
            reached.Add(touchingGears[0]);
            toVisit.Push(touchingGears[0]);

            while (toVisit.Count > 0)
            {
                LineGear gear = toVisit.Pop();
                foreach (var connection in gear.connectedTo)
                {
                    if (!connection || connection == addedGear) continue;

                    if (reached.Add(connection))
                        toVisit.Push(connection);
                }
            }

            for (int i = 1; i < touchingGears.Count; i++)
            {
                if (reached.Contains(touchingGears[i]))
                    return true;
            }

            return false;
        }

        public void LinkGears(LineGear addedGear)
        {
            List<LineGear> touchingGears = GetGearsRotatingTogetherWith(addedGear);
            List<LineGear> spinningGears = new List<LineGear>();

            foreach (var touchingGear in touchingGears)
            {
                touchingGear.AddConnection(addedGear);
                addedGear.AddConnection(touchingGear);

                if (touchingGear.angularSpeed != 0.0f)
                {
                    spinningGears.Add(touchingGear);
                }
            }

            if (spinningGears.Count > 1) return;

            if (spinningGears.Count == 1)
            {
                LineGear parent = spinningGears[0];
                if (parent.ShareSameCell(addedGear))
                {
                    addedGear.connectionToParent = ConnectionType.Stick;
                }
                else if (addedGear.connectionToParent != ConnectionType.Line)
                {
                    addedGear.connectionToParent = ConnectionType.Teeth;
                }
                addedGear.UpdateParent(parent);
            }
            else if (addedGear.angularSpeed != 0.0f)
            {
                foreach (var touchingGear in touchingGears)
                {
                    touchingGear.connectionToParent = touchingGear.ShareSameCell(addedGear)
                        ? ConnectionType.Stick
                        : ConnectionType.Teeth;
                    touchingGear.UpdateParent(addedGear);
                }
            }
        }

        private void Update()
        {
            // a hole has no art and no collider, so there is nothing to tint or hover
            if (cellType == TmpCell.CellType.Hole)
                return;

            DetermineTargetColor();
            SmoothUpdateColor();

            if (!grid.SelectedGear || grid.SelectedGear.cell)
            {
                sr.color = Color.white * 0.0f;
                col.enabled = false;
                return;
            }

            col.enabled = true;

            if (Input.GetMouseButtonUp(0) && isHovering)
            {
                isHovering = false;
                if (TryPlaceGearOnTop(grid.SelectedGear))
                {
                    LinkGears(grid.SelectedGear);
                    grid.SelectedGear.PlaceToCell(this);
                }
            }
        }

        private void DetermineTargetColor()
        {
            // 2. If nothing is selected or the gear is already placed somewhere, hide cells
            if (!grid.SelectedGear || grid.SelectedGear.cell)
            {
                targetColor = normalColor;
                return;
            }

            // 3. Evaluate if the currently held gear can be placed here
            bool canPlace = CanPlaceGear(grid.SelectedGear);

            if (canPlace)
            {
                // Globally show valid color. If hovered, show stronger valid color.
                // Cells where the gear would also start spinning get their own color.
                if (WouldGearSpin(grid.SelectedGear))
                    targetColor = isHovering ? hoverSpinColor : highlightSpinColor;
                else
                    targetColor = isHovering ? hoverValidColor : highlightValidColor;

                if (isHovering)
                {
                    grid.SelectedGear.sr.color = Dim(Color.white, 0.8f);
                    grid.NotifyGearTinted();
                    grid.ShowGhost(this, grid.SelectedGear);
                }
            }
            else
            {
                // Globally show invalid color. If hovered, show stronger invalid color.
                targetColor = isHovering ? hoverInvalidColor : highlightInvalidColor;

                if (isHovering)
                {
                    grid.SelectedGear.sr.color = Dim(Color.red, 0.8f);
                    grid.NotifyGearTinted();
                }
            }
        }

        private void SmoothUpdateColor()
        {
            if (sr.color != targetColor)
            {
                sr.color = Color.Lerp(sr.color, targetColor, Time.deltaTime * colorTransitionSpeed);
            }
        }

        private static Color Dim(Color color, float amount)
        {
            return new Color(color.r * amount, color.g * amount, color.b * amount, color.a);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isHovering = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isHovering = false;
        }
    }
}