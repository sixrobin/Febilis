namespace Templar.UI
{
    using UnityEngine;

    public class MessagePopup : UIPanel
    {
        public struct PopupTextsData
        {
            public string TextKey;
            public string ContinueTextKey;
        }

        [SerializeField] private TMPro.TextMeshProUGUI _text = null;
        [SerializeField] private RSLib.Framework.GUI.EnhancedButton _continueBtn = null;

        private UIPanel _sourcePanel;
        private GameObject _sourceSelected;

        public delegate void ContinuedEventHandler();

        private ContinuedEventHandler _continueCallback;

        public override GameObject FirstSelected => _continueBtn.gameObject;

        public override void OnBackButtonPressed()
        {
            Close();

            Navigation.UINavigationManager.SetPanelAsCurrent(_sourcePanel);
            Navigation.UINavigationManager.Select(_sourceSelected);
        }

        public void ShowMessage(PopupTextsData textsData, ContinuedEventHandler continueCallback)
        {
            _sourcePanel = Navigation.UINavigationManager.CurrentlyOpenPanel;
            _sourceSelected = Navigation.UINavigationManager.CurrentlySelected;

            Navigation.UINavigationManager.OpenAndSelect(this);

            _continueCallback = continueCallback.Invoke;

            _text.text = RSLib.Localization.Localizer.Get(textsData.TextKey);
            _continueBtn.SetText(RSLib.Localization.Localizer.Get(textsData.ContinueTextKey));
        }

        private void InvokeContinueCallback()
        {
            _continueCallback.Invoke();
            OnBackButtonPressed();
        }

        protected override void Awake()
        {
            base.Awake();

            _continueBtn.onClick.AddListener(InvokeContinueCallback);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _continueBtn.onClick.RemoveListener(InvokeContinueCallback);
        }
    }
}