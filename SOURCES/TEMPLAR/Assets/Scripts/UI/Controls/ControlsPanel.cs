namespace Templar.UI
{
    using RSLib.Framework.InputSystem;
    using System.Linq;
    using UnityEngine;

    public class ControlsPanel : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas = null;
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
                    _rectTransform = _canvas.GetComponent<RectTransform>();

                return _rectTransform;
            }
        }

        public bool Displayed { get; private set; }

        public void Display(bool show)
        {
            Displayed = show;
            _canvas.enabled = Displayed;

            if (show)
            {
                UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(RectTransform);
                _controlsScrollBar.value = 1f;
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

        private void Start()
        {
            _resetBindingsBtn.onClick.AddListener(ResetBindings);

            for (int i = 0; i < _bindingPanels.Length; ++i)
            {
                KeyBindingPanel panel = _bindingPanels[i];
                _bindingPanels[i].BaseBtnButton.onClick.AddListener(() => AssignKey(panel, false));
                _bindingPanels[i].AltBtnButton.onClick.AddListener(() => AssignKey(panel, true));
            }

            UpdateAllBindingsPanels();
        }

        private void OnDestroy()
        {
            _resetBindingsBtn.onClick.RemoveAllListeners();
        }

        [ContextMenu("Locate binding panels")]
        private void LocateBindingPanels()
        {
            _bindingPanels = FindObjectsOfType<KeyBindingPanel>().OrderBy(o => o.transform.GetSiblingIndex()).ToArray();
        }
    }
}