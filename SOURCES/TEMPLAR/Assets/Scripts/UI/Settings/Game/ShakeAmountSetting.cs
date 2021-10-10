namespace Templar.UI.Settings.Game
{
    public class ShakeAmountSetting : SliderSettingView
    {
        public override Templar.Settings.FloatSetting FloatSetting => Manager.SettingsManager.ShakeAmount;
    }
}