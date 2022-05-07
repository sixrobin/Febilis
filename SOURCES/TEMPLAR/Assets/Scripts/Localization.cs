namespace Templar
{
    public static class Localization
    {
        public static class Item
        {
            public const string NAME_PREFIX = "ItemName_";
            public const string DESCRIPTION_PREFIX = "ItemDescription_";
            public const string TYPE_PREFIX = "ItemType_";

            public const string ACTION_USE = "Inventory_Action_Use";
            public const string ACTION_EQUIP = "Inventory_Action_Equip";
            public const string ACTION_DROP = "Inventory_Action_Drop";
            public const string ACTION_MOVE = "Inventory_Action_Move";

            public const string EMPTY_SLOT_NAME = "Inventory_NoItemName";
            public const string EMPTY_SLOT_DESCRIPTION = "Inventory_NoItemDescription";
        }

        public static class Boss
        {
            public const string BOSS_NAME_PREFIX = "BossName_";
        }

        public static class InputTutorial
        {
            public const string PREFIX = "InputTutorial_";
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
            public const string AUDIO_RESET = "Audio_Reset";
            public const string AUDIO_SAVE = "Audio_Save";
            
            // Controls.
            public const string CONTROLS_ACTION_TITLE = "Controls_ActionTitle";
            public const string CONTROLS_BUTTON_TITLE = "Controls_ButtonTitle";
            public const string CONTROLS_ALT_BUTTON_TITLE = "Controls_AltButtonTitle";
            public const string CONTROLS_RESET = "Controls_Reset";
            public const string CONTROLS_ASSIGN_BUTTON_FORMAT = "Controls_AssignBtnFormat";
            public const string CONTROLS_ASSIGN_ALT_BUTTON_FORMAT = "Controls_AssignAltBtnFormat";
            public const string CONTROLS_SAVE = "Controls_Save";
            public const string CONTROLS_SAVE_ASK = "Controls_SaveChanges_Ask";
            public const string CONTROLS_SAVE_CONFIRM = "Controls_SaveChanges_Confirm";
            public const string CONTROLS_SAVE_CANCEL = "Controls_SaveChanges_Cancel";
            public const string CONTROLS_ACTION_NAME_PREFIX = "ActionName_";
            
            // Game settings.
            public const string GAME_TITLE = "Settings_Title";
            public const string GAME_RESET = "Controls_Reset";
            public const string GAME_SAVE = "Controls_Save";
            public const string SCREEN_MODE_PREFIX = "ScreenMode_";
            
            // Language.
            public const string LANGUAGE_SAVE = "Language_Save";
        }

        public static class MainMenu
        {
            public const string PRESS_ANY_KEY = "MainMenu_PressAnyKey";
            public const string CONTINUE = "MainMenu_Continue";
            public const string NEW_GAME = "MainMenu_NewGame";
            public const string SETTINGS = "MainMenu_Settings";
            public const string QUIT = "MainMenu_Quit";

            public const string OVERWRITE_SAVE_ASK = "MainMenu_OverwriteSave_Ask";
            public const string OVERWRITE_SAVE_CONFIRM = "MainMenu_OverwriteSave_Confirm";
            public const string OVERWRITE_SAVE_CANCEL = "MainMenu_OverwriteSave_Cancel";
        }
    }
}
