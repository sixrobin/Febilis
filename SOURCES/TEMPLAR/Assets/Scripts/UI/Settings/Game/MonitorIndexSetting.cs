namespace Templar.UI.Settings.Game
{
    public class MonitorIndexSetting : IntButtonSettingView
    {
        // TODO: LOCALIZATION.
        private MessagePopup.PopupTextsData _rebootWarningPopupTexts = new MessagePopup.PopupTextsData
        {
            TextKey = "You'll need to reboot the game to apply this change.",
            ContinueTextKey = "OKAY"
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