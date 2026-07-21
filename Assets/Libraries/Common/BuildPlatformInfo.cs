using System;

namespace Pospec.Common
{
    public static class BuildPlatformInfo
    {
        private const BuildPlatform current =
#if UNITY_EMBEDDED_LINUX
            BuildPlatform.EmbededLinux;
#elif UNITY_QNX
            BuildPlatform.QNX;
#elif UNITY_STANDALONE_OSX
            BuildPlatform.OSX;
#elif UNITY_STANDALONE_WIN
            BuildPlatform.Windows;
#elif UNITY_STANDALONE_LINUX
            BuildPlatform.Linux;
#elif UNITY_IOS
            BuildPlatform.IOS;
#elif UNITY_IPHONE
            BuildPlatform.iPhone;
#elif UNITY_VISIONOS
            BuildPlatform.VisionOS;
#elif UNITY_ANDROID
            BuildPlatform.Android;
#elif UNITY_TVOS
            BuildPlatform.TvOS;
#elif UNITY_WSA
            BuildPlatform.WSA;
#elif UNITY_WSA_10_0
            BuildPlatform.WSA10;
#elif UNITY_WEBGL
            BuildPlatform.WebGL;
#else
            BuildPlatform.Unknown;
#endif

        public static bool ContainsCurrentPlatform(this BuildPlatform mask)
        {
            return current == (current & mask);
        }
    }

    [Flags]
    public enum BuildPlatform
    {
        Unknown = 0x0,
        Windows = 0x1,
        Linux = 0x2,
        EmbededLinux = 0x4,
        OSX = 0x8,
        Android = 0x10,
        IOS = 0x20,
        iPhone = 0x40,
        WebGL = 0x80,
        VisionOS = 0x100,
        TvOS = 0x200,
        WSA = 0x400,
        WSA10 = 0x800,
        QNX = 0x1000,
    }
}
