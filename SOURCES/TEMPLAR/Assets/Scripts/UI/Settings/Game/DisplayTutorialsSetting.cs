namespace Templar.UI.Settings.Game
{
    public class DisplayTutorialsSetting : ToggleSettingView
    {
        public override Templar.Settings.BoolSetting BoolSetting => Manager.SettingsManager.DisplayTutorials;
    }
}