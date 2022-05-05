namespace Templar
{
    public static class Localization
    {
        public static class Item
        {
            public const string NAME_PREFIX = "ItemName_";
            public const string DESCRIPTION_PREFIX = "ItemDescription_";
            public const string TYPE_PREFIX = "ItemType_";
        }

        public static class Game
        {
            public const string QUIT = "QuitGame";
            public const string QUIT_ASK = "QuitGame_Ask";
            public const string QUIT_CONFIRM = "QuitGame_Confirm";
            public const string QUIT_CANCEL = "QuitGame_Cancel";
        }
        
        public static class Settings
        {
            // Hub.
            public const string CONTROLS = "Settings_Controls";
            public const string GAME = "Settings_Game";
            public const string AUDIO = "Settings_Audio";
            public const string LANGUAGE = "Settings_Language";
            
            // Audio.
            public const string VOLUME_PREFIX = "VolumeSlider_";
        }
    }
}
