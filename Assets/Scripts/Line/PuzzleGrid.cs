using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

namespace Pospec
{
    public class PuzzleGrid : MonoBehaviour
    {
        public List<LineGear> startingGears;
        public Grid grid;
        public Vector2Int gridSize;
        public TmpCell cellPrefab;
        [SerializeField, HideInInspector] private List<TmpCell> cells;

        private void OnValidate()
        {
            if (grid)
            {
                grid.size = gridSize;
            }
        }

        [ContextMenu("Regenerate")]
        private void GenerateCells()
        {
            if (cells == null)
                cells = new List<TmpCell>();

            foreach (var cell in cells)
                if (cell != null)
                    DestroyImmediate(cell.gameObject);
            cells.Clear();

            Vector3 offset = transform.position - new Vector3(gridSize.x - 1, gridSize.y - 1) / 2.0f;
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    TmpCell cell = Instantiate(cellPrefab, new Vector3(x, y) + offset, Quaternion.identity, transform);
                    cell.Setup(new Vector2Int(x, y));
                    cells.Add(cell);
                }
            }
        }

        public void Awake()
        {
            grid.Setup(cells);
        }

        private void Update()
        {
            foreach (var gear in startingGears)
            {
                gear.UpdateAngularSpeed(gear);
            }
        }
    }
}
