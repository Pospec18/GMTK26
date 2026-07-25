using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Pospec
{
    public class Cell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public List<LineGear> gears = new List<LineGear>();
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
            // dropping a gear back where it already is would add it to this cell twice
            if (gear.cell == this)
            {
                return false;
            }

            // this cell can only have maxLayers gears
            if (gears.Count >= grid.maxLayers)
            {
                Debug.Log("Cell " + pos + " is full, cannot place gear on top");
                return false;
            }

            // we need to check with every already placed gear in the grid if it is not colliding
            foreach (var cell in grid.cells)
            {
                if (cell == this)
                    continue;

                foreach (var g in cell.gears)
                {
                    if (AreGearsColliding(gear, g, this))
                    {
                        Debug.Log("Gear is colliding with another gear in cell " + cell.pos + ", cannot place gear on top");
                        return false;
                    }
                }
            }

            // okay lets place it then bro
            gear.SetLevel(gears.Count);
            gears.Add(gear);
            gear.SetCell(this);

            return true;
        }

        public void RemoveTopGear()
        {
            if (gears.Count == 0)
            {
                return;
            }

            gears.RemoveAt(gears.Count - 1);
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

        private bool AreGearsColliding(LineGear thisGear, LineGear otherGear, Cell thisCell)
        {
            if (!thisGear || !otherGear)
                return false;

            if (thisGear == otherGear)
                return false;

            if (thisGear.GetLevel() != otherGear.GetLevel())
                return false;

            float distance = Vector3.Distance(thisCell.transform.position, otherGear.cell.transform.position);

            // gears may overlap slightly before we call it a collision
            if (distance < thisGear.radius + otherGear.radius - grid.graceCollisionOffset)
                return true;

            return false;
        }

        private bool AreGearsRotatingTogether(LineGear thisGear, LineGear otherGear)
        {
            if (!thisGear || !otherGear)
                return false;

            if (thisGear == otherGear)
                return false;

            // gears stacked in the same cell sit on a shared axle, so they always turn
            // together
            if (thisGear.ShareSameCell(otherGear))
                return true;

            if (thisGear.GetLevel() != otherGear.GetLevel())
                return false;

            if (!thisGear.cell || !otherGear.cell)
                return false;

            float distance = Vector3.Distance(thisGear.cell.transform.position, otherGear.cell.transform.position);
            float touchDistance = thisGear.radius + otherGear.radius;

            // gears drive each other only when their teeth meet. too far apart and they
            // never touch, too close and they are overlapping
            return distance <= touchDistance + grid.graceCollisionOffset
                && distance >= touchDistance - grid.graceCollisionOffset;
        }

        private List<LineGear> GetGearsRotatingTogether()
        {
            List<LineGear> result = new List<LineGear>();
            LineGear addedGear = gears[gears.Count - 1];

            foreach (var cell in grid.cells)
            {
                foreach (var gear in cell.gears)
                {
                    if (AreGearsRotatingTogether(gear, addedGear))
                    {
                        result.Add(gear);
                    }
                }
            }

            return result;
        }

        private void LinkGears(LineGear addedGear)
        {
            // we need to check if the gear is touching any other gear in the grid and link them if they are

            // first we find if we can find a touching gear that is already spinning 
            List<LineGear> touchingGears = GetGearsRotatingTogether();

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

            Debug.Log("Cell " + pos + " has " + touchingGears.Count + " touching gears, of which " + spinningGears.Count + " are spinning");

            if (spinningGears.Count > 1)
            {
                // TODO: JAMMING, maybe if same speed this could work
                Debug.LogError("HOLY FUCKING SHIT WERE ALL GONNA DIE");
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
            if (!grid.SelectedGear)
            {
                sr.color = Color.white;
                return;
            }

            if (Input.GetMouseButtonUp(0) && isHovering)
            {
                isHovering = false;
                Cell oldCell = grid.SelectedGear.cell;
                if (TryPlaceGearOnTop(grid.SelectedGear))
                {
                    if (oldCell)
                    {
                        oldCell.RemoveTopGear();
                    }
                    // it is not touching what it used to touch anymore
                    grid.SelectedGear.ClearConnections();
                    LinkGears(grid.SelectedGear);

                    grid.SelectedGear.PlaceToCell(this);
                }
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
