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

        public static Grid Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            Instance = null;
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

        public bool DidWin()
        {
            return false;
        }

        private void LateUpdate()
        {
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
                foreach (var item in cell.gears)
                {
                    this.cells[cell.pos.x, cell.pos.y].TryPlaceGearOnTop(item);
                    this.cells[cell.pos.x, cell.pos.y].LinkGears(item);
                }
            }
        }
    }
}
