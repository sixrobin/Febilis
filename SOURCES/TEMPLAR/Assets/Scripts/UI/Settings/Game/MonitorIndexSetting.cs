namespace Templar.UI.Settings.Game
{
    public class MonitorIndexSetting : IntButtonSettingView
    {
        private MessagePopup.PopupTextsDatas _rebootWarningPopupTexts
            = new MessagePopup.PopupTextsDatas("You'll need to reboot the game to apply this change.", "Okay");

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