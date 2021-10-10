namespace Templar.UI.Settings.Game
{
    public class ScreenSizeSetting : ButtonTextSettingView
    {
        public override Templar.Settings.StringRangeSetting StringRangeSetting => Manager.SettingsManager.ScreenSize;
    }
}