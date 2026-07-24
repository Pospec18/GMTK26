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

            var timer = Instantiate(spawner.timerPrefab, sticks[sticks.Count - 1].transform);
            timer.link = link;
        }

        private void Update()
        {
            List<Gear> crashedGears = new List<Gear>();
            sticks[0].angularSpeed = statingAngularSpeed;

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

                int numSameOverlaps = 0;
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
                        numSameOverlaps = 1;
                        longestOverlap = overlap;
                        sticks[i].angularSpeed = -g2.radius * sticks[i - 1].angularSpeed / g1.radius;
                        sticks[i].distFromPrev = overlap;
                    }
                    // SYSTEM CRASH
                    else if (overlap == longestOverlap)
                    {
                        numSameOverlaps++;
                    }
                }

                if (numSameOverlaps > 1)
                {
                    sticks[i - 1].angularSpeed = 0;
                    sticks[i].angularSpeed = 0;

                    for (int j = 0; j < Mathf.Min(sticks[i].gears.Count, sticks[i - 1].gears.Count); j++)
                    {
                        var g1 = sticks[i].gears[j];
                        var g2 = sticks[i - 1].gears[j];
                        if (g1 == null || g2 == null)
                            continue;

                        float overlap = g2.radius + g1.radius;
                        if (overlap == longestOverlap)
                        {
                            crashedGears.Add(g1);
                            crashedGears.Add(g2);
                        }
                    }
                }
            }

            Vector3 prevLayoutPos = sticks[0].transform.localPosition;
            sticks[0].UpdateStick(prevLayoutPos);

            for (int i = 1; i < sticks.Count; i++)
            {
                spaces[i].Active(DragManager.instance.SelectedStick() != null && (DragManager.instance.FromColumn() == this ? i != DragManager.instance.SelectedStick().id && i != DragManager.instance.SelectedStick().id + 1 : true) /* && DragManager.instance.FromColumn() != this */);

                Vector3 layoutPos = prevLayoutPos + Vector3.down * sticks[i].distFromPrev;
                spaces[i].Reposition(prevLayoutPos, sticks[i].distFromPrev);
                sticks[i].UpdateStick(layoutPos);
                prevLayoutPos = layoutPos;
            }

            foreach (Gear gear in crashedGears)
                gear.GetComponent<SpriteRenderer>().color = Color.red;

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
            if (s == null) return;

            if (sticks.Contains(s) && s.id < id)
            {
                s.MoveToColumn(this, id - 1);
            }
            else
            {
                s.MoveToColumn(this, id);
            }
        }
    }
}
