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

        private UIPanel _sourcePanel;
        private GameObject _sourceSelected;

        private bool _selectConfirmFirst;

        public delegate void ConfirmedEventHandler();

        private ConfirmedEventHandler _confirmCallback;
        private ConfirmedEventHandler _cancelCallback;

        public override GameObject FirstSelected => _selectConfirmFirst ? _confirmBtn.gameObject : _cancelBtn.gameObject;

        public override void OnBackButtonPressed()
        {
            Close();

            Navigation.UINavigationManager.SetPanelAsCurrent(_sourcePanel);
            Navigation.UINavigationManager.Select(_sourceSelected);
        }

        public void AskForConfirmation(PopupTextsDatas textsDatas, ConfirmedEventHandler confirmCallback, ConfirmedEventHandler cancelCallback, bool selectConfirmFirst = false)
        {
            _sourcePanel = Navigation.UINavigationManager.CurrentlyOpenPanel;
            _sourceSelected = Navigation.UINavigationManager.CurrentlySelected;
            _selectConfirmFirst = selectConfirmFirst;

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

        protected override void Awake()
        {
            base.Awake();

            _confirmBtn.onClick.AddListener(InvokeConfirmCallback);
            _cancelBtn.onClick.AddListener(InvokeCancelCallback);

            InitButtonsNavigation();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _confirmBtn.onClick.RemoveListener(InvokeConfirmCallback);
            _cancelBtn.onClick.RemoveListener(InvokeCancelCallback);
        }
    }
}