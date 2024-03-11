namespace Templar.UI.Settings.Game
{
    public class MonitorIndexSetting : IntButtonSettingView
    {
        private MessagePopup.PopupTextsData _rebootWarningPopupTexts = new MessagePopup.PopupTextsData
        {
            TextKey = Localization.Settings.MONITOR_INDEX_WARNING,
            ContinueTextKey = Localization.Settings.MONITOR_INDEX_WARNING_CONFIRM
        };

        
        public override Templar.Settings.IntSetting IntSetting => Manager.SettingsManager.MonitorIndex;

        protected override void OnButtonClicked()
        {
            base.OnButtonClicked();

            Navigation.UINavigationManager.MessagePopup.ShowMessage(
                _rebootWarningPopupTexts,
                () => UI.Navigation.UINavigationManager.Select(Selectable.gameObject));
        }
    }
}