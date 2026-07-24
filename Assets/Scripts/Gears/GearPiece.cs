using System.Collections.Generic;
using UnityEngine;

namespace Pospec
{
    [CreateAssetMenu()]
    public class GearPiece : ScriptableObject
    {
        public List<GearSize> gears;
    }

    public enum GearSize
    {
        None = 0,
        Tiny = 1,
        Small = 2,
        Medium = 3,
        Large = 4
    }
}
