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
            lr.positionCount = gears.Count * 2;
            for (int i = 0; i < gears.Count; i++)
            {
                var g1 = gears[i];
                var g2 = gears[(i + 1) % gears.Count];

                var g1P = g1.transform.position;
                var g2P = g2.transform.position;
                Vector3 dir = g2P - g1P;
                Vector3 perp = Vector3.Cross(dir, Vector3.forward).normalized;

                lr.SetPosition(2 * i, g1P + perp * g1.radius);
                lr.SetPosition(2 * i + 1, g2P + perp * g2.radius);
            }
        }
    }
}
