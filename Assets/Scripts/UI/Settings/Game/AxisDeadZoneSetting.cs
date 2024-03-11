namespace Templar.UI.Settings.Game
{
    public class AxisDeadZoneSetting : SliderSettingView
    {
        public override Templar.Settings.FloatSetting FloatSetting => Manager.SettingsManager.AxisDeadZone;
    }
}