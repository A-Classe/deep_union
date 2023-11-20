#if !EXCLUDE_UNITY_DEBUG_SHEET
namespace Debug
{
    public static class AssetKeys
    {
        public static class Resources
        {
            public static class Icon
            {
                private const string prefix = "Icons/tex_uds_icon_";
                public const string Settings = prefix + "settings";
                public const string Tools = prefix + "tools";
                public const string CharacterViewer = prefix + "character_viewer";
                public const string Model = prefix + "model";
                public const string Motion = prefix + "motion";
                public const string Position = prefix + "position";
                public const string Rotation = prefix + "rotation";
                public const string AutoRotation = prefix + "auto_rotation";
                public const string FPS = prefix + "fps";
                public const string Ram = prefix + "ram";
                public const string Console = prefix + "console";
            }
        }
    }
}
#endif