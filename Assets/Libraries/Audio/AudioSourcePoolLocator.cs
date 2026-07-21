namespace Pospec.Audio
{
    /// <summary>
    /// Used for accessing and changing active IAudioSourcePool.
    /// </summary>
    public static class AudioSourcePoolLocator
    {
        private static IAudioSourcePool service = new NullAudioSourcePool();

        public static IAudioSourcePool Get()
        {
            return service;
        }

        public static void Register(IAudioSourcePool service)
        {
            if (service == null)
                service = new NullAudioSourcePool();
            AudioSourcePoolLocator.service = service;
        }
    }
}
