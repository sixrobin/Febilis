namespace Templar.UI
{
    using UnityEngine;

    public class KeyBindingPanel : UnityEngine.UI.Selectable, UnityEngine.EventSystems.ISelectHandler
    {
        [SerializeField] private TMPro.TextMeshProUGUI _actionName = null;
        [SerializeField] private TMPro.TextMeshProUGUI _btnName = null;
        [SerializeField] private TMPro.TextMeshProUGUI _altBtnName = null;

        [SerializeField] private KeyBindingButton _baseBtnButton = null;
        [SerializeField] private KeyBindingButton _altBtnButton = null;

        private (KeyCode btn, KeyCode altBtn) _btns;

        public string ActionId { get; private set; }

        public UnityEngine.UI.Button BaseBtnButton => _baseBtnButton;
        public UnityEngine.UI.Button AltBtnButton => _altBtnButton;

        public override void OnSelect(UnityEngine.EventSystems.BaseEventData eventData)
        {
            base.OnSelect(eventData);
            UI.Navigation.UINavigationManager.Select(_baseBtnButton.gameObject);
        }

        public void Init(string actionId, (KeyCode btn, KeyCode alt) btns)
        {
            _baseBtnButton.SetHorizontalNavigation(_altBtnButton);
            _altBtnButton.SetHorizontalNavigation(_baseBtnButton);

            ActionId = actionId;
            _btns = btns;

            // [TODO] Need some sprites or localized texts for some KeyCodes (like JoystickButton0, LeftAlt etc.).
            _actionName.text = ActionId.ToString();
            _btnName.text = _btns.btn != KeyCode.None ? _btns.btn.ToString() : string.Empty;
            _altBtnName.text = _btns.altBtn != KeyCode.None ? _btns.altBtn.ToString() : string.Empty;

            Show();
        }

        public void SetUpNavigation(UnityEngine.UI.Selectable selectable)
        {
            _baseBtnButton.SetUpNavigation(selectable);
            _altBtnButton.SetUpNavigation(selectable);
        }

        public void SetUpNavigation(KeyBindingPanel bindingPanel)
        {
            _baseBtnButton.SetUpNavigation(bindingPanel.BaseBtnButton);
            _altBtnButton.SetUpNavigation(bindingPanel.AltBtnButton);
        }

        public void SetDownNavigation(UnityEngine.UI.Selectable selectable)
        {
            _baseBtnButton.SetDownNavigation(selectable);
            _altBtnButton.SetDownNavigation(selectable);
        }

        public void SetDownNavigation(KeyBindingPanel bindingPanel)
        {
            _baseBtnButton.SetDownNavigation(bindingPanel.BaseBtnButton);
            _altBtnButton.SetDownNavigation(bindingPanel.AltBtnButton);
        }

        public void OverrideKey(KeyCode btn, bool alt)
        {
            if (alt)
                _btns.altBtn = btn;
            else
                _btns.btn = btn;

            _btnName.text = _btns.btn.ToString();
            _altBtnName.text = _btns.altBtn.ToString();
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}