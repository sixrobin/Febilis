namespace Templar.MainMenu
{
    using RSLib.Extensions;
    using RSLib.Maths;
    using System.Linq;
    using UnityEngine;

    public class MainMenuPanel : UI.UIPanel
    {
        [Header("REFS")]
        [SerializeField] private UI.Settings.SettingsHubPanel _settingsHubPanel = null;
        [SerializeField] private UnityEngine.UI.Image _title = null;
        [SerializeField] private UnityEngine.UI.Image _titleShadow = null;
        [SerializeField] private Transform _vignette = null;
        [SerializeField] private GameObject _pressAnyKey = null;

        [Header("BUTTONS REFS")]
        [SerializeField] private GameObject _btnsContainer = null;
        [SerializeField] private MainMenuButton _continueBtn = null;
        [SerializeField] private MainMenuButton _newGameBtn = null;
        [SerializeField] private MainMenuButton _settingsBtn = null;
        [SerializeField] private MainMenuButton _quitBtn = null;

        [Header("FADES DATAS")]
        [SerializeField] private Datas.MainMenu.MainMenuFadeOutDatas _mainMenuFadeOutDatas = null;
        [SerializeField] private Datas.MainMenu.MainMenuFadeInDatas _mainMenuFadeInDatas = null;
        [SerializeField] private RSLib.ImageEffects.CameraGrayscaleRamp _grayscaleRamp = null;
        [SerializeField] private Datas.RampFadeDatas _rampFadeOutDatas = null;

        [Header("PRESS ANY KEY")]
        [SerializeField] private float _pressAnyKeyBlinkSpeed = 0.5f;
        [SerializeField] private float _pressAnyKeyPostDelay = 0.5f;

        private MainMenuButton[] _allBtns;
        private MainMenuButton _lastSelectedBtn;

        private System.Collections.IEnumerator _vignetteFadeCoroutine;

        private UI.ConfirmationPopup.PopupTextsDatas _eraseSaveFilePopupTexts
            = new UI.ConfirmationPopup.PopupTextsDatas("A save file already exists.\nAre you sure you want to overwrite it ?", "YES", "NO");

        public override GameObject FirstSelected => FirstButtonSelected.gameObject;

        public MainMenuButton FirstButtonSelected => Manager.SaveManager.GameSaveExist ? _continueBtn : _newGameBtn;

        public override void OnBackButtonPressed()
        {
        }

        private void OnOptionsOpened()
        {
            DisplayButtons(false);
            DisplayTitle(false);
        }

        private void OnOptionsClosed()
        {
            DisplayButtons(true);
            DisplayTitle(true);

            UI.Navigation.UINavigationManager.Select(_settingsBtn.gameObject);
        }

        private void OnNewGameButtonPressed()
        {
            if (Manager.SaveManager.GameSaveExist)
            {
                UI.Navigation.UINavigationManager.ConfirmationPopup.AskForConfirmation(
                    _eraseSaveFilePopupTexts,
                    OnNewGameConfirmed,
                    () => UI.Navigation.UINavigationManager.Select(_newGameBtn.gameObject));
            }
            else
            {
                OnNewGameConfirmed();
            }
        }

        private void OnNewGameConfirmed()
        {
            UI.Navigation.UINavigationManager.NullifySelected();
            DisplayButtons(false);
            DisplayTitle(false);

            GlobalFadeOutInCoroutine(Manager.MainMenuManager.NewGame);
        }

        private void OnContinueButtonPressed()
        {
            UI.Navigation.UINavigationManager.NullifySelected();
            DisplayButtons(false);
            DisplayTitle(false);

            GlobalFadeOutInCoroutine(Manager.MainMenuManager.LoadSavedGame);
        }

        private void OnSettingsButtonPressed()
        {
            Manager.OptionsManager.Instance.OpenSettings();
        }

        private void OnQuitButtonPressed()
        {
            DisplayButtons(false);
            DisplayTitle(false);

            UI.Navigation.UINavigationManager.NullifySelected();

            GlobalFadeOutInCoroutine(Manager.MainMenuManager.Quit);
        }

        private void OnMainMenuButtonSelected(MainMenuButton mainMenuButton)
        {
            _lastSelectedBtn?.Highlight(false);
            _lastSelectedBtn = mainMenuButton;
            _lastSelectedBtn.Highlight(true);

            if (UI.Navigation.UINavigationManager.CurrentlySelected != _lastSelectedBtn.gameObject)
                UI.Navigation.UINavigationManager.Select(_lastSelectedBtn.gameObject);
        }

        private void OnMainMenuButtonDeselected(MainMenuButton mainMenuButton)
        {
            _lastSelectedBtn?.Highlight(false);
            _lastSelectedBtn = null;
        }

        private void InitButtonsState()
        {
            bool gameSaveExist = Manager.SaveManager.GameSaveExist;
            _continueBtn.gameObject.SetActive(gameSaveExist);
        }

        private void InitButtonsEvents()
        {
            _newGameBtn.Button.onClick.AddListener(OnNewGameButtonPressed);
            _continueBtn.Button.onClick.AddListener(OnContinueButtonPressed);
            _settingsBtn.Button.onClick.AddListener(OnSettingsButtonPressed);
            _quitBtn.Button.onClick.AddListener(OnQuitButtonPressed);
        }

        private void InitNavigation()
        {
            System.Collections.Generic.List<MainMenuButton> enabledBtns = _allBtns.Where(o => o.gameObject.activeSelf).ToList();
            enabledBtns.Sort((MainMenuButton a, MainMenuButton b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));

            for (int i = enabledBtns.Count - 1; i >= 0; --i)
            {
                enabledBtns[i].Button.SetMode(UnityEngine.UI.Navigation.Mode.Explicit);
                enabledBtns[i].Button.SetSelectOnUp(enabledBtns[RSLib.Helpers.Mod(i - 1, enabledBtns.Count)].Button);
                enabledBtns[i].Button.SetSelectOnDown(enabledBtns[RSLib.Helpers.Mod(i + 1, enabledBtns.Count)].Button);
            }

            MainMenuButton.Selected += OnMainMenuButtonSelected;
            MainMenuButton.Deselected += OnMainMenuButtonDeselected;
        }

        private void SetupInitViewState()
        {
            DisplayButtons(false);
            DisplayTitle(false);
            
            _pressAnyKey.SetActive(false);
            _vignette.transform.localScale = Vector3.one * _mainMenuFadeInDatas.VignetteTargetScale;
        }

        private void DisplayTitle(bool show)
        {
            _titleShadow.enabled = show;
            _title.material.SetColor("_Color", Color.white.WithA(show ? 1f : 0f));
        }

        private void DisplayButtons(bool show)
        {
            _btnsContainer.SetActive(show);
        }

        private void GlobalFadeOutInCoroutine(System.Action callback)
        {
            if (_vignetteFadeCoroutine != null)
                StopCoroutine(_vignetteFadeCoroutine);

            StartCoroutine(_vignetteFadeCoroutine = FadeVignetteCoroutine(_mainMenuFadeOutDatas.VignetteTargetScale,
                                                                          _mainMenuFadeInDatas.VignetteTargetScale,
                                                                          (0f, 0f),
                                                                          _mainMenuFadeInDatas.VignetteDur,
                                                                          _mainMenuFadeInDatas.VignetteCurve));

            Manager.RampFadeManager.Fade(_grayscaleRamp, _rampFadeOutDatas, (0f, 0f), (fadeIn) => callback?.Invoke());
        }

        private System.Collections.IEnumerator FadeVignetteCoroutine(float sourceValue, float targetValue, (float, float) delays, float dur, Curve curve, System.Action callback = null)
        {
            yield return RSLib.Yield.SharedYields.WaitForSeconds(delays.Item1);

            for (float t = 0f; t <= 1f; t += Time.deltaTime / dur)
            {
                _vignette.localScale = Vector3.one * Mathf.Lerp(sourceValue, targetValue, t.Ease(curve));
                yield return null;
            }

            _vignette.localScale = Vector3.one * targetValue;
        
            yield return RSLib.Yield.SharedYields.WaitForSeconds(delays.Item2);
            callback?.Invoke();
        }

        private System.Collections.IEnumerator FadeTitleColorCoroutine()
        {
            yield return RSLib.Yield.SharedYields.WaitForSeconds(_mainMenuFadeOutDatas.TitleColorDelay);
            Color color;

            for (float t = 0f; t <= 1f; t += Time.deltaTime / _mainMenuFadeOutDatas.TitleColorDur)
            {
                color = _title.materialForRendering.GetColor("_Color");
                _title.materialForRendering.SetColor("_Color", Color.Lerp(Color.black, Color.white, t.Ease(_mainMenuFadeOutDatas.TitleCurve)).WithA(color.a));
                yield return null;
            }

            color = _title.materialForRendering.GetColor("_Color");
            _title.materialForRendering.SetColor("_Color", Color.white.WithA(color.a));
            _titleShadow.enabled = true;
        }

        private System.Collections.IEnumerator FadeTitleAlphaCoroutine()
        {
            yield return RSLib.Yield.SharedYields.WaitForSeconds(_mainMenuFadeOutDatas.TitleAlphaDelay);
            Color color;

            for (float t = 0f; t <= 1f; t += Time.deltaTime / _mainMenuFadeOutDatas.TitleAlphaDur)
            {
                color = _title.materialForRendering.GetColor("_Color");
                _title.materialForRendering.SetColor("_Color", color.WithA(t.Ease(_mainMenuFadeOutDatas.TitleCurve)));
                yield return null;
            }

            color = _title.materialForRendering.GetColor("_Color");
            _title.materialForRendering.SetColor("_Color", color.WithA(1f));
            _titleShadow.enabled = true;
        }

        private System.Collections.IEnumerator PressAnyKeyCoroutine()
        {
            bool anyKeyDown = false;

            while (true)
            {
                yield return new RSLib.Yield.WaitForSecondsOrBreakIf(_pressAnyKeyBlinkSpeed, () => Input.anyKeyDown, () => anyKeyDown = true);

                if (anyKeyDown)
                {
                    _pressAnyKey.SetActive(false);
                    break;
                }

                _pressAnyKey.SetActive(!_pressAnyKey.activeSelf);
            }

            yield return null;
            yield return new RSLib.Yield.WaitForSecondsOrBreakIf(_pressAnyKeyPostDelay, () => Input.anyKeyDown);

            _btnsContainer.SetActive(true);
            UI.Navigation.UINavigationManager.OpenAndSelect(this);

            _lastSelectedBtn = FirstButtonSelected;
            _lastSelectedBtn.Highlight(true);
        }

        protected override void Awake()
        {
            base.Awake();

            Manager.OptionsManager.Instance.OptionsOpened += OnOptionsOpened;
            Manager.OptionsManager.Instance.OptionsClosed += OnOptionsClosed;

            _allBtns = new MainMenuButton[]
            {
                _continueBtn,
                _newGameBtn,
                _settingsBtn,
                _quitBtn
            };

            InitButtonsState();
            InitButtonsEvents();
            InitNavigation();
        }

        private void Start()
        {
            SetupInitViewState();

            StartCoroutine(FadeTitleColorCoroutine());
            StartCoroutine(FadeTitleAlphaCoroutine());

            StartCoroutine(_vignetteFadeCoroutine = FadeVignetteCoroutine(_mainMenuFadeInDatas.VignetteTargetScale,
                                                                          _mainMenuFadeOutDatas.VignetteTargetScale,
                                                                          (_mainMenuFadeOutDatas.VignetteDelay, 0.8f),
                                                                          _mainMenuFadeOutDatas.VignetteDur,
                                                                          _mainMenuFadeOutDatas.VignetteCurve,
                                                                          () => StartCoroutine(PressAnyKeyCoroutine())));
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            MainMenuButton.Selected -= OnMainMenuButtonSelected;
            MainMenuButton.Deselected -= OnMainMenuButtonDeselected;

            if (Manager.OptionsManager.Exists())
            {
                Manager.OptionsManager.Instance.OptionsOpened -= OnOptionsOpened;
                Manager.OptionsManager.Instance.OptionsClosed -= OnOptionsClosed;
            }
        }
    }
}