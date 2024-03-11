namespace Templar.UI.Settings.Game
{
    public class FullscreenModeSetting : ButtonTextSettingView
    {
        public override Templar.Settings.StringRangeSetting StringRangeSetting => Manager.SettingsManager.FullscreenMode;
    }
}