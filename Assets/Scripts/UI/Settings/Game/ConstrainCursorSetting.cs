namespace Templar.UI.Settings.Game
{
    public class ConstrainCursorSetting : ToggleSettingView
    {
        public override Templar.Settings.BoolSetting BoolSetting => Manager.SettingsManager.ConstrainCursor;
    }
}