namespace FogboundMaze
{
    public static class ReleaseVersion
    {
        public const string Current = "1.5.1";
        public const int AndroidCode = 13;
        public static string Display
        {
            get
            {
#if UNITY_EDITOR
                return UnityEditor.PlayerSettings.bundleVersion;
#else
                return UnityEngine.Application.version;
#endif
            }
        }
    }
}
