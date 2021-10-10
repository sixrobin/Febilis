namespace Templar.UI.Settings.Game
{
    using RSLib.Extensions;
    using UnityEditor;
#if UNITY_EDITOR
    using UnityEngine;
#endif

    public class GameSettingsPanel : SettingsPanelBase
    {
        [Header("GAME SETTINGS")]
        [SerializeField] private SettingView[] _settings = null;
        [SerializeField] private UnityEngine.UI.Button _resetSettingsBtn = null;
        [SerializeField] private UnityEngine.UI.Button _saveSettingsBtn = null;

        private bool _initialized;
        
        public override GameObject FirstSelected => _settings.Length > 0 ? _settings[0].gameObject : null;

        public override void OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Manager.OptionsManager.Instance.OpenSettings(this);
        }

        public override void Display(bool show)
        {
            base.Display(show);

            if (show)
                Init();
        }

        private void Init()
        {
            if (_initialized)
                return;

            for (int i = _settings.Length - 1; i >= 0; --i)
            {
                _settings[i].Init();
                _settings[i].gameObject.SetActive(_settings[i].Visible);
            }

            InitNavigation();

            _initialized = true;
        }

        private void InitNavigation()
        {
            for (int i = 0; i < _settings.Length; ++i)
            {
                bool first = i == 0;
                bool last = i == _settings.Length - 1 || !_settings[i + 1].Visible;

                _settings[i].Selectable.SetMode(UnityEngine.UI.Navigation.Mode.Explicit);

                if (first)
                {
                    _settings[i].Selectable.SetSelectOnUp(BackBtn);
                    _settings[i].Selectable.SetSelectOnUp(QuitBtn);

                    BackBtn.SetSelectOnDown(_settings[i].Selectable);
                    QuitBtn.SetSelectOnDown(_settings[i].Selectable);
                }
                else
                {
                    _settings[i].Selectable.SetSelectOnUp(_settings[i - 1]);
                }

                if (!last)
                {
                    _settings[i].Selectable.SetSelectOnDown(_settings[i + 1]);
                    continue;
                }

                _resetSettingsBtn.SetMode(UnityEngine.UI.Navigation.Mode.Explicit);
                _saveSettingsBtn.SetMode(UnityEngine.UI.Navigation.Mode.Explicit);

                _resetSettingsBtn.SetSelectOnUp(_settings[i].Selectable);
                _saveSettingsBtn.SetSelectOnUp(_settings[i].Selectable);

                _settings[i].Selectable.SetSelectOnDown(_saveSettingsBtn);

                break;
            }
        }

        private void ResetSettings()
        {
            Manager.SettingsManager.Init();

            for (int i = _settings.Length - 1; i >= 0; --i)
                _settings[i].Init();
        }

        protected override void Start()
        {
            base.Start();

            _resetSettingsBtn.onClick.AddListener(ResetSettings);
            _saveSettingsBtn.onClick.AddListener(Manager.SettingsManager.Save);
            _saveSettingsBtn.onClick.AddListener(OnBackButtonPressed);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _resetSettingsBtn.onClick.RemoveListener(ResetSettings);
            _saveSettingsBtn.onClick.RemoveListener(Manager.SettingsManager.Save);
            _saveSettingsBtn.onClick.RemoveListener(OnBackButtonPressed);
        }
    
        public void LocateSettings()
        {
            _settings = transform.parent.GetComponentsInChildren<SettingView>();
            RSLib.EditorUtilities.PrefabEditorUtilities.SetCurrentPrefabStageDirty();
            RSLib.EditorUtilities.SceneManagerUtilities.SetCurrentSceneDirty();
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(GameSettingsPanel))]
    public class GameSettingsPanelEditor : RSLib.EditorUtilities.ButtonProviderEditor<GameSettingsPanel>
    {
        protected override void DrawButtons()
        {
            DrawButton("Locate Settings", Obj.LocateSettings);
        }
    }
#endif
}