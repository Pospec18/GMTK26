using System.Collections.Generic;
using UnityEditorInternal;
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
    }
}
