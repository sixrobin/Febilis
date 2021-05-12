namespace Templar.UI
{
    using RSLib.Framework.InputSystem;
    using UnityEngine;

    public class ControlsPanel : MonoBehaviour
    {
        [SerializeField] private KeyBindingPanel[] _bindingPanels = null;
        [SerializeField] private GameObject _assignKeyScreen = null;
        [SerializeField] private TMPro.TextMeshProUGUI _assignKeyText = null;
        [SerializeField] private UnityEngine.UI.Button _resetBindingsBtn = null;

        private KeyBindingPanel _currentlyAssignedPanel;

        public bool Displayed { get; private set; }

        private void OnKeyAssigned(string actionId, KeyCode btn, bool alt)
        {
            UnityEngine.Assertions.Assert.IsNotNull(_currentlyAssignedPanel, "Trying to assign button to a null panel.");
            UnityEngine.Assertions.Assert.IsTrue(actionId == _currentlyAssignedPanel.ActionId, "Assigned panel action Id and system assigned action Id are not the same.");

            _currentlyAssignedPanel.OverrideKey(btn, alt);
            _currentlyAssignedPanel = null;
            _assignKeyScreen.SetActive(false);
        }

        private void AssignKey(KeyBindingPanel bindingPanel, bool alt)
        {
            _currentlyAssignedPanel = bindingPanel;
            _assignKeyScreen.SetActive(true);
            _assignKeyText.text = $"Assign key to {bindingPanel.ActionId}...";
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
            _bindingPanels = FindObjectsOfType<KeyBindingPanel>();
            System.Array.Reverse(_bindingPanels);
        }
    }
}