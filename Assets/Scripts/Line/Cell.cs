using System.Collections.Generic;
using UnityEngine;

namespace Pospec
{
    public class Cell : MonoBehaviour
    {
        public List<LineGear> gear = new List<LineGear>();
        public Vector2Int pos { get; private set; }
        private Grid grid;

        public void Setup(Vector2Int pos, Grid grid)
        {
            this.pos = pos;
            this.grid = grid;
        }

        public bool TryPlaceGearOnTop(LineGear gear)
        {
            return true;
        }

        public void RemoveTopGear()
        {

        }
    }
}
