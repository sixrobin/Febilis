namespace Templar.UI
{
    using RSLib.Framework.InputSystem;
    using UnityEngine;

    public class ControlsPanel : MonoBehaviour
    {
        [SerializeField] private KeyBindingPanel[] _bindingPanels = null;
        [SerializeField] private GameObject _assignKeyScreen = null;
        [SerializeField] private TMPro.TextMeshProUGUI _assignKeyText = null;

        private KeyBindingPanel _currentlyAssignedPanel;

        public bool Displayed { get; private set; }

        private void OnKeyAssigned(InputAction action, KeyCode btn, bool alt)
        {
            UnityEngine.Assertions.Assert.IsNotNull(_currentlyAssignedPanel, "Trying to assign button to a null panel.");
            UnityEngine.Assertions.Assert.IsTrue(action == _currentlyAssignedPanel.Action, "Assigned panel action and system assigned action are not the same.");

            _currentlyAssignedPanel.OverrideKey(btn, alt);
            _currentlyAssignedPanel = null;
            _assignKeyScreen.SetActive(false);
        }

        private void AssignKey(KeyBindingPanel bindingPanel, bool alt)
        {
            _currentlyAssignedPanel = bindingPanel;
            _assignKeyScreen.SetActive(true);
            _assignKeyText.text = $"Assign key to {bindingPanel.Action.ToString()}...";
            InputManager.AssignKey(_currentlyAssignedPanel.Action, alt, OnKeyAssigned);
        }

        [ContextMenu("Locate binding panels")]
        private void LocateBindingPanels()
        {
            _bindingPanels = FindObjectsOfType<KeyBindingPanel>();
            System.Array.Reverse(_bindingPanels);
        }

        private void Start()
        {
            int i = 0;
            for (; i < _bindingPanels.Length; ++i)
            {
                KeyBindingPanel panel = _bindingPanels[i];
                _bindingPanels[i].BaseBtnButton.onClick.AddListener(() => AssignKey(panel, false));
                _bindingPanels[i].AltBtnButton.onClick.AddListener(() => AssignKey(panel, true));
            }

            i = 0;
            System.Collections.Generic.Dictionary<InputAction, (KeyCode btn, KeyCode altBtn)> allBindings = InputManager.GetMapCopy();

            foreach (System.Collections.Generic.KeyValuePair<InputAction, (KeyCode btn, KeyCode altBtn)> binding in allBindings)
                _bindingPanels[i++].Init(binding.Key, binding.Value);

            for (; i < _bindingPanels.Length; ++i)
                _bindingPanels[i].Hide();
        }
    }
}