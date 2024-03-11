namespace Templar.UI.Settings.Game
{
    public class PixelPerfectSetting : ToggleSettingView
    {
        public override Templar.Settings.BoolSetting BoolSetting => Manager.SettingsManager.PixelPerfect;
    }
}