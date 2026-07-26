using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Pospec
{
    public class LineGear : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public float angularSpeed;
        public float radius;
        public bool canMove;

        private int level = -1;

        public List<LineGear> connectedTo = new List<LineGear>();

        public ConnectionType connectionToParent = ConnectionType.None;

        /// <summary>The line this gear is a part of, if any. Set by Grid.CreateLine.</summary>
        public LineConnection line;

        public Cell cell;
        public CircleCollider2D col;
        private LineGear parent = null;
        public SpriteRenderer sr;

        public int normalSortingLayer;
        public int holdingSortingLayer;

        // where the gear sits relative to the cursor while dragged, in world units. the gear
        // is always the same size in our hand, so this does not scale with its radius.
        // kept out of the inspector so the values here are the ones that actually apply
        private static readonly Vector2 dragOffset = new Vector2(0.42f, -0.42f);

        // the gear in our hand is always this big, in world units of radius, no matter how
        // big the gear itself is - a small gear and a huge one look the same while dragged
        private const float dragRadius = 0.25f;

        public bool isDragging;
        private bool placedThisFrame;
        // was the cursor over a cell when the gear was released?
        private bool droppedOnGrid;
        private Vector3 grabOffset;
        private Camera dragCamera;
        private Cell originCell;
        private Grid dragGrid;
        private Vector3 baseScale = Vector3.one;
        private HoverHighlight hover;
        private bool initialized;
        private bool isHovered;

        // where the gear started the level. gears that are not listed in any TmpCell never get a
        // cell, so this is the only home they have to fall back to
        private Vector3 homePosition;

        /// <summary>The cell this gear was picked up from, so it can be dropped back there. Null while not dragging.</summary>
        public Cell OriginCell => originCell;

        /// <summary>World scale of the gear at full size, so a preview is not shrunk along with the dragged gear.</summary>
        public Vector3 FullWorldScale =>
            transform.parent ? Vector3.Scale(transform.parent.lossyScale, baseScale) : baseScale;

        // radius is measured at the gear's normal scale, so this lands every gear on the same
        // dragRadius regardless of how big it started
        private Vector3 DragLocalScale =>
            radius > 0.0f ? baseScale * (dragRadius / radius) : baseScale;

        private void Awake()
        {
            Init();
        }

        // the grid places the authored gears from its own Awake, which may run before ours, so
        // anything it needs has to be ready on demand
        private void Init()
        {
            if (initialized)
                return;

            initialized = true;
            baseScale = transform.localScale;
            homePosition = transform.position;
            hover = GetComponent<HoverHighlight>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (Grid.Instance.lineDrawing)
            {
                return;
            }

            if (!canMove)
                return;

            // the line stops describing reality the moment any of its gears moves, so it goes
            // away along with everything it was driving
            if (line)
                line.Remove();

            isDragging = true;
            dragGrid = cell ? cell.grid : FindAnyObjectByType<Grid>();
            dragGrid.SelectGear(this);

            // the gear leaves the grid as soon as we grab it, so it is not driven by
            // anything while it is in our hand
            originCell = cell;
            DetachFromGrid();

            dragCamera = eventData.pressEventCamera != null ? eventData.pressEventCamera : Camera.main;

            // the gear hangs to the bottom right of the cursor, no matter where it was
            // grabbed, so the cursor never sits under the gear and hides the cell below it
            grabOffset = new Vector3(dragOffset.x, dragOffset.y, 0.0f);

            Color c = sr.color;
            c.a = 0.35f;
            sr.color = c;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (Grid.Instance.lineDrawing)
            {
                return;
            }

            if (!canMove)
                return;

            Grid grid = dragGrid ? dragGrid : (cell ? cell.grid : FindAnyObjectByType<Grid>());
            grid.DeselectGear();

            EndDrag();
        }

        /// <summary>Ends the drag. Safe to call more than once, and from anywhere - the grid also
        /// calls it on mouse up, because our collider is off while dragging and the pointer up
        /// event is not guaranteed to reach us.</summary>
        public void EndDrag()
        {
            if (!isDragging)
                return;

            isDragging = false;
            // the actual drop is resolved in LateUpdate, after every cell had its chance to
            // claim the gear this frame
            placedThisFrame = true;

            // released over a cell that refuses the gear means an invalid drop, and the gear
            // goes home. released off the grid it just stays in our hand's last position
            Grid grid = dragGrid ? dragGrid : Grid.Instance;
            droppedOnGrid = grid && grid.HoveredCell;

            transform.localScale = baseScale;

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

        /// <summary>Drops everything this gear got from a line that is being removed: the
        /// connections to the other gears on it, and the rotation it took from it.</summary>
        public void RemoveLine(LineConnection removedLine, List<LineGear> lineGears)
        {
            if (line == removedLine)
                line = null;

            foreach (var other in lineGears)
            {
                if (other && other != this)
                    connectedTo.Remove(other);
            }

            // only gears the line was driving stop - the one that drove it keeps its own speed
            if (connectionToParent != ConnectionType.Line)
                return;

            parent = null;
            angularSpeed = 0.0f;
            connectionToParent = ConnectionType.None;

            foreach (var connection in new List<LineGear>(connectedTo))
            {
                if (connection)
                    connection.StopDrivenBy(this);
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
            // no rule checking - the gear was already sitting here, so it has to fit here.
            // asking first would strand the gear outside the grid whenever the spot it came
            // from does not satisfy the drop rules (authored layouts skip them)
            originCell.PlaceGearOnTop(this);
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

        // gears on the upper layer are nudged up-right, scaled up and lightened a touch, so the
        // stack reads as two layers instead of one gear hiding another
        private const float pixelsPerUnit = 300.0f; // the art's import setting
        private const float upperLayerOffset = 0.0f / pixelsPerUnit; // 4 px
        // grown by a fixed number of pixels of radius, not by a factor, so a small gear and a big
        // one stick out by the same amount
        private const float upperLayerGrowth = 28f / pixelsPerUnit;
        private const float upperLayerTint = 1.4f;
        private const float upperLayerAlpha = 0.9f;

        private const float lowerLayerTint = 0.9f;

        private bool IsUpperLayer => level >= 1;

        // only darkened while something is actually stacked on top - a lone gear is not the
        // bottom of anything
        private bool IsLowerLayer => level == 0 && cell;

        // radius is measured at the gear's normal scale, so this turns the fixed pixel growth
        // into the factor that produces it for this particular gear
        private float UpperLayerScale =>
            radius > 0.0f ? (radius + upperLayerGrowth) / radius : 1.0f;

        public void PlaceToCell(Cell cell)
        {
            Init();

            this.cell = cell;
            int idx = cell.gears.IndexOf(this);
            sr.sortingOrder = idx;
            transform.position = cell.transform.position + Vector3.forward * idx
                + (IsUpperLayer ? new Vector3(upperLayerOffset, upperLayerOffset, 0.0f) : Vector3.zero);
            Vector3 scale = IsUpperLayer ? baseScale * UpperLayerScale : baseScale;
            transform.localScale = scale;

            // the hover pop tweens from the scale it cached in Awake, so it has to learn the new
            // one or it would tween the gear straight back to single-layer size
            if (hover)
                hover.SetBaseScale(scale);
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

            // derived from the pair, not from the stored field: a gear that was relinked while
            // nothing around it was spinning never got the field assigned (LinkGears only sets it
            // when it finds a spinning driver), so it would keep the type from wherever it sat
            // before - and PuzzleGrid re-propagates speed through these every frame
            if (parent != this)
            {
                if (connectionToParent != ConnectionType.Line)
                {
                    connectionToParent = ShareSameCell(parent) ? ConnectionType.Stick : ConnectionType.Teeth;
                }
            }


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
            // the puzzle is gone while the scene is being torn down, but gears keep ticking
            // for the rest of the frame
            PuzzleGrid puzzle = PuzzleGrid.instance;
            if (puzzle == null || Grid.Instance == null)
                return;

            if ((puzzle.GetStartingGears().Contains(this) || puzzle.endGear == this) && !Grid.Instance.lineDrawing)
            {
                col.enabled = false;
            }
            else
            {
                col.enabled = Grid.Instance.SelectedGear == null;
                if (col.enabled && cell)
                    col.enabled = cell.gears[cell.gears.Count - 1] == this; // only top gear can be moved
            }

            // a gear whose collider went off under the cursor never gets its exit event, so it
            // would stay lit forever
            if (!col.enabled)
                isHovered = false;

            if (isDragging)
            {
                sr.sortingLayerID = SortingLayer.layers[holdingSortingLayer].id;
                FollowPointer();

                // HoverHighlight tweens the scale in Update while we are dragging, so we have
                // to claim it back every frame. only shrunk while over a cell, where the small
                // gear keeps the cell below it visible - off the grid it shows its real size
                Grid grid = dragGrid ? dragGrid : Grid.Instance;
                transform.localScale = (grid && grid.HoveredCell) ? DragLocalScale : baseScale;
            }
            else
                sr.sortingLayerID = SortingLayer.layers[normalSortingLayer].id;

            // the second condition is a safety net: a gear that is not being dragged and has no
            // cell is stranded outside the grid, no matter which frame it happened on
            if (placedThisFrame || (!isDragging && cell == null && originCell != null))
            {
                placedThisFrame = false;

                // the drop landed on a cell that would not take the gear, so it goes back where
                // it came from. dropped off the grid it stays where we let go of it
                if (cell == null && droppedOnGrid)
                {
                    if (originCell != null)
                        ReturnToOriginCell();
                    else
                        transform.position = homePosition;
                }

                originCell = null;
                if (cell != null)
                    PlaceToCell(cell);
            }

            // while dragged, the hovered cell owns our tint (white when it fits, red when it
            // does not), so the idle dimming must not overwrite it
            if (!isDragging)
            {
                bool idle = Mathf.Abs(angularSpeed) < 0.05f;

                float shade = idle ? 0.85f : 1.0f;
                if (IsUpperLayer)
                    shade = Mathf.Min(shade * upperLayerTint, 1.0f);
                else if (IsLowerLayer)
                    shade *= lowerLayerTint;

                Color targetTint = new Color(shade, shade, shade);
                // the gear under the cursor is always solid, so hovering it pulls it out of the
                // stack instead of leaving the one below showing through
                targetTint.a = isHovered ? 1.0f : (IsUpperLayer ? upperLayerAlpha : sr.color.a);

                sr.color = targetTint;
            }

            // spin smoothly instead of in DiscreteTime's per-cycle pulses
            var dt = DiscreteTime.instance;
            transform.Rotate(Vector3.forward * angularSpeed * Time.deltaTime * dt.timeSpeed * dt.timeStep);
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


            // UpdateAngularSpeed(parent);

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

        public void OnPointerEnter(PointerEventData eventData)
        {
            isHovered = true;

            if (Grid.Instance.lineDrawing)
            {
                Grid.Instance.AddToLine(this);
                return;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isHovered = false;

            if (Grid.Instance.lineDrawing)
            {
                return;
            }
        }
    }

    public enum ConnectionType { Stick, Line, Teeth, None };
}
