namespace Templar.UI.Options.Controls
{
    using RSLib.Extensions;
    using RSLib.Framework.InputSystem;
    using System.Linq;
    using UnityEngine;

    [DisallowMultipleComponent]
    public class ControlsPanel : OptionPanelBase
    {
        [SerializeField] private UnityEngine.UI.Scrollbar _controlsScrollBar = null;
        [SerializeField] private KeyBindingPanel[] _bindingPanels = null;
        [SerializeField] private GameObject _assignKeyScreen = null;
        [SerializeField] private RSLib.DataColor _assignedKeyTextColor = null;
        [SerializeField] private TMPro.TextMeshProUGUI _assignKeyText = null;
        [SerializeField] private UnityEngine.UI.Button _resetBindingsBtn = null;

        private KeyBindingPanel _currentlyAssignedPanel;
        private RectTransform _rectTransform;

        private RectTransform RectTransform
        {
            get
            {
                if (_rectTransform == null)
                    _rectTransform = Canvas.GetComponent<RectTransform>();

                return _rectTransform;
            }
        }

        public override GameObject FirstSelected => BackBtn.gameObject;

        public override void OnBackButtonPressed()
        {
            // Players may want to assign the key that is used to go back.
            if (InputManager.IsAssigningKey)
                return;

            base.OnBackButtonPressed();

            InputManager.SaveCurrentMap(); // [TODO] Validate popup ? Or just don't save ?
            Manager.OptionsManager.Instance.OpenSettings();
        }

        public override void Display(bool show)
        {
            base.Display(show);

            if (show)
            {
                UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(RectTransform);
                _controlsScrollBar.value = 1f;
            }
        }

        private void InitBindingsPanelsNavigation()
        {
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
                _resetBindingsBtn.SetSelectOnUp(_bindingPanels[i]);
                _bindingPanels[i].SetSelectOnDown(_resetBindingsBtn);

                break;
            }
        }

        private void OnKeyAssigned(string actionId, KeyCode btn, bool alt)
        {
            UnityEngine.Assertions.Assert.IsNotNull(_currentlyAssignedPanel, "Trying to assign button to a null panel.");
            UnityEngine.Assertions.Assert.IsTrue(actionId == _currentlyAssignedPanel.ActionId, "Assigned panel action Id and system assigned action Id are not the same.");

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
            _assignKeyText.text = $"Assign key to <color=#{_assignedKeyTextColor.HexCode}>{bindingPanel.ActionId}</color>...";
            InputManager.AssignKey(_currentlyAssignedPanel.ActionId, alt, OnKeyAssigned);
        }

        private void UpdateAllBindingsPanels()
        {
            int i = 0;
            System.Collections.Generic.Dictionary<string, (KeyCode btn, KeyCode altBtn)> allBindings = InputManager.GetMapCopy();

            foreach (System.Collections.Generic.KeyValuePair<string, (KeyCode btn, KeyCode altBtn)> binding in allBindings)
                _bindingPanels[i++].Init(binding.Key, binding.Value);

            for (; i < _bindingPanels.Length; ++i)
                _bindingPanels[i].Hide();
        }

        private void ResetBindings()
        {
            InputManager.RestoreDefaultMap();
            UpdateAllBindingsPanels();
        }

        protected override void Start()
        {
            base.Start();

            _resetBindingsBtn.onClick.AddListener(ResetBindings);

            for (int i = 0; i < _bindingPanels.Length; ++i)
            {
                KeyBindingPanel panel = _bindingPanels[i];
                _bindingPanels[i].BaseBtnButton.onClick.AddListener(() => AssignKey(panel, false));
                _bindingPanels[i].AltBtnButton.onClick.AddListener(() => AssignKey(panel, true));
            }

            UpdateAllBindingsPanels();
            InitBindingsPanelsNavigation();
        }

        private void OnDestroy()
        {
            _resetBindingsBtn.onClick.RemoveAllListeners();
        }

        [ContextMenu("Locate binding panels")]
        private void LocateBindingPanels()
        {
            _bindingPanels = FindObjectsOfType<KeyBindingPanel>().OrderBy(o => o.transform.GetSiblingIndex()).ToArray();
#if UNITY_EDITOR
            RSLib.EditorUtilities.PrefabEditorUtilities.SetCurrentPrefabStageDirty();
#endif
        }
    }
}