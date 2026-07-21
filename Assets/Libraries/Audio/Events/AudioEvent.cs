using UnityEngine;

namespace Pospec.Audio
{
	public abstract class AudioEvent : ScriptableObject
	{
		public abstract void Play(AudioSource source);

		public void PlaySFX()
		{
			AudioSourcePoolLocator.Get().Play(this);
		}

		public void PlaySFXFrom(Vector3 pos)
		{
			AudioSourcePoolLocator.Get().PlayFrom(this, pos);
		}
	}
}
