using System.Collections.Generic;
using UnityEngine;

namespace Pospec
{
    [CreateAssetMenu()]
    public class GearSpawner : ScriptableObject
    {
        public Gear gearPrefab;
        public Stick stickPrefab;
        public List<Sprite> bottomGearSprites;
        public List<Sprite> topGearSprites;
        public List<float> gearSizeToRadius;

        public Stick SpawnStick(List<GearSize> pieces)
        {
            Stick stick = Instantiate(stickPrefab);
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
                g.transform.localScale = 2 * gearSizeToRadius[iSize] * Vector3.one;
                g.sr.sprite = i == 0 ? bottomGearSprites[iSize] : topGearSprites[iSize];
                stick.gears.Add(g);
            }
            return stick;
        }
    }
}
