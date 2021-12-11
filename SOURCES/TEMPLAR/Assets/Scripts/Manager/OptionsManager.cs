namespace Templar.Manager
{
    using Templar.Datas;
    using UnityEngine;

    public class OptionsManager : RSLib.Framework.ConsoleProSingleton<OptionsManager>
    {
        [Header("OPTIONS PANELS")]
        [SerializeField] private UI.Settings.SettingsHubPanel _settingsHubPanel = null;
        [SerializeField] private UI.Settings.Controls.ControlsPanel _controlsPanel = null;
        [SerializeField] private UI.Settings.Game.GameSettingsPanel _gamePanel = null;
        [SerializeField] private UI.Settings.SettingsPanelBase _audioPanel = null;
        [SerializeField] private UI.Settings.SettingsPanelBase _languagePanel = null;

        [Header("BACK TO MENU")]
        [SerializeField] private RampFadeDatas _backToMenuFadeInDatas = null;

        private System.Collections.Generic.Dictionary<UI.Settings.SettingsPanelBase, UnityEngine.UI.Button> _panelsBtns;

        private UI.ConfirmationPopup.PopupTextsDatas _quitGamePopupTexts
            = new UI.ConfirmationPopup.PopupTextsDatas("Quit game and go back to main menu ?", "YES", "NO");

        public delegate void OptionsToggleEventHandler();
        public event OptionsToggleEventHandler OptionsOpened;
        public event OptionsToggleEventHandler OptionsClosed;

        public static bool ClosedThisFrame { get; private set; }

        public static void QuitGame()
        {
            UI.Navigation.UINavigationManager.ConfirmationPopup.AskForConfirmation(
                Instance._quitGamePopupTexts,
                () =>
                {
                    UI.Navigation.UINavigationManager.SetPanelAsCurrent(Instance._settingsHubPanel);
                    UI.Navigation.UINavigationManager.CurrentlyOpenPanel.OnBackButtonPressed();

                    GameManager.PlayerCtrl.AllowInputs(false);

                    RampFadeManager.Fade(GameManager.CameraCtrl.GrayscaleRamp,
                                         Instance._backToMenuFadeInDatas,
                                         (0f, 1f),
                                         (fadeIn) => ScenesManager.LoadScene(ScenesManager.MainMenuScene));
                },
                () =>
                {
                    UI.Navigation.UINavigationManager.Select(Instance._settingsHubPanel.QuitGameBtn.gameObject);
                });
        }

        public static bool AnyPanelOpen()
        {
            // [TODO] Remove those null checks once panels are implemented.
            return Instance._settingsHubPanel.Displayed
                || Instance._controlsPanel.Displayed
                || Instance._gamePanel.Displayed
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
                && GameManager.Exists() && (!GameManager.InventoryView?.Displayed ?? true);
        }

        public void OpenSettings(UI.Settings.SettingsPanelBase sourcePanel = null)
        {
            if (!AnyPanelOpen())
                OptionsOpened?.Invoke();

            UI.Navigation.UINavigationManager.OpenAndSelect(_settingsHubPanel);
            if (sourcePanel != null && _panelsBtns.TryGetValue(sourcePanel, out UnityEngine.UI.Button btn) && btn != null)
                UI.Navigation.UINavigationManager.Select(btn.gameObject);

            _settingsHubPanel.Display(true);
        }

        public void Close()
        {
            if (!ClosedThisFrame)
                StartCoroutine(CloseAtEndOfFrame());
        }

        private void InitOptionsButtons()
        {
            _settingsHubPanel.ControlsBtn.onClick.AddListener(OpenControlsPanel);
            _settingsHubPanel.GameBtn.onClick.AddListener(OpenGamePanel);
            //_settingsPanel.AudioBtn.onClick.AddListener(OpenAudioPanel);
            //_settingsPanel.LanguageBtn.onClick.AddListener(OpenLanguagePanel);
            _settingsHubPanel.QuitGameBtn.onClick.AddListener(QuitGame);
        }

        private void CleanupOptionsButtons()
        {
            _settingsHubPanel.ControlsBtn.onClick.RemoveListener(OpenControlsPanel);
            _settingsHubPanel.GameBtn.onClick.RemoveListener(OpenGamePanel);
            //_settingsPanel.AudioBtn.onClick.RemoveListener(OpenAudioPanel);
            //_settingsPanel.LanguageBtn.onClick.RemoveListener(OpenLanguagePanel);
            _settingsHubPanel.QuitGameBtn.onClick.RemoveListener(QuitGame);
        }

        private void OpenControlsPanel()
        {
            _settingsHubPanel.Display(false);
            UI.Navigation.UINavigationManager.OpenAndSelect(_controlsPanel);
        }

        private void OpenGamePanel()
        {
            _settingsHubPanel.Display(false);
            UI.Navigation.UINavigationManager.OpenAndSelect(_gamePanel);
        }

        private void OpenAudioPanel()
        {
            _settingsHubPanel.Display(false);
            UI.Navigation.UINavigationManager.OpenAndSelect(_audioPanel);
        }

        private void OpenLanguagePanel()
        {
            _settingsHubPanel.Display(false);
            UI.Navigation.UINavigationManager.OpenAndSelect(_languagePanel);
        }

        private System.Collections.IEnumerator CloseAtEndOfFrame()
        {
            ClosedThisFrame = true;

            yield return RSLib.Yield.SharedYields.WaitForEndOfFrame;

            UI.Navigation.UINavigationManager.CloseCurrentPanel();
            UI.Navigation.UINavigationManager.NullifySelected();

            ClosedThisFrame = false;

            SettingsManager.Save();

            OptionsClosed?.Invoke();
        }

        protected override void Awake()
        {
            base.Awake();

            // OptionsManager is not destroyed on load so we need to retrieve it on each loading.
            if (_settingsHubPanel == null)
                _settingsHubPanel = FindObjectOfType<UI.Settings.SettingsHubPanel>();

            _panelsBtns = new System.Collections.Generic.Dictionary<UI.Settings.SettingsPanelBase, UnityEngine.UI.Button>()
            {
                { _controlsPanel, _settingsHubPanel.ControlsBtn },
                { _gamePanel, _settingsHubPanel.GameBtn },
                //{ _audioPanel, _settingsHubPanel.AudioBtn },
                //{ _languagePanel, _settingsHubPanel.LanguageBtn }
            };

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
            _settingsHubPanel = FindObjectOfType<UI.Settings.SettingsHubPanel>();
            _controlsPanel = FindObjectOfType<UI.Settings.Controls.ControlsPanel>();
            _gamePanel = FindObjectOfType<UI.Settings.Game.GameSettingsPanel>();

            RSLib.EditorUtilities.SceneManagerUtilities.SetCurrentSceneDirty();
        }

        [ContextMenu("Find Missing References")]
        private void DebugFindMissingReferences()
        {
            _settingsHubPanel = _settingsHubPanel ?? FindObjectOfType<UI.Settings.SettingsHubPanel>();
            _controlsPanel = _controlsPanel ?? FindObjectOfType<UI.Settings.Controls.ControlsPanel>();
            _gamePanel = _gamePanel ?? FindObjectOfType<UI.Settings.Game.GameSettingsPanel>();

            RSLib.EditorUtilities.SceneManagerUtilities.SetCurrentSceneDirty();
        }
    }
}