namespace Templar.UI.Settings.Game
{
    public class RunInBackgroundSetting : ToggleSettingView
    {
        public override Templar.Settings.BoolSetting BoolSetting => Manager.SettingsManager.RunInBackground;
    }
}