using UnityEngine;
using Pospec.Common;
using Random = UnityEngine.Random;

namespace Pospec.Audio
{
	[CreateAssetMenu(menuName="Audio Events/Simple")]
	public class SimpleAudioEvent : AudioEvent
	{
		public AudioClip[] clips;

        [MinMaxRange(0, 1)]
        public RangedFloat volume = new RangedFloat(1f, 1f);

		[MinMaxRange(0, 3)]
		public RangedFloat pitch = new RangedFloat(1f, 1f);

		public bool loop;

		public override void Play(AudioSource source)
		{
			if (clips.Length == 0) return;

			source.clip = clips[Random.Range(0, clips.Length)];
			source.volume = Random.Range(volume.minValue, volume.maxValue);
			source.pitch = Random.Range(pitch.minValue, pitch.maxValue);
			source.loop = loop;
			source.Play();
		}
	}
}
