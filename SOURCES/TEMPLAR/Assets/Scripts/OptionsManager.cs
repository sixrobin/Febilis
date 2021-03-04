using UnityEngine;

public class OptionsManager : MonoBehaviour
{
    [SerializeField] private GameObject _optionsPanel = null;

    public bool OptionsPanelDisplayed { get; private set; }

    public bool CanToggleOptions()
    {
        return !RSLib.Framework.InputSystem.InputManager.IsAssigningKey;
    }

    private void Update()
    {
        if (!CanToggleOptions())
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OptionsPanelDisplayed = !OptionsPanelDisplayed;
            _optionsPanel.SetActive(OptionsPanelDisplayed);
        }
    }
}