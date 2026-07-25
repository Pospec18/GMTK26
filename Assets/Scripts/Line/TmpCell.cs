using System.Collections.Generic;
using UnityEngine;

namespace Pospec
{
    public class TmpCell : MonoBehaviour
    {
        public List<LineGear> gears;
        [HideInInspector] public Vector2Int pos;


        public enum CellType
        {
            Placeable,
            Hole,
            Obstacle
        };

        public CellType cellType = CellType.Placeable;

        private void OnValidate()
        {
            // purely a level design aid - this object is destroyed on Start, so the
            // tint never shows up in play mode
            var sr = GetComponent<SpriteRenderer>();
            if (sr)
            {
                sr.color = cellType switch
                {
                    CellType.Hole => Color.black,
                    CellType.Obstacle => Color.red,
                    _ => Color.white,
                };
            }

            for (int i = 0; i < gears.Count; i++)
            {
                if (gears[i] != null)
                {
                    gears[i].transform.position = transform.position;
                    gears[i].sr.sortingOrder = i;
                }
            }
        }

        public void Start()
        {
            Destroy(gameObject);
        }

        public void Setup(Vector2Int pos)
        {
            this.pos = pos;
            gameObject.name = $"Cell ({pos.x}, {pos.y})";
        }
    }
}
