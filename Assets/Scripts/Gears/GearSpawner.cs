using System.Collections.Generic;
using UnityEngine;

namespace Pospec
{
    [CreateAssetMenu()]
    public class GearSpawner : ScriptableObject
    {
        public Gear gearPrefab;
        public Stick stickPrefab;
        public DragArea dragAreaPrefab;
        public TimerViz timerPrefab;
        public List<Sprite> bottomGearSprites;
        public List<Sprite> topGearSprites;
        public List<float> gearSizeToRadius;

        public Stick SpawnStick(List<GearSize> pieces)
        {
            Stick stick = Instantiate(stickPrefab);
            stick.pointer.radius = 0;
            for (int i = 0; i < pieces.Count; i++)
            {
                GearSize size = pieces[i];
                int iSize = (int)size;
                if (size == GearSize.None)
                {
                    stick.gears.Add(null);
                    continue;
                }
                Gear g = Instantiate(gearPrefab, stick.transform);
                g.radius = gearSizeToRadius[iSize];
                g.transform.localScale = 2 * g.radius * Vector3.one;
                g.sr.sprite = i == 0 ? bottomGearSprites[iSize] : topGearSprites[iSize];
                stick.gears.Add(g);
                if (g.radius > stick.pointer.radius)
                    stick.pointer.radius = g.radius;
            }
            return stick;
        }
    }
}
