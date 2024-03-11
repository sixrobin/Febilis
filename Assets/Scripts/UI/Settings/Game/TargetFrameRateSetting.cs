namespace Templar.UI.Settings.Game
{
    public class TargetFrameRateSetting : ButtonTextSettingView
    {
        public override Templar.Settings.StringRangeSetting StringRangeSetting => Manager.SettingsManager.TargetFrameRate;
    }
}