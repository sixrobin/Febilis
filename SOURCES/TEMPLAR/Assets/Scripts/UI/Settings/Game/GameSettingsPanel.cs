namespace Templar.UI.Settings.Game
{
    using RSLib.Extensions;
    using System.Linq;
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    public class GameSettingsPanel : SettingsPanelBase
    {
        private const float SCROLL_BAR_AUTO_REFRESH_VALUE = 0.05f;
        private const float SCROLL_BAR_AUTO_REFRESH_MARGIN = 0.1f;

        [Header("GAME SETTINGS")]
        [SerializeField] private SettingView[] _settings = null;
        [Space(10f)]
        [SerializeField] private UnityEngine.UI.Button _resetSettingsBtn = null;
        [SerializeField] private UnityEngine.UI.Button _saveSettingsBtn = null;

        [Header("UI NAVIGATION")]
        [SerializeField] private RectTransform _settingsViewport = null;
        [SerializeField] private UnityEngine.UI.Scrollbar _scrollbar = null;
        [SerializeField] private RectTransform _scrollHandle = null;

        private bool _initialized;

        private Vector3[] _settingsViewportWorldCorners = new Vector3[4];

        public override GameObject FirstSelected => _settings.Where(o => o.gameObject.activeSelf).FirstOrDefault()?.gameObject;

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
            bool displayUpdated = UpdateOptionsDisplay();

            if (_initialized && !displayUpdated)
                return;

            if (!_initialized)
                for (int i = _settings.Length - 1; i >= 0; --i)
                    _settings[i].Init();

            if (displayUpdated || !_initialized)
                InitNavigation();

            _initialized = true;
        }

        private bool UpdateOptionsDisplay()
        {
            bool anyChange = false;

            for (int i = _settings.Length - 1; i >= 0; --i)
            {
                if (_settings[i].Setting.CanBeDisplayedToUser() != _settings[i].gameObject.activeSelf)
                {
                    _settings[i].gameObject.SetActive(!_settings[i].gameObject.activeSelf);
                    anyChange = true;
                }
            }

            return anyChange;
        }

        private void InitNavigation()
        {
            SettingView[] enabledSettings = _settings.Where(o => o.gameObject.activeSelf).ToArray();

            for (int i = 0; i < enabledSettings.Length; ++i)
            {
                bool first = i == 0;
                bool last = i == enabledSettings.Length - 1;

                enabledSettings[i].Selectable.SetMode(UnityEngine.UI.Navigation.Mode.Explicit);
                enabledSettings[i].PointerEventsHandler.PointerEnter += OnSettingPointerEnter;

                if (!(enabledSettings[i].Selectable is UnityEngine.UI.Slider))
                    enabledSettings[i].Selectable.SetSelectOnRight(_scrollbar);

                if (first)
                {
                    enabledSettings[i].Selectable.SetSelectOnUp(BackBtn);
                    enabledSettings[i].Selectable.SetSelectOnUp(QuitBtn);

                    BackBtn.SetSelectOnDown(enabledSettings[i]);
                    QuitBtn.SetSelectOnDown(enabledSettings[i]);
                }
                else
                {
                    enabledSettings[i].Selectable.SetSelectOnUp(enabledSettings[i - 1]);
                }

                if (!last)
                {
                    enabledSettings[i].Selectable.SetSelectOnDown(enabledSettings[i + 1]);
                    continue;
                }

                _resetSettingsBtn.SetMode(UnityEngine.UI.Navigation.Mode.Explicit);
                _saveSettingsBtn.SetMode(UnityEngine.UI.Navigation.Mode.Explicit);

                _resetSettingsBtn.SetSelectOnUp(enabledSettings[i]);
                _saveSettingsBtn.SetSelectOnUp(enabledSettings[i]);

                enabledSettings[i].Selectable.SetSelectOnDown(_saveSettingsBtn);

                break;
            }
        }

        private void OnSettingPointerEnter(RSLib.Framework.GUI.PointerEventsHandler pointerEventsHandler)
        {
            RSLib.Helpers.AdjustScrollViewToFocusedItem(pointerEventsHandler.RectTransform,
                                                        _settingsViewport,
                                                        _scrollbar,
                                                        SCROLL_BAR_AUTO_REFRESH_VALUE,
                                                        SCROLL_BAR_AUTO_REFRESH_MARGIN);
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