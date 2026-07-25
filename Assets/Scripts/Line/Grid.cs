using UnityEngine;

namespace Pospec
{
    public class Grid : MonoBehaviour
    {
        public Cell[,] cells;
        public int maxLayers;
        public Vector2Int size;
        public Cell cellPrefab;

        public void Start()
        {
            Vector3 offset = transform.position - new Vector3(size.x, size.y) / 2.0f;
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
        }
    }
}
