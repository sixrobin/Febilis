namespace Templar.UI.Settings.Game
{
    using RSLib.Extensions;
    using System.Linq;
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    public class GameSettingsPanel : SettingsPanelBase, IScrollViewClosestItemGetter
    {
        private const float SCROLL_BAR_AUTO_REFRESH_VALUE = 0.05f;
        private const float SCROLL_BAR_AUTO_REFRESH_MARGIN = 0.1f;

        [Header("GAME SETTINGS")]
        [SerializeField] private TMPro.TextMeshProUGUI _title = null;
        [SerializeField] private SettingView[] _settings = null;
        [Space(10f)]
        [SerializeField] private RSLib.Framework.GUI.EnhancedButton _resetSettingsBtn = null;
        [SerializeField] private RSLib.Framework.GUI.EnhancedButton _saveSettingsBtn = null;

        [Header("UI NAVIGATION")]
        [SerializeField] private RectTransform _settingsViewport = null;
        [SerializeField] private UnityEngine.UI.Scrollbar _scrollbar = null;
        [SerializeField] private RectTransform _scrollHandle = null;
        [SerializeField] private ScrollbarToScrollViewNavigationHandler _scrollbarToScrollViewNavigationHandler = null;

        private bool _initialized;

        public override GameObject FirstSelected => _settings.FirstOrDefault(o => o.gameObject.activeSelf)?.gameObject;

        public ScrollbarToScrollViewNavigationHandler ScrollbarToScrollViewNavigationHandler => _scrollbarToScrollViewNavigationHandler;

        public override void OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Manager.OptionsManager.Instance.OpenSettings(this);
        }

        public override void Display(bool show)
        {
            base.Display(show);

            if (show)
            {
                Init();
                Localize();
            }
        }

        public GameObject GetClosestItemToScrollbar()
        {
            RectTransform closestSlot = null;
            float sqrClosestDist = Mathf.Infinity;

            Vector3[] scrollHandleWorldCorners = new Vector3[4];
            _scrollHandle.GetWorldCorners(scrollHandleWorldCorners);
            Vector3 scrollHandleCenterWorld = RSLib.Maths.Maths.ComputeAverageVector(scrollHandleWorldCorners);

            foreach (RectTransform target in _settings.Where(o => o.gameObject.activeSelf).Select(o => o.GetComponent<RectTransform>()))
            {
                Vector3[] slotWorldCorners = new Vector3[4];
                target.GetWorldCorners(slotWorldCorners);
                Vector3 slotCenterWorld = RSLib.Maths.Maths.ComputeAverageVector(slotWorldCorners);

                float sqrTargetDist = (slotCenterWorld - scrollHandleCenterWorld).sqrMagnitude;
                if (sqrTargetDist > sqrClosestDist)
                    continue;

                sqrClosestDist = sqrTargetDist;
                closestSlot = target;
            }

            return closestSlot.gameObject;
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

            _resetSettingsBtn.SetSelectOnRight(_saveSettingsBtn);
            _saveSettingsBtn.SetSelectOnLeft(_resetSettingsBtn);
            
            _resetSettingsBtn.SetSelectOnDown(_backBtn);
            _backBtn.SetSelectOnUp(_resetSettingsBtn);
            _saveSettingsBtn.SetSelectOnDown(_quitBtn);
            _quitBtn.SetSelectOnUp(_saveSettingsBtn);
            
            _scrollbar.SetMode(UnityEngine.UI.Navigation.Mode.Explicit);
            _scrollbar.SetSelectOnLeft(ScrollbarToScrollViewNavigationHandler);
            ScrollbarToScrollViewNavigationHandler.SetClosestItemGetter(this);
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

        private void Localize()
        {
            _title.text = RSLib.Localization.Localizer.Get(Localization.Settings.GAME_TITLE);
            _resetSettingsBtn.SetText(RSLib.Localization.Localizer.Get(Localization.Settings.GAME_RESET));
            _saveSettingsBtn.SetText(RSLib.Localization.Localizer.Get(Localization.Settings.GAME_SAVE));

            for (int i = _settings.Length - 1; i >= 0; --i)
                _settings[i].Localize();
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