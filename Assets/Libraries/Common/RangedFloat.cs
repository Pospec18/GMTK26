using System;

namespace Pospec.Common
{
	[Serializable]
	public struct RangedFloat
	{
		public float minValue;
		public float maxValue;

		public RangedFloat(float min, float max)
		{
			minValue = min;
			maxValue = max;
		}
	}
}
