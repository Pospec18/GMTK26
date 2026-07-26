using System.Collections.Generic;
using UnityEngine;

namespace Pospec
{
    public class LineConnection : MonoBehaviour
    {
        public List<LineGear> gears;
        public LineRenderer lr;

        private void Update()
        {
            DrawLine.DrawLineFromGears(gears, lr);
        }

        /// <summary>Takes the line away and unlinks every gear that was on it.</summary>
        public void Remove()
        {
            foreach (var gear in gears)
            {
                if (gear)
                    gear.RemoveLine(this, gears);
            }

            gears = new List<LineGear>();
            Destroy(gameObject);
        }
    }
}
