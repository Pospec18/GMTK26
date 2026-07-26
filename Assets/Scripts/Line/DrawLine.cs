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
            if (!Grid.Instance.lineDrawing)
                return;

            DrawLineFromGears(gears, lr);

            if (Input.GetMouseButtonDown(0))
            {
                lr.SetPosition(0, GetMouseWorldPos());
            }

            int lastId = gears.Count > 0 ? gears.Count * 2 - 1 : 1;

            if (Input.GetMouseButton(0))
            {
                lr.SetPosition(lastId, GetMouseWorldPos());
            }

            if (Input.GetMouseButtonUp(0))
            {
                lr.SetPosition(lastId, GetMouseWorldPos());
                FinalizeLine();
            }
        }

        public static void DrawLineFromGears(List<LineGear> gears, LineRenderer lr)
        {
            lr.positionCount = gears.Count > 0 ? gears.Count * 2 : 2;

            for (int i = 0; i < gears.Count; i++)
            {
                var g1 = gears[i];
                var g2 = gears[(i + 1) % gears.Count];

                Vector3 g1P = g1.transform.position;
                Vector3 g2P = g2.transform.position;
                Vector3 dir = g2P - g1P;

                Vector3 forwardStart = Vector3.forward;
                Vector3 forwardEnd = Vector3.forward;

                if (gears.Count >= 3)
                {
                    // Turn direction around gear i (g1)
                    var p1 = gears[(i - 1 + gears.Count) % gears.Count].transform.position;
                    var p2 = g1P;
                    var p3 = g2P;
                    float dStart = (p2.x - p1.x) * (p3.y - p1.y) - (p2.y - p1.y) * (p3.x - p1.x);
                    if (dStart < 0) forwardStart *= -1;

                    // Turn direction around gear i+1 (g2)
                    var p4 = gears[(i + 2) % gears.Count].transform.position;
                    float dEnd = (p3.x - p2.x) * (p4.y - p2.y) - (p3.y - p2.y) * (p4.x - p2.x);
                    if (dEnd < 0) forwardEnd *= -1;
                }

                Vector3 perpStart = Vector3.Cross(dir, forwardStart).normalized;
                Vector3 perpEnd = Vector3.Cross(dir, forwardEnd).normalized;

                lr.SetPosition(2 * i, g1P + perpStart * g1.radius);
                lr.SetPosition(2 * i + 1, g2P + perpEnd * g2.radius);
            }
        }

        public void AddToLine(LineGear lineGear)
        {
            if (gears.Count > 0 && lineGear == gears[0])
            {
                FinalizeLine();
                return;
            }

            foreach (var gear in gears)
                if (lineGear == gear || lineGear.cell == gear.cell)
                    return;

            gears.Add(lineGear);
        }

        public void FinalizeLine()
        {
            Grid.Instance.lineDrawing = false;
            lr.positionCount = 0;
            if (gears.Count < 2)
            {
                Debug.Log("INVALID");
                gears.Clear();
                return;
            }
            Grid.Instance.CreateLine(gears);
            gears.Clear();
        }

        private Vector3 GetMouseWorldPos() => Camera.main.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 10;
    }
}
