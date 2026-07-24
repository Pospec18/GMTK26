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
        private List<DragArea> spaces = new List<DragArea>();

        public int LastStickId => sticks.Count - 1;

        private void Start()
        {
            spaces.Add(null);
            for (int i = 0; i < initialPieces.Count; i++)
            {
                Stick stick = spawner.SpawnStick(initialPieces[i].gears);
                stick.transform.parent = transform;
                stick.gameObject.name = initialPieces[i].name;
                stick.transform.localPosition = Vector3.zero;
                stick.Setup(this, i);
                sticks.Add(stick);

                if (i == initialPieces.Count - 1)
                    break;

                var s = Instantiate(spawner.dragAreaPrefab, transform);
                s.Setup(this, i + 1);
                spaces.Add(s);
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
                spaces[i].Active(DragManager.instance.SelectedStick() != null && DragManager.instance.FromColumn() != this);
                spaces[i].Reposition(sticks[i - 1].transform.localPosition, sticks[i].distFromPrev);
                sticks[i].UpdateStick(sticks[i - 1]);
            }
            link.value = sticks[sticks.Count - 1].rotationNormalized();
        }

        public void RemoveAt(int id)
        {
            sticks.RemoveAt(id);
            var lastSpace = spaces[spaces.Count - 1];
            spaces.RemoveAt(spaces.Count - 1);
            Destroy(lastSpace.gameObject);
            for (int i = 0; i < sticks.Count; i++)
                sticks[i].Setup(this, i);

            for (int i = 1; i < spaces.Count; i++)
                spaces[i].Setup(this, i);
        }

        public void AddAt(Stick s, int id)
        {
            sticks.Insert(id, s);
            spaces.Add(Instantiate(spawner.dragAreaPrefab, transform));
            for (int i = 0; i < sticks.Count; i++)
                sticks[i].Setup(this, i);

            for (int i = 1; i < spaces.Count; i++)
                spaces[i].Setup(this, i);
        }

        public void Swap(int id)
        {
            Stick s = DragManager.instance.SelectedStick();
            if (s != null)
                s.MoveToColumn(this, id);
        }
    }
}
