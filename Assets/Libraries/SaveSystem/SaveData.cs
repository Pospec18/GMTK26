using System;
using UnityEngine;

namespace Pospec.Saving
{
    [Serializable]
    public class SaveData
    {
        [HideInInspector] public int version;

        public SaveData(int version)
        {
            this.version = version;
        }
    }
}
