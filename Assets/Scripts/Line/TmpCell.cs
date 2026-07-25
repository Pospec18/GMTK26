using System.Collections.Generic;
using UnityEngine;

namespace Pospec
{
    public class TmpCell : MonoBehaviour
    {
        public List<LineGear> gears;
        [HideInInspector] public Vector2Int pos;

        private void OnValidate()
        {
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
