using System.Collections.Generic;
using UnityEngine;

namespace Pospec
{
    public class PuzzleGrid : MonoBehaviour
    {
        public List<LineGear> startingGears;
        public LineGear endGear;
        public float winAngularSpeed;
        public float winMarginOfError = 1;
        public Grid grid;
        public Vector2Int gridSize;
        public TmpCell cellPrefab;
        public RotationViz winConViz;
        [SerializeField, HideInInspector] private List<TmpCell> cells;
        public LevelFinisher levelFinisher;
        public LineCanvas lineCanvas;
        public List<float> lines;

        public static PuzzleGrid instance;

        private LineGear[] allGears;

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
            // same order as Grid.GenerateLevel, so both hierarchies read the same way
            for (int y = 0; y < gridSize.y; y++)
            {
                for (int x = 0; x < gridSize.x; x++)
                {
                    TmpCell cell = Instantiate(cellPrefab, new Vector3(x, y) + offset, Quaternion.identity, transform);
                    cell.Setup(new Vector2Int(x, y));
                    cells.Add(cell);
                }
            }
        }

        public void Awake()
        {
            instance = this;

            grid.Setup(cells);

            allGears = FindObjectsByType<LineGear>(FindObjectsSortMode.None);
            if (winConViz)
            {
                winConViz.angularSpeed = winAngularSpeed;
                winConViz.GetComponent<Follower>().target = endGear.transform;
            }
            if (lineCanvas)
                lineCanvas.Setup(lines);
        }

        public void OnDestroy()
        {
            instance = null;
        }

        private void Update()
        {
            foreach (var gear in allGears)
            {
                if (startingGears.Contains(gear)) continue;

                gear.angularSpeed = 0.0f;
            }

            foreach (var gear in startingGears)
            {
                gear.UpdateAngularSpeed(gear);
            }

            if (endGear && Mathf.Abs(endGear.angularSpeed - winAngularSpeed) < winMarginOfError)
            {
                Debug.Log("WIN");
                if (levelFinisher)
                {
                    levelFinisher.FinishLevel();
                }
            }
        }

        public List<LineGear> GetStartingGears()
        {
            return startingGears;
        }
    }
}
