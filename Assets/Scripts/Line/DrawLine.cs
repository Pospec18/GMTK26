using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Pospec
{
    public class DrawLine : MonoBehaviour
    {
        public LineRenderer lr;
        public List<LineGear> gears;

        public void Start()
        {
            lr.positionCount = 2;
        }

        public void Update()
        {
            lr.positionCount = gears.Count > 0 ? gears.Count * 2 : 2;

            if (Input.GetMouseButtonDown(0))
            {
                lr.SetPosition(0, GetMouseWorldPos());
            }

            for (int i = 0; i < gears.Count; i++)
            {
                var g1 = gears[i];
                var g2 = gears[(i + 1) % gears.Count];

                var g1P = g1.transform.position;
                var g2P = g2.transform.position;
                Vector3 dir = g2P - g1P;
                Vector3 forward = Vector3.forward;
                if (gears.Count >= 3)
                {
                    var pg2 = gears[(i - 1 + gears.Count) % gears.Count].transform.position;
                    var pg3 = gears[(i - 2 + gears.Count) % gears.Count].transform.position;
                    float d = (pg2.x - g1P.x) * (pg3.x - g1P.x) - (pg2.y - g1P.y) * (pg3.x - g1P.x);
                    if (d < 0)
                        forward *= -1;
                }
                Vector3 perp = Vector3.Cross(dir, forward).normalized;

                lr.SetPosition(2 * i, g1P + perp * g1.radius);
                lr.SetPosition(2 * i + 1, g2P + perp * g2.radius);
            }

            int lastId = gears.Count > 0 ? gears.Count * 2 - 1 : 1;

            if (Input.GetMouseButton(0))
            {
                lr.SetPosition(lastId, GetMouseWorldPos());
            }

            if (Input.GetMouseButtonUp(0))
            {
                lr.SetPosition(lastId, GetMouseWorldPos());
            }
        }

        public void AddToLine(LineGear lineGear)
        {
            if (!gears.Contains(lineGear))
                gears.Add(lineGear);
        }

        private Vector3 GetMouseWorldPos() => Camera.main.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 10;
    }
}
