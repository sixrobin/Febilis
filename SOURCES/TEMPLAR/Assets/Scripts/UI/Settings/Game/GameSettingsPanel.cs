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
        private const float SCROLL_BAR_AUTO_REFRESH_VALUE = 0.02f;
        private const float SCROLL_BAR_AUTO_REFRESH_MARGIN = 0.05f;

        [Header("GAME SETTINGS")]
        [SerializeField] private SettingView[] _settings = null;
        [Space(10f)]
        [SerializeField] private UnityEngine.UI.Button _resetSettingsBtn = null;
        [SerializeField] private UnityEngine.UI.Button _saveSettingsBtn = null;

        [Header("UI NAVIGATION")]
        [SerializeField] private RectTransform _settingsViewport = null;
        [SerializeField] private UnityEngine.UI.Scrollbar _scrollbar = null;

        private bool _initialized;

        private Vector3[] _settingsViewportWorldCorners = new Vector3[4];

        public override GameObject FirstSelected => _settings.Where(o => o.Visible).FirstOrDefault()?.gameObject;

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
            SettingView[] enabledSettings = _settings.Where(o => o.Visible).ToArray();

            for (int i = 0; i < enabledSettings.Length; ++i)
            {
                bool first = i == 0;
                bool last = i == enabledSettings.Length - 1;

                enabledSettings[i].Selectable.SetMode(UnityEngine.UI.Navigation.Mode.Explicit);
                enabledSettings[i].PointerEventsHandler.PointerEnter += OnSettingPointerEnter;

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
            // Automatically adjust the scroll view content position so that navigating through the settings with a controller
            // works without having to move the scroll bar manually.
            // This is also handling mouse hovering for now.

            Vector3[] sourceCorners = new Vector3[4];
            pointerEventsHandler.RectTransform.GetWorldCorners(sourceCorners);
            _settingsViewport.GetWorldCorners(_settingsViewportWorldCorners);

            while (sourceCorners[1].y > _settingsViewportWorldCorners[1].y)
            {
                _scrollbar.value += SCROLL_BAR_AUTO_REFRESH_VALUE;
                pointerEventsHandler.RectTransform.GetWorldCorners(sourceCorners);
            }

            while (sourceCorners[0].y < _settingsViewportWorldCorners[0].y)
            {
                _scrollbar.value -= SCROLL_BAR_AUTO_REFRESH_VALUE;
                pointerEventsHandler.RectTransform.GetWorldCorners(sourceCorners);
            }

            if (_scrollbar.value - SCROLL_BAR_AUTO_REFRESH_MARGIN < 0f)
                _scrollbar.value = 0f;
            else if (_scrollbar.value + SCROLL_BAR_AUTO_REFRESH_MARGIN > 1f)
                _scrollbar.value = 1f;
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