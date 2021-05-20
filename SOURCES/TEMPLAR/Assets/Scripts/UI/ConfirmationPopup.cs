namespace Templar.UI
{
    using RSLib.Extensions;
    using UnityEngine;

    public class ConfirmationPopup : UIPanel
    {
        public struct PopupTextsDatas
        {
            public PopupTextsDatas(string text, string confirmText, string cancelText)
            {
                Text = text;
                ConfirmText = confirmText;
                CancelText = cancelText;
            }

            public string Text { get; private set; }
            public string ConfirmText { get; private set; }
            public string CancelText { get; private set; }
        }

        [SerializeField] private TMPro.TextMeshProUGUI _text = null;
        [SerializeField] private RSLib.Framework.GUI.EnhancedButton _confirmBtn = null;
        [SerializeField] private RSLib.Framework.GUI.EnhancedButton _cancelBtn = null;

        public delegate void ConfirmedEventHandler();

        public ConfirmedEventHandler _confirmCallback;
        public ConfirmedEventHandler _cancelCallback;

        public override GameObject FirstSelected => _cancelBtn.gameObject;

        public override void OnBackButtonPressed()
        {
            // [TODO] Cancel confirmation and just go back.
        }

        public void AskForConfirmation(PopupTextsDatas textsDatas, ConfirmedEventHandler confirmCallback, ConfirmedEventHandler cancelCallback)
        {
            Navigation.UINavigationManager.OpenAndSelect(this);

            _confirmCallback = confirmCallback.Invoke;
            _cancelCallback = cancelCallback.Invoke;

            _text.text = textsDatas.Text;
            _confirmBtn.SetText(textsDatas.ConfirmText);
            _cancelBtn.SetText(textsDatas.CancelText);
        }

        private void InvokeConfirmCallback()
        {
            _confirmCallback.Invoke();
            Close();
        }

        private void InvokeCancelCallback()
        {
            _cancelCallback.Invoke();
            Close();
        }

        private void InitButtonsNavigation()
        {
            _confirmBtn.SetMode(UnityEngine.UI.Navigation.Mode.Explicit);
            _cancelBtn.SetMode(UnityEngine.UI.Navigation.Mode.Explicit);

            _confirmBtn.SetSelectOnRight(_cancelBtn);
            _confirmBtn.SetSelectOnLeft(_cancelBtn);

            _cancelBtn.SetSelectOnRight(_confirmBtn);
            _cancelBtn.SetSelectOnLeft(_confirmBtn);
        }

        private void Awake()
        {
            _confirmBtn.onClick.AddListener(InvokeConfirmCallback);
            _cancelBtn.onClick.AddListener(InvokeCancelCallback);

            InitButtonsNavigation();
        }

        private void OnDestroy()
        {
            _confirmBtn.onClick.RemoveListener(InvokeConfirmCallback);
            _cancelBtn.onClick.RemoveListener(InvokeCancelCallback);
        }
    }
}