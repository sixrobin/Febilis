namespace Templar.UI.Settings.Controls
{
    using RSLib.Extensions;
    using RSLib.Framework.InputSystem;
    using System.Linq;
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [DisallowMultipleComponent]
    public class ControlsPanel : SettingsPanelBase, IScrollViewClosestItemGetter
    {
        private const float SCROLL_BAR_AUTO_REFRESH_VALUE = 0.05f;
        private const float SCROLL_BAR_AUTO_REFRESH_MARGIN = 0.1f;
        
        [Header("TEXT REFS")]
        [SerializeField] private TMPro.TextMeshProUGUI _title = null;
        [SerializeField] private TMPro.TextMeshProUGUI _actionTitle = null;
        [SerializeField] private TMPro.TextMeshProUGUI _buttonsTitle = null;
        [SerializeField] private TMPro.TextMeshProUGUI _buttonTitle = null;
        [SerializeField] private TMPro.TextMeshProUGUI _altButtonTitle = null;
        [SerializeField] private TMPro.TextMeshProUGUI _assignKeyText = null;
        [SerializeField] private TMPro.TextMeshProUGUI _cancelAssignKeyText = null;

        [Header("REFS")]
        [SerializeField] private RectTransform _settingsViewport = null;
        [SerializeField] private UnityEngine.UI.Scrollbar _controlsScrollBar = null;
        [SerializeField] private RectTransform _scrollHandle = null;
        [SerializeField] private ScrollbarToScrollViewNavigationHandler _scrollbarToScrollViewNavigationHandler = null;
        [SerializeField] private KeyBindingPanel[] _bindingPanels = null;
        [SerializeField] private GameObject _assignKeyScreen = null;
        [SerializeField] private ColorByZone[] _assignedKeyTextColors = null;
        [SerializeField] private RSLib.Framework.GUI.EnhancedButton _resetBindingsBtn = null;
        [SerializeField] private RSLib.Framework.GUI.EnhancedButton _saveBindingsBtn = null;

        private InputMap _editedMap;
        private KeyBindingPanel _currentlyAssignedPanel;

        private ConfirmationPopup.PopupTextsData _uncommittedChangesPopupTexts = new ConfirmationPopup.PopupTextsData
        {
            TextKey = Localization.Settings.CONTROLS_SAVE_ASK,
            ConfirmTextKey = Localization.Settings.CONTROLS_SAVE_CONFIRM,
            CancelTextKey = Localization.Settings.CONTROLS_SAVE_CANCEL
        };

        private bool _navigationInit;

        public override GameObject FirstSelected => BackBtn.gameObject;

        public ScrollbarToScrollViewNavigationHandler ScrollbarToScrollViewNavigationHandler => _scrollbarToScrollViewNavigationHandler;
        
        public bool UncommittedChanges { get; private set; }

        public GameObject GetClosestItemToScrollbar()
        {
            RectTransform closestSlot = null;
            float sqrClosestDist = Mathf.Infinity;

            Vector3[] scrollHandleWorldCorners = new Vector3[4];
            _scrollHandle.GetWorldCorners(scrollHandleWorldCorners);
            Vector3 scrollHandleCenterWorld = RSLib.Maths.Maths.ComputeAverageVector(scrollHandleWorldCorners);

            System.Collections.Generic.List<RectTransform> buttons = new System.Collections.Generic.List<RectTransform>();
            foreach (KeyBindingPanel bindingPanel in _bindingPanels.Where(o => o.gameObject.activeSelf))
            {
                buttons.Add(bindingPanel.BaseBtnButton.transform as RectTransform);
                buttons.Add(bindingPanel.AltBtnButton.transform as RectTransform);
            }
            
            foreach (RectTransform target in buttons)
            {
                Vector3[] slotWorldCorners = new Vector3[4];
                target.GetWorldCorners(slotWorldCorners);
                Vector3 slotCenterWorld = RSLib.Maths.Maths.ComputeAverageVector(slotWorldCorners);

                float sqrTargetDist = (slotCenterWorld - scrollHandleCenterWorld).sqrMagnitude;
                if (sqrTargetDist > sqrClosestDist)
                    continue;

                sqrClosestDist = sqrTargetDist;
                closestSlot = target;
            }

            return closestSlot.gameObject;
        }
        
        public override void Close()
        {
            if (!UncommittedChanges)
            {
                base.Close();
                return;
            }

            Navigation.UINavigationManager.ConfirmationPopup.AskForConfirmation(
                _uncommittedChangesPopupTexts,
                () =>
                {
                    SaveBindings();
                    Close();
                },
                () =>
                {
                    UncommittedChanges = false;
                    Close();
                });
        }

        public override void OnBackButtonPressed()
        {
            // Players may want to assign the key that is used to go back.
            if (InputManager.IsAssigningKey)
                return;

            if (!UncommittedChanges)
            {
                base.OnBackButtonPressed();
                Manager.OptionsManager.Instance.OpenSettings(this);
                return;
            }

            Navigation.UINavigationManager.ConfirmationPopup.AskForConfirmation(
                _uncommittedChangesPopupTexts,
                () =>
                {
                    SaveBindings();
                    OnBackButtonPressed();
                },
                () =>
                {
                    UncommittedChanges = false;
                    OnBackButtonPressed();
                });
        }

        public override void Display(bool show)
        {
            base.Display(show);

            if (show)
            {
                ResetEditedMap();
                InitNavigation();

                Localize();
                
                UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(RectTransform);
                _controlsScrollBar.value = 1f;
                UncommittedChanges = false;
            }
        }

        private void SaveBindings()
        {
            InputManager.SetMap(_editedMap);
            InputManager.SaveCurrentMap();
            UncommittedChanges = false;
        }

        private void InitNavigation()
        {
            if (_navigationInit)
                return;

            for (int i = 0; i < _bindingPanels.Length; ++i)
            {
                bool first = i == 0;
                bool last = i == _bindingPanels.Length - 1 || !_bindingPanels[i + 1].gameObject.activeSelf;

                _bindingPanels[i].BaseBtnButton.Selected += OnBindingPanelSelected;
                _bindingPanels[i].AltBtnButton.Selected += OnBindingPanelSelected;
                
                _bindingPanels[i].BaseBtnButton.SetSelectOnRight(_controlsScrollBar);
                _bindingPanels[i].AltBtnButton.SetSelectOnRight(_controlsScrollBar);
                _bindingPanels[i].BaseBtnButton.SetSelectOnDown(_bindingPanels[i].AltBtnButton);
                _bindingPanels[i].AltBtnButton.SetSelectOnUp(_bindingPanels[i].BaseBtnButton);
                
                if (first)
                {
                    _bindingPanels[i].BaseBtnButton.SetSelectOnUp(QuitBtn);
                }

                if (!last)
                {
                    _bindingPanels[i + 1].BaseBtnButton.SetSelectOnUp(_bindingPanels[i].AltBtnButton);
                    _bindingPanels[i].AltBtnButton.SetSelectOnDown(_bindingPanels[i + 1].BaseBtnButton);
                    continue;
                }

                _resetBindingsBtn.SetMode(UnityEngine.UI.Navigation.Mode.Explicit);
                _saveBindingsBtn.SetMode(UnityEngine.UI.Navigation.Mode.Explicit);

                _resetBindingsBtn.SetSelectOnUp(_bindingPanels[i].AltBtnButton);
                _saveBindingsBtn.SetSelectOnUp(_bindingPanels[i].AltBtnButton);

                _bindingPanels[i].AltBtnButton.SetSelectOnDown(_saveBindingsBtn);
                
                break;
            }

            _resetBindingsBtn.SetSelectOnRight(_saveBindingsBtn);
            _saveBindingsBtn.SetSelectOnLeft(_resetBindingsBtn);
            
            _resetBindingsBtn.SetSelectOnDown(_backBtn);
            _backBtn.SetSelectOnUp(_resetBindingsBtn);
            _saveBindingsBtn.SetSelectOnDown(_quitBtn);
            _quitBtn.SetSelectOnUp(_saveBindingsBtn);
            _quitBtn.SetSelectOnDown(_bindingPanels[0].BaseBtnButton);
            
            _controlsScrollBar.SetMode(UnityEngine.UI.Navigation.Mode.Explicit);
            _controlsScrollBar.SetSelectOnLeft(ScrollbarToScrollViewNavigationHandler);
            ScrollbarToScrollViewNavigationHandler.SetClosestItemGetter(this);
            
            _navigationInit = true;
        }

        private void OnBindingPanelSelected(KeyBindingButton keyBindingButton)
        {
            RSLib.Helpers.AdjustScrollViewToFocusedItem(keyBindingButton.transform as RectTransform,
                                                        _settingsViewport,
                                                        _controlsScrollBar,
                                                        SCROLL_BAR_AUTO_REFRESH_VALUE,
                                                        SCROLL_BAR_AUTO_REFRESH_MARGIN);
        }

        private void OnKeyAssigned(string actionId, KeyCode btn, bool alt)
        {
            UnityEngine.Assertions.Assert.IsNotNull(_currentlyAssignedPanel, "Trying to assign button to a null panel.");
            UnityEngine.Assertions.Assert.IsTrue(actionId == _currentlyAssignedPanel.ActionId, "Assigned panel action Id and system assigned action Id are not the same.");

            if (!UncommittedChanges && _currentlyAssignedPanel.IsKeyDifferent(btn, alt))
                UncommittedChanges = true;

            _currentlyAssignedPanel.OverrideKey(btn, alt);
            _currentlyAssignedPanel = null;

            UpdateAllBindingsPanels();
            _assignKeyScreen.SetActive(false);
        }

        private void AssignKey(KeyBindingPanel bindingPanel, bool alt)
        {
            if (!Displayed)
                return;

            _currentlyAssignedPanel = bindingPanel;
            _assignKeyScreen.SetActive(true);

            string assignKeyTextFormat = Localizer.Get(alt ? Localization.Settings.CONTROLS_ASSIGN_ALT_BUTTON_FORMAT : Localization.Settings.CONTROLS_ASSIGN_BUTTON_FORMAT);
            string localizedActionId = Localizer.Get($"{Localization.Settings.CONTROLS_ACTION_NAME_PREFIX}{bindingPanel.ActionId}");
            
            Flags.ZoneIdentifier currentZone = Manager.BoardsManager.CurrentBoard != null ? Manager.BoardsManager.CurrentBoard.BoardIdentifier.ContainingZoneIdentifier : null;
            Color assignKeyColor = currentZone != null
                                   ? _assignedKeyTextColors.FirstOrDefault(o => o.Zone == currentZone).DataColor
                                   : _assignKeyText.color;
            
            _assignKeyText.text = string.Format(assignKeyTextFormat, $"<color=#{assignKeyColor.ToHexRGB()}>{localizedActionId}</color>");
            
            InputManager.AssignKey(_editedMap, _currentlyAssignedPanel.ActionId, alt, OnKeyAssigned);
        }

        private void UpdateAllBindingsPanels()
        {
            int i = 0;

            foreach (System.Collections.Generic.KeyValuePair<string, InputMapDatas.KeyBinding> binding in _editedMap.MapCopy)
                if (binding.Value.UserAssignable)
                    _bindingPanels[i++].Init(binding.Key, binding.Value);

            for (; i < _bindingPanels.Length; ++i)
                _bindingPanels[i].Hide();
        }

        private void ResetEditedMap()
        {
            InputMap map = InputManager.GetMap();
            _editedMap = new InputMap(map.MapCopy, map.UseAltButtons);
            UpdateAllBindingsPanels();
        }

        private void ResetDefaultMap()
        {
            _editedMap = InputManager.GetDefaultMapCopy();
            UpdateAllBindingsPanels();
            UncommittedChanges = true;
        }

        private void Localize()
        {
            _title.text = Localizer.Get(Localization.Settings.CONTROLS);
            _actionTitle.text = Localizer.Get(Localization.Settings.CONTROLS_ACTION_TITLE);
            _buttonsTitle.text = Localizer.Get(Localization.Settings.CONTROLS_BUTTONS_TITLE);
            _buttonTitle.text = Localizer.Get(Localization.Settings.CONTROLS_BUTTON_TITLE);
            _altButtonTitle.text = Localizer.Get(Localization.Settings.CONTROLS_ALT_BUTTON_TITLE);
            _cancelAssignKeyText.text = Localizer.Get(Localization.Settings.CONTROLS_CANCEL_ASSIGN);
            
            _resetBindingsBtn.SetText(Localizer.Get(Localization.Settings.CONTROLS_RESET));
            _saveBindingsBtn.SetText(Localizer.Get(Localization.Settings.CONTROLS_SAVE));

            for (int i = 0; i < _bindingPanels.Length; ++i)
                if (!string.IsNullOrEmpty(_bindingPanels[i].ActionId))
                   _bindingPanels[i].Localize();
        }
        
        protected override void Start()
        {
            base.Start();

            _resetBindingsBtn.onClick.AddListener(ResetDefaultMap);
            _saveBindingsBtn.onClick.AddListener(SaveBindings);
            _saveBindingsBtn.onClick.AddListener(OnBackButtonPressed);

            for (int i = 0; i < _bindingPanels.Length; ++i)
            {
                KeyBindingPanel panel = _bindingPanels[i];
                _bindingPanels[i].BaseBtnButton.onClick.AddListener(() => AssignKey(panel, false));
                _bindingPanels[i].AltBtnButton.onClick.AddListener(() => AssignKey(panel, true));
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _resetBindingsBtn.onClick.RemoveListener(ResetDefaultMap);
            _saveBindingsBtn.onClick.RemoveListener(SaveBindings);
            _saveBindingsBtn.onClick.RemoveListener(OnBackButtonPressed);
        }

        public void LocateBindingPanels()
        {
            _bindingPanels = FindObjectsOfType<KeyBindingPanel>().OrderBy(o => o.transform.GetSiblingIndex()).ToArray();
            RSLib.EditorUtilities.PrefabEditorUtilities.SetCurrentPrefabStageDirty();
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ControlsPanel))]
    public class ControlsPanelEditor : RSLib.EditorUtilities.ButtonProviderEditor<ControlsPanel>
    {
        protected override void DrawButtons()
        {
            DrawButton("Locate Binding Panels", Obj.LocateBindingPanels);
        }
    }
#endif
}