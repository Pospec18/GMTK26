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
                Vector3 forwardStart = Vector3.forward;
                Vector3 forwardEnd = Vector3.forward;
                if (gears.Count >= 3)
                {
                    var p1 = gears[(i - 2 + gears.Count) % gears.Count].transform.position;
                    var p2 = gears[(i - 1 + gears.Count) % gears.Count].transform.position;
                    var p3 = gears[(i) % gears.Count].transform.position;
                    float d = (p2.x - p1.x) * (p3.y - p1.y) - (p2.y - p1.y) * (p3.x - p1.x);
                    if (d < 0)
                        forwardEnd *= -1;
                }

                if (gears.Count >= 3)
                {
                    var p1 = gears[(i - 1 + gears.Count) % gears.Count].transform.position;
                    var p2 = gears[(i) % gears.Count].transform.position;
                    var p3 = gears[(i + 1) % gears.Count].transform.position;
                    float d = (p2.x - p1.x) * (p3.y - p1.y) - (p2.y - p1.y) * (p3.x - p1.x);
                    if (d < 0)
                        forwardStart *= -1;
                }

                Vector3 perpStart = Vector3.Cross(dir, forwardStart).normalized;
                Vector3 perpEnd = Vector3.Cross(dir, forwardEnd).normalized;

                lr.SetPosition(2 * i, g1P + perpStart * g1.radius);
                lr.SetPosition(2 * i + 1, g2P + perpEnd * g2.radius);
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
