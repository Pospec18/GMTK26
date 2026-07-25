using System;
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

        public void Setup(Vector2Int pos, Grid grid)
        {
            this.pos = pos;
            this.grid = grid;
            gameObject.name = $"Cell {pos.x} {pos.y}";

            var col = GetComponent<BoxCollider2D>();
            if (col)
            {
                float pad = (1.0f + grid.hoverPadding) / transform.localScale.x;
                col.size = new Vector2(pad, pad);
            }
        }

        public bool TryPlaceGearOnTop(LineGear gear)
        {
            if (!CanPlaceGear(gear, true))
                return false;

            // okay lets place it then bro
            gear.SetLevel(gears.Count);
            gears.Add(gear);
            gear.SetCell(this);

            return true;
        }

        // read only, does not touch the gear in any way, so it is safe to call for
        // things like highlighting cells while dragging
        public bool CanPlaceGear(LineGear gear)
        {
            return CanPlaceGear(gear, false);
        }

        private bool CanPlaceGear(LineGear gear, bool log)
        {
            if (!gear)
                return false;

            // dropping a gear back where it already is would add it to this cell twice
            if (gear.cell == this)
            {
                return false;
            }

            // this cell can only have maxLayers gears
            if (gears.Count >= grid.maxLayers)
            {
                if (log)
                    Debug.Log("Cell " + pos + " is full, cannot place gear on top");
                return false;
            }

            // the level the gear would end up on if we placed it here
            int level = gears.Count;

            // we need to check with every already placed gear in the grid if it is not colliding
            foreach (var cell in grid.cells)
            {
                if (cell == this)
                    continue;

                foreach (var g in cell.gears)
                {
                    if (AreGearsColliding(gear, level, g, this))
                    {
                        if (log)
                            Debug.Log("Gear is colliding with another gear in cell " + cell.pos + ", cannot place gear on top");
                        return false;
                    }
                }
            }

            if (WouldFormLoop(GetGearsRotatingTogetherWith(gear, level), gear))
            {
                if (log)
                    Debug.Log("Gear would close a loop in cell " + pos + ", cannot place gear on top");
                return false;
            }

            return true;
        }

        public void RemoveGear(LineGear gear)
        {
            gears.Remove(gear);
        }

        public void ClearGears()
        {
            foreach (var gear in gears)
            {
                if (!gear)
                    continue;

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

        // thisLevel is the level thisGear sits on (or would sit on, when we are still
        // deciding whether it can be placed)
        private bool AreGearsColliding(LineGear thisGear, int thisLevel, LineGear otherGear, Cell thisCell)
        {
            if (!thisGear || !otherGear)
                return false;

            if (thisGear == otherGear)
                return false;

            if (thisLevel != otherGear.GetLevel())
                return false;

            float distance = Vector3.Distance(thisCell.transform.position, otherGear.cell.transform.position);

            // gears may overlap slightly before we call it a collision
            if (distance < thisGear.radius + otherGear.radius - grid.graceCollisionOffset)
                return true;

            return false;
        }

        // otherCell / otherLevel is where otherGear sits (or is about to sit, when we are
        // still deciding whether it can be placed there)
        private bool AreGearsRotatingTogether(LineGear thisGear, LineGear otherGear, Cell otherCell, int otherLevel)
        {
            if (!thisGear || !otherGear)
                return false;

            if (thisGear == otherGear)
                return false;

            if (!thisGear.cell || !otherCell)
                return false;

            // gears stacked in the same cell sit on a shared axle, so they always turn
            // together
            if (thisGear.cell == otherCell)
                return true;

            if (thisGear.GetLevel() != otherLevel)
                return false;

            float distance = Vector3.Distance(thisGear.cell.transform.position, otherCell.transform.position);
            float touchDistance = thisGear.radius + otherGear.radius;

            // gears drive each other only when their teeth meet. too far apart and they
            // never touch, too close and they are overlapping
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
            if (touchingGears.Count < 2)
                return false;

            HashSet<LineGear> reached = new HashSet<LineGear>();
            Stack<LineGear> toVisit = new Stack<LineGear>();
            reached.Add(touchingGears[0]);
            toVisit.Push(touchingGears[0]);

            while (toVisit.Count > 0)
            {
                LineGear gear = toVisit.Pop();
                foreach (var connection in gear.connectedTo)
                {
                    if (!connection || connection == addedGear)
                        continue;

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
            // we need to check if the gear is touching any other gear in the grid and link them if they are

            // first we find if we can find a touching gear that is already spinning
            List<LineGear> touchingGears = GetGearsRotatingTogetherWith(addedGear);

            // check which ones are already rotating
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

            if (spinningGears.Count > 1)
            {
                return;
            }

            if (spinningGears.Count == 1)
            {
                LineGear parent = spinningGears[0];
                if (parent.ShareSameCell(addedGear))
                {
                    addedGear.connectionToParent = ConnectionType.Stick;
                }
                else
                {
                    addedGear.connectionToParent = ConnectionType.Teeth;
                }
                addedGear.UpdateParent(parent);
            }
            else if (addedGear.angularSpeed != 0.0f)
            {
                // nothing drives the new gear, but it is spinning, so it drives what it touches
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
            if (cellType == TmpCell.CellType.Hole)
            {
                sr.color = Color.black;
                return;
            }

            if (!grid.SelectedGear)
            {
                sr.color = Color.white;
                return;
            }

            if (Input.GetMouseButtonUp(0) && isHovering)
            {
                isHovering = false;
                // the gear already left its old cell and its connections when it was
                // picked up, so we only have to add it here
                if (TryPlaceGearOnTop(grid.SelectedGear))
                {
                    LinkGears(grid.SelectedGear);

                    grid.SelectedGear.PlaceToCell(this);
                }
            }

            if (grid.SelectedGear.cell)
            {
                sr.color = Color.white;
                return;
            }

            if (CanPlaceGear(grid.SelectedGear))
            {
                sr.color = Color.white * (isHovering ? 1.0f : 0.8f);
                if (isHovering)
                {
                    grid.SelectedGear.sr.color = Color.white * 0.8f;
                    grid.NotifyGearTinted();
                    grid.ShowGhost(this, grid.SelectedGear);
                }
            }
            else
            {
                sr.color = Color.red * (isHovering ? 0.8f : 0.4f);
                if (isHovering)
                {
                    // red and smaller alpha for grid.SelectedGear.sr.color
                    grid.SelectedGear.sr.color = Color.red * 0.65f;
                    grid.NotifyGearTinted();
                }
            }
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
