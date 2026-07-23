using System.Collections.Generic;
using UnityEngine;

namespace Pospec
{
    public class Column : MonoBehaviour
    {
        public List<Gear> gears;
        public float statingAngularSpeed;
        public Link link;

        public void Update()
        {
            gears[0].angularSpeed = statingAngularSpeed;
            for (int i = 1; i < gears.Count; i++)
                gears[i].angularSpeed = -gears[i - 1].surfaceSpeed() / gears[i].radius;
            link.value = gears[gears.Count - 1].rotationNormalized();
        }
    }
}
