using System.Collections.Generic;
using UnityEngine;

namespace Pospec
{
    public class Column : MonoBehaviour
    {
        public List<GearPiece> initialPieces;
        public float statingAngularSpeed;
        public Link link;
        public GearSpawner spawner;
        private List<Stick> sticks = new List<Stick>();

        private void Start()
        {
            foreach (var piece in initialPieces)
            {
                Stick stick = spawner.SpawnStick(piece.gears);
                stick.transform.parent = transform;
                stick.gameObject.name = piece.name;
                stick.transform.localPosition = Vector3.zero;
                sticks.Add(stick);
            }
        }

        private void Update()
        {
            sticks[0].angularSpeed = statingAngularSpeed;
            sticks[0].UpdateStick(null);
            for (int i = 1; i < sticks.Count; i++)
            {
                sticks[i].angularSpeed = 0;
                float maxR1 = 0;
                float maxR2 = 0;
                foreach (Gear gear in sticks[i].gears)
                    if (gear != null)
                        maxR1 = Mathf.Max(gear.radius, maxR1);
                foreach (Gear gear in sticks[i - 1].gears)
                    if (gear != null)
                        maxR2 = Mathf.Max(gear.radius, maxR2);

                sticks[i].distFromPrev = maxR1 + maxR2 + 0.1f;

                float longestOverlap = 0;
                for (int j = 0; j < Mathf.Min(sticks[i].gears.Count, sticks[i - 1].gears.Count); j++)
                {
                    var g1 = sticks[i].gears[j];
                    var g2 = sticks[i - 1].gears[j];
                    if (g1 == null || g2 == null)
                        continue;

                    float overlap = g2.radius + g1.radius;
                    if (overlap > longestOverlap)
                    {
                        longestOverlap = overlap;
                        sticks[i].angularSpeed = (-g2.radius * sticks[i - 1].angularSpeed / g1.radius);
                        sticks[i].distFromPrev = overlap;
                    }
                }
                sticks[i].UpdateStick(sticks[i - 1]);
            }
            link.value = sticks[sticks.Count - 1].rotationNormalized();
        }
    }
}
