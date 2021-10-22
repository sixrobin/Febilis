namespace Templar.UI.Settings.Controls   
{
    using RSLib.Extensions;
    using UnityEngine;

    [DisallowMultipleComponent]
    public class KeyBindingPanel : UnityEngine.UI.Selectable, UnityEngine.EventSystems.ISelectHandler
    {
        [SerializeField] private TMPro.TextMeshProUGUI _actionName = null;
        [SerializeField] private KeyBindingButton _baseBtn = null;
        [SerializeField] private KeyBindingButton _altBtn = null;

        private (KeyCode btn, KeyCode altBtn) _btns;

        public string ActionId { get; private set; }

        public UnityEngine.UI.Button BaseBtnButton => _baseBtn;
        public UnityEngine.UI.Button AltBtnButton => _altBtn;

        /// <summary>
        /// Redirects the UI navigation to the base button, so that the panel can be set as a selectable.
        /// </summary>
        /// <param name="eventData">Navigation event data.</param>
        public override void OnSelect(UnityEngine.EventSystems.BaseEventData eventData)
        {
            base.OnSelect(eventData);
            Navigation.UINavigationManager.Select(_baseBtn.gameObject);
        }

        public void Init(string actionId, RSLib.Framework.InputSystem.InputMapDatas.KeyBinding keyBinding)
        {
            ActionId = actionId;
            _btns = keyBinding.KeyCodes;

            _baseBtn.SetMode(UnityEngine.UI.Navigation.Mode.Explicit);
            _altBtn.SetMode(UnityEngine.UI.Navigation.Mode.Explicit);
            _baseBtn.SetSelectOnRight(_altBtn);
            _altBtn.SetSelectOnLeft(_baseBtn);

            // [TODO] Need some sprites or localized texts for some KeyCodes (like JoystickButton0, LeftAlt etc.).
            _actionName.text = ActionId.ToString();
            _baseBtn.SetText(_btns.btn != KeyCode.None ? _btns.btn.ToString() : string.Empty);
            _altBtn.SetText(_btns.altBtn != KeyCode.None ? _btns.altBtn.ToString() : string.Empty);

            Show();
        }

        public void SetSelectOnUp(UnityEngine.UI.Selectable selectable)
        {
            _baseBtn.SetSelectOnUp(selectable);
            _altBtn.SetSelectOnUp(selectable);
        }

        public void SetPanelOnUp(KeyBindingPanel bindingPanel)
        {
            _baseBtn.SetSelectOnUp(bindingPanel.BaseBtnButton);
            _altBtn.SetSelectOnUp(bindingPanel.AltBtnButton);
        }

        public void SetSelectOnDown(UnityEngine.UI.Selectable selectable)
        {
            _baseBtn.SetSelectOnDown(selectable);
            _altBtn.SetSelectOnDown(selectable);
        }

        public void SetPanelOnDown(KeyBindingPanel bindingPanel)
        {
            _baseBtn.SetSelectOnDown(bindingPanel.BaseBtnButton);
            _altBtn.SetSelectOnDown(bindingPanel.AltBtnButton);
        }

        public bool IsKeyDifferent(KeyCode btn, bool alt)
        {
            return alt ? _btns.altBtn != btn : _btns.btn != btn;
        }

        public void OverrideKey(KeyCode btn, bool alt)
        {
            if (alt)
                _btns.altBtn = btn;
            else
                _btns.btn = btn;

            _baseBtn.SetText(_btns.btn.ToString());
            _altBtn.SetText(_btns.altBtn.ToString());
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