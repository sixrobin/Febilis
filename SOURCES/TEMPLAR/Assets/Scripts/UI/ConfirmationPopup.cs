namespace Templar.UI
{
    using RSLib.Extensions;
    using UnityEngine;

    public class ConfirmationPopup : UIPanel
    {
        public struct PopupTextsData
        {
            public string TextKey;
            public string ConfirmTextKey;
            public string CancelTextKey;
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

        public void AskForConfirmation(PopupTextsData textsData, ConfirmedEventHandler confirmCallback, ConfirmedEventHandler cancelCallback, bool selectConfirmFirst = false)
        {
            _sourcePanel = Navigation.UINavigationManager.CurrentlyOpenPanel;
            _sourceSelected = Navigation.UINavigationManager.CurrentlySelected;
            _selectConfirmFirst = selectConfirmFirst;

            Navigation.UINavigationManager.OpenAndSelect(this);

            _confirmCallback = confirmCallback.Invoke;
            _cancelCallback = cancelCallback.Invoke;

            _text.text = Localizer.Get(textsData.TextKey);
            _confirmBtn.SetText(Localizer.Get(textsData.ConfirmTextKey));
            _cancelBtn.SetText(Localizer.Get(textsData.CancelTextKey));
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