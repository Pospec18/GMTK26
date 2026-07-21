using UnityEngine;

namespace Pospec.ScreenSettings
{
    public class AspectRatio
    {
        public uint Width { get; set; }
        public uint Height { get; set; }

        public AspectRatio(Resolution resolution) : this((uint)resolution.width, (uint)resolution.height) { }

        public AspectRatio(uint width, uint height)
        {
            uint div = GCD(width, height);
            Width = width / div;
            Height = height / div;
        }

        public bool CorrespondsTo(Resolution resolution)
        {
            return resolution.width / (int)Width == resolution.height / (int)Height;
        }

        public float ToFloat()
        {
            return Width / (float)Height;
        }

        public override string ToString()
        {
            return Width.ToString() + " x " + Height.ToString();
        }

        private static uint GCD(uint a, uint b)
        {
            while (a != 0 && b != 0)
            {
                if (a > b)
                    a %= b;
                else
                    b %= a;
            }

            return a | b;
        }
    }
}
