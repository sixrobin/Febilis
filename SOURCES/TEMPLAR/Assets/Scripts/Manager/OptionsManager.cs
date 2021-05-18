namespace Templar.Manager
{
    using UnityEngine;

    public class OptionsManager : RSLib.Framework.ConsoleProSingleton<OptionsManager>
    {
        [SerializeField] private UI.ControlsPanel _controlsPanel = null;
        [SerializeField] private GameObject _selectedObjectOnOpen = null;

        public delegate void OptionsToggleEventHandler();

        public event OptionsToggleEventHandler OptionsOpened;
        public event OptionsToggleEventHandler OptionsClosed;

        public static bool OptionsPanelDisplayed { get; private set; }

        public static bool CanToggleOptions()
        {
            return !RSLib.Framework.InputSystem.InputManager.IsAssigningKey;
        }

        public void Open()
        {
            OptionsPanelDisplayed = true;
            _controlsPanel.Display(true);

            UI.Navigation.UINavigationManager.Select(_selectedObjectOnOpen);

            OptionsOpened?.Invoke();
        }

        public void Close()
        {
            OptionsPanelDisplayed = false;
            _controlsPanel.Display(false);
            RSLib.Framework.InputSystem.InputManager.SaveCurrentMap();

            OptionsClosed?.Invoke();
        }

        private void Update()
        {
            if (!CanToggleOptions())
                return;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (OptionsPanelDisplayed)
                    Close();
                else
                    Open();
            }
        }
    }
}