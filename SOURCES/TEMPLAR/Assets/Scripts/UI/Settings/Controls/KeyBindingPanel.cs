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

        public KeyBindingButton BaseBtnButton => _baseBtn;
        public KeyBindingButton AltBtnButton => _altBtn;

        /// <summary>
        /// Redirects the UI navigation to the base button, so that the panel can be set as a selectable.
        /// </summary>
        /// <param name="eventData">Navigation event data.</param>
        public override void OnSelect(UnityEngine.EventSystems.BaseEventData eventData)
        {
            base.OnSelect(eventData);
            Navigation.UINavigationManager.Select(BaseBtnButton.gameObject);
        }

        public void Init(string actionId, RSLib.Framework.InputSystem.InputMapDatas.KeyBinding keyBinding)
        {
            ActionId = actionId;
            _btns = keyBinding.KeyCodes;

            BaseBtnButton.SetMode(UnityEngine.UI.Navigation.Mode.Explicit);
            AltBtnButton.SetMode(UnityEngine.UI.Navigation.Mode.Explicit);
            BaseBtnButton.SetSelectOnRight(AltBtnButton);
            AltBtnButton.SetSelectOnLeft(BaseBtnButton);

            Localize();
            Show();
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

            BaseBtnButton.SetText(_btns.btn.ToString());
            AltBtnButton.SetText(_btns.altBtn.ToString());
        }

        public void Localize()
        {
            _actionName.text = Localizer.Get($"{Localization.Settings.CONTROLS_ACTION_NAME_PREFIX}{ActionId}");

            LocalizeKeyCode(BaseBtnButton, _btns.btn);
            LocalizeKeyCode(AltBtnButton, _btns.altBtn);
        }

        private void LocalizeKeyCode(KeyBindingButton button, KeyCode keycode)
        {
            if (Localizer.TryGet($"{Localization.KeyCode.KEYCODE_PREFIX}{keycode}", out string localizedKeyCode))
                button.SetText(localizedKeyCode);
            else
                button.SetText(keycode != KeyCode.None ? KeyCodeSymbols.GetSymbol(keycode) : string.Empty);
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