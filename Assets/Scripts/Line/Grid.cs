using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pospec
{
    public class Grid : MonoBehaviour
    {
        public Cell[,] cells;
        public int maxLayers = 10;
        [HideInInspector] public Vector2Int size;
        public Cell cellPrefab;
        public LineGear SelectedGear { get; private set; }
        private bool stickIsDeselected;
        private bool gearTintedThisFrame;
        public float hoverPadding = 0.05f;
        public bool lineDrawing;
        public DrawLine lineDrawer;


        [Header("Drop preview")]
        public Color ghostColor = new Color(1.0f, 1.0f, 1.0f, 0.4f);
        private SpriteRenderer ghost;
        private bool ghostShownThisFrame;

        /// <summary>Called by the hovered cell when it tints the selected gear this frame.</summary>
        public void NotifyGearTinted()
        {
            gearTintedThisFrame = true;
        }

        /// <summary>Shows a preview of the dragged gear where it would land if it was dropped on this cell.</summary>
        public void ShowGhost(Cell cell, LineGear gear)
        {
            if (!cell || !gear || !gear.sr)
                return;

            if (!ghost)
            {
                var go = new GameObject("GearGhost");
                ghost = go.AddComponent<SpriteRenderer>();
            }

            int idx = cell.gears.Count;
            ghost.sprite = gear.sr.sprite;
            ghost.color = ghostColor;
            ghost.sortingLayerID = SortingLayer.layers[gear.normalSortingLayer].id;
            ghost.sortingOrder = idx;
            ghost.transform.position = cell.transform.position + Vector3.forward * idx;
            ghost.transform.rotation = gear.transform.rotation;
            ghost.transform.localScale = gear.FullWorldScale;
            ghost.enabled = true;

            ghostShownThisFrame = true;
        }

        public static Grid Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            Instance = null;

            if (ghost)
                Destroy(ghost.gameObject);
        }

        public void SelectGear(LineGear gear)
        {
            SelectedGear = gear;
        }

        public void DeselectGear()
        {
            stickIsDeselected = true;
        }

        public float graceCollisionOffset;

        public void ClearGears()
        {
            if (cells == null)
                return;

            foreach (var cell in cells)
            {
                if (cell != null)
                {
                    cell.ClearGears();
                }
            }
        }

        public void GenerateLevel()
        {
            Vector3 offset = transform.position - new Vector3(size.x - 1, size.y - 1) / 2.0f;
            cells = new Cell[size.x, size.y];
            for (int y = 0; y < size.y; y++)
            {
                for (int x = 0; x < size.x; x++)
                {
                    Vector2Int pos = new Vector2Int(x, y);
                    var c = Instantiate(cellPrefab, new Vector3(x, y) + offset, Quaternion.identity, transform);
                    c.Setup(pos, this);
                    c.gameObject.transform.localPosition = new Vector3(x, y) + offset;
                    cells[x, y] = c;
                }
            }
        }

        private void LateUpdate()
        {
            // no cell claimed the gear this frame, so it is being dragged off the
            // grid - put it back to its untinted look
            if (SelectedGear && !gearTintedThisFrame)
                SelectedGear.sr.color = Color.white;
            gearTintedThisFrame = false;

            // no cell offered a valid drop spot this frame, so the gear would go back
            // where it came from - preview that instead
            if (!ghostShownThisFrame && SelectedGear && !SelectedGear.cell && SelectedGear.OriginCell)
                ShowGhost(SelectedGear.OriginCell, SelectedGear);

            if (ghost && !ghostShownThisFrame)
                ghost.enabled = false;
            ghostShownThisFrame = false;

            if (stickIsDeselected)
            {
                stickIsDeselected = false;
                SelectedGear = null;
            }
        }

        public void Setup(List<TmpCell> cells)
        {
            GenerateLevel();
            foreach (var cell in cells)
            {
                Cell target = this.cells[cell.pos.x, cell.pos.y];
                target.cellType = cell.cellType;

                foreach (var item in cell.gears)
                {
                    target.TryPlaceGearOnTop(item);
                    target.LinkGears(item);
                }
            }
        }

        public void AddToLine(LineGear lineGear)
        {
            lineDrawer.AddToLine(lineGear);
        }
    }
}
