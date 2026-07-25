using UnityEngine;

namespace Pospec
{
    public class Grid : MonoBehaviour
    {
        public Cell[,] cells;
        public int maxLayers;
        public Vector2Int size;
        public Cell cellPrefab;
        public LineGear SelectedGear { get; private set; }
        private bool stickIsDeselected;

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

        public void Start()
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
                    cells[x, y] = c;
                }
            }


            /* // placing grids simulation
            // CASE 1: stacked in one cell, shared axle
            LineGear gear1 = SpawnTestGear("gear1", 1.0f, cells[0, 0]);
            gear1.angularSpeed = 1.0f;

            LineGear gear2 = SpawnTestGear("gear2", 0.5f, cells[0, 0]);

            cells[0, 0].TryPlaceGearOnTop(gear1);
            cells[0, 0].TryPlaceGearOnTop(gear2);

            // assert
            Debug.Assert(gear1.angularSpeed == 1.0f, "Gear 1 angular speed should be 1.0f, instead it is " + gear1.angularSpeed);
            Debug.Assert(gear2.angularSpeed == 1.0f, "Gear 2 angular speed should be 1.0f, instead it is " + gear2.angularSpeed);

            ClearGears();

            // CASE 2
            LineGear gear3 = SpawnTestGear("gear3", 0.5f, cells[0, 0]);
            gear3.angularSpeed = 1.0f;

            LineGear gear4 = SpawnTestGear("gear4", 0.5f, cells[1, 0]);

            cells[0, 0].TryPlaceGearOnTop(gear3);
            cells[1, 0].TryPlaceGearOnTop(gear4);

            Debug.Assert(gear3.angularSpeed == 1.0f, "Gear 3 angular speed should be 1.0f, instead it is " + gear3.angularSpeed);
            Debug.Assert(gear4.angularSpeed == -1.0f, "Gear 4 angular speed should be -1.0f, instead it is " + gear4.angularSpeed);

            ClearGears();

            // CASE 3
            LineGear gear5 = SpawnTestGear("gear5", 0.5f, cells[0, 0]);
            gear5.angularSpeed = 1.0f;
            LineGear gear6 = SpawnTestGear("gear6", 1.0f, cells[0, 0]);

            LineGear gear7 = SpawnTestGear("gear7", 0.5f, cells[2, 0]);
            LineGear gear8 = SpawnTestGear("gear8", 1.0f, cells[2, 0]);


            cells[0, 0].TryPlaceGearOnTop(gear5);
            cells[0, 0].TryPlaceGearOnTop(gear6);
            cells[2, 0].TryPlaceGearOnTop(gear7);
            cells[2, 0].TryPlaceGearOnTop(gear8);

            Debug.Assert(gear5.angularSpeed == 1.0f, "Gear 5 angular speed should be 1.0f, instead it is " + gear5.angularSpeed);
            Debug.Assert(gear6.angularSpeed == 1.0f, "Gear 6 angular speed should be 1.0f, instead it is " + gear6.angularSpeed);
            Debug.Assert(gear7.angularSpeed == -1.0f, "Gear 7 angular speed should be 1.0f, instead it is " + gear7.angularSpeed);
            Debug.Assert(gear8.angularSpeed == -1.0f, "Gear 8 angular speed should be 1.0f, instead it is " + gear8.angularSpeed);
             */
        }

        private LineGear SpawnTestGear(string name, float radius, Cell cell)
        {
            var gear = new GameObject(name).AddComponent<LineGear>();
            gear.radius = radius;
            gear.transform.position = cell.transform.position;
            return gear;
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
    }
}
