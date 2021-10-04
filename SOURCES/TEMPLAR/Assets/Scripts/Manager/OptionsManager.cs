namespace Templar.Manager
{
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    public class OptionsManager : RSLib.Framework.ConsoleProSingleton<OptionsManager>
    {
        [Header("OPTIONS PANELS")]
        [SerializeField] private UI.Options.SettingsPanel _settingsPanel = null;
        [SerializeField] private UI.Options.Controls.ControlsPanel _controlsPanel = null;
        [SerializeField] private UI.Options.OptionPanelBase _videoPanel = null;
        [SerializeField] private UI.Options.OptionPanelBase _audioPanel = null;
        [SerializeField] private UI.Options.OptionPanelBase _languagePanel = null;

        public delegate void OptionsToggleEventHandler();

        public event OptionsToggleEventHandler OptionsOpened;
        public event OptionsToggleEventHandler OptionsClosed;

        public static bool ClosedThisFrame { get; private set; }

        public static bool AnyPanelOpen()
        {
            // [TODO] Remove those null checks once panels are implemented.
            return Instance._settingsPanel.Displayed
                || Instance._controlsPanel.Displayed
                || (Instance._videoPanel?.Displayed ?? false)
                || (Instance._audioPanel?.Displayed ?? false)
                || (Instance._languagePanel?.Displayed ?? false);
        }

        public static bool AnyPanelOpenOrClosedThisFrame()
        {
            return ClosedThisFrame || AnyPanelOpen();
        }

        public static bool CanToggleOptions()
        {
            return !RSLib.Framework.InputSystem.InputManager.IsAssigningKey
                && !UI.Dialogue.DialogueManager.DialogueRunning
                && !Manager.BoardsTransitionManager.IsInBoardTransition
                && (!GameManager.InventoryView?.Displayed ?? true);
        }

        public void OpenSettings()
        {
            if (!AnyPanelOpen())
                OptionsOpened?.Invoke();

            UI.Navigation.UINavigationManager.OpenAndSelect(_settingsPanel);
            _settingsPanel.Display(true);
        }

        public void Close()
        {
            if (!ClosedThisFrame)
                StartCoroutine(CloseAtEndOfFrame());
        }

        private void InitOptionsButtons()
        {
            _settingsPanel.ControlsBtn.onClick.AddListener(OpenControlsPanel);
            //_settingsPanel.VideoBtn.onClick.AddListener(OpenVideoPanel);
            //_settingsPanel.AudioBtn.onClick.AddListener(OpenAudioPanel);
            //_settingsPanel.LanguageBtn.onClick.AddListener(OpenLanguagePanel);
        }

        private void CleanupOptionsButtons()
        {
            _settingsPanel.ControlsBtn.onClick.RemoveListener(OpenControlsPanel);
            //_settingsPanel.VideoBtn.onClick.RemoveListener(OpenVideoPanel);
            //_settingsPanel.AudioBtn.onClick.RemoveListener(OpenAudioPanel);
            //_settingsPanel.LanguageBtn.onClick.RemoveListener(OpenLanguagePanel);
        }

        private void OpenControlsPanel()
        {
            _settingsPanel.Display(false);
            UI.Navigation.UINavigationManager.OpenAndSelect(_controlsPanel);
        }

        private void OpenVideoPanel()
        {
            _settingsPanel.Display(false);
            UI.Navigation.UINavigationManager.OpenAndSelect(_videoPanel);
        }

        private void OpenAudioPanel()
        {
            _settingsPanel.Display(false);
            UI.Navigation.UINavigationManager.OpenAndSelect(_audioPanel);
        }

        private void OpenLanguagePanel()
        {
            _settingsPanel.Display(false);
            UI.Navigation.UINavigationManager.OpenAndSelect(_languagePanel);
        }

        private System.Collections.IEnumerator CloseAtEndOfFrame()
        {
            ClosedThisFrame = true;

            yield return RSLib.Yield.SharedYields.WaitForEndOfFrame;

            UI.Navigation.UINavigationManager.CloseCurrentPanel();
            UI.Navigation.UINavigationManager.NullifySelected();
            ClosedThisFrame = false;

            OptionsClosed?.Invoke();
        }

        protected override void Awake()
        {
            base.Awake();
            InitOptionsButtons();
        }

        private void Update()
        {
            if (!CanToggleOptions())
                return;

            if (Input.GetButtonDown("Menu")) // [TODO] Constant.
            {
                if (!AnyPanelOpen())
                {
                    OpenSettings();
                    return;
                }

                UI.Navigation.UINavigationManager.CloseCurrentPanel();
            }
        }

        private void OnDestroy()
        {
            CleanupOptionsButtons();
        }

        [ContextMenu("Find All References")]
        private void DebugFindAllReferences()
        {
            _settingsPanel = FindObjectOfType<UI.Options.SettingsPanel>();
            _controlsPanel = FindObjectOfType<UI.Options.Controls.ControlsPanel>();
            RSLib.EditorUtilities.SceneManagerUtilities.SetCurrentSceneDirty();
        }

        [ContextMenu("Find Missing References")]
        private void DebugFindMissingReferences()
        {
            _settingsPanel = _settingsPanel ?? FindObjectOfType<UI.Options.SettingsPanel>();
            _controlsPanel = _controlsPanel ?? FindObjectOfType<UI.Options.Controls.ControlsPanel>();
            RSLib.EditorUtilities.SceneManagerUtilities.SetCurrentSceneDirty();
        }
    }
}