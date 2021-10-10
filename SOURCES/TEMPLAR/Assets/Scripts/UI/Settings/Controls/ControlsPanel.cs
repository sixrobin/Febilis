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
    public class ControlsPanel : SettingsPanelBase
    {
        [SerializeField] private UnityEngine.UI.Scrollbar _controlsScrollBar = null;
        [SerializeField] private KeyBindingPanel[] _bindingPanels = null;
        [SerializeField] private GameObject _assignKeyScreen = null;
        [SerializeField] private RSLib.DataColor _assignedKeyTextColor = null;
        [SerializeField] private TMPro.TextMeshProUGUI _assignKeyText = null;
        [SerializeField] private UnityEngine.UI.Button _resetBindingsBtn = null;
        [SerializeField] private UnityEngine.UI.Button _saveBindingsBtn = null;

        private InputMap _editedMap;
        private KeyBindingPanel _currentlyAssignedPanel;

        private ConfirmationPopup.PopupTextsDatas _uncommittedChangesPopupTexts
            = new ConfirmationPopup.PopupTextsDatas("Save changes ?", "Yes", "No");

        private bool _navigationInit;

        public override GameObject FirstSelected => BackBtn.gameObject;

        public bool UncommittedChanges { get; private set; }

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
                InitBindingsPanelsNavigation();

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

        private void InitBindingsPanelsNavigation()
        {
            if (_navigationInit)
                return;

            for (int i = 0; i < _bindingPanels.Length; ++i)
            {
                bool first = i == 0;
                bool last = i == _bindingPanels.Length - 1 || !_bindingPanels[i + 1].gameObject.activeSelf;

                if (first)
                {
                    _bindingPanels[i].BaseBtnButton.SetSelectOnUp(BackBtn);
                    _bindingPanels[i].AltBtnButton.SetSelectOnUp(QuitBtn);
                }
                else
                {
                    _bindingPanels[i].SetPanelOnUp(_bindingPanels[i - 1]);
                }

                if (!last)
                {
                    _bindingPanels[i].SetPanelOnDown(_bindingPanels[i + 1]);
                    continue;
                }

                _resetBindingsBtn.SetMode(UnityEngine.UI.Navigation.Mode.Explicit);
                _saveBindingsBtn.SetMode(UnityEngine.UI.Navigation.Mode.Explicit);

                _resetBindingsBtn.SetSelectOnUp(_bindingPanels[i].BaseBtnButton);
                _saveBindingsBtn.SetSelectOnUp(_bindingPanels[i].AltBtnButton);

                _bindingPanels[i].BaseBtnButton.SetSelectOnDown(_resetBindingsBtn);
                _bindingPanels[i].AltBtnButton.SetSelectOnDown(_saveBindingsBtn);

                break;
            }

            _navigationInit = true;
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
            _assignKeyText.text = $"Assign {(alt ? "alt" : "base")} key to\n<color=#{_assignedKeyTextColor.HexCode}>{bindingPanel.ActionId}</color>...";
            
            InputManager.AssignKey(_editedMap, _currentlyAssignedPanel.ActionId, alt, OnKeyAssigned);
        }

        private void UpdateAllBindingsPanels()
        {
            int i = 0;

            foreach (System.Collections.Generic.KeyValuePair<string, (KeyCode btn, KeyCode altBtn)> binding in _editedMap.MapCopy)
                _bindingPanels[i++].Init(binding.Key, binding.Value);

            for (; i < _bindingPanels.Length; ++i)
                _bindingPanels[i].Hide();
        }

        private void ResetEditedMap()
        {
            _editedMap = new InputMap(InputManager.GetMapCopy());
            UpdateAllBindingsPanels();
        }

        private void ResetDefaultMap()
        {
            _editedMap = InputManager.GetDefaultMapCopy();
            UpdateAllBindingsPanels();
            UncommittedChanges = true;
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