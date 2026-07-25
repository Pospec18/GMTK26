using System.Collections.Generic;
using UnityEngine;

namespace Pospec
{
    public class PuzzleGrid : MonoBehaviour
    {
        public List<LineGear> startingGears;

        private void Update()
        {
            foreach (var gear in startingGears)
            {
                gear.UpdateAngularSpeed(gear);
            }
        }
    }
}
