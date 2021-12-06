namespace Templar.MainMenu
{
    using RSLib.Extensions;
    using RSLib.Maths;
    using System.Linq;
    using UnityEngine;

    public class MainMenuPanel : UI.UIPanel
    {
        [Header("VIGNETTE")]
        [SerializeField] private Transform _vignette = null;
        [SerializeField] private float _fadeOutDelay = 1f;
        [SerializeField] private float _vignetteInScale = 5.2f;
        [SerializeField] private float _vignetteOutScale = 100f;
        [SerializeField] private float _vignetteInDur = 1f;
        [SerializeField] private float _vignetteOutDur = 2f;
        [SerializeField] private Curve _vignetteInCurve = Curve.InOutSine;
        [SerializeField] private Curve _vignetteOutCurve = Curve.InOutSine;

        [Header("FADE OUT")]
        [SerializeField] private RSLib.ImageEffects.CameraGrayscaleRamp _grayscaleRamp = null;
        [SerializeField] private Datas.RampFadeDatas _rampFadeOutDatas = null;

        [Header("TITLE")]
        [SerializeField] private UnityEngine.UI.Image _title = null;
        [SerializeField] private UnityEngine.UI.Image _titleShadow = null;
        [SerializeField] private float _titleColorInDur = 1.6f;
        [SerializeField] private float _titleColorInDelay = 1.1f;
        [SerializeField] private float _titleAlphaInDur = 1.6f;
        [SerializeField] private float _titleAlphaInDelay = 1.1f;
        [SerializeField] private Curve _titleInCurve = Curve.InQuint;

        [Header("PRESS ANY KEY")]
        [SerializeField] private GameObject _pressAnyKey = null;
        [SerializeField] private float _pressAnyKeyBlinkSpeed = 0.5f;
        [SerializeField] private float _pressAnyKeyPostDelay = 0.5f;

        [Header("BUTTONS")]
        [SerializeField] private GameObject _btnsContainer = null;
        [SerializeField] private MainMenuButton _continueBtn = null;
        [SerializeField] private MainMenuButton _newGameBtn = null;
        [SerializeField] private MainMenuButton _settingsBtn = null;
        [SerializeField] private MainMenuButton _quitBtn = null;

        private MainMenuButton[] _allBtns;
        private MainMenuButton _lastSelectedBtn;

        private System.Collections.IEnumerator _vignetteFadeCoroutine;
        private System.Collections.IEnumerator _pressAnyKeyCoroutine;

        public override GameObject FirstSelected => FirstButtonSelected.gameObject;

        public MainMenuButton FirstButtonSelected => Manager.SaveManager.GameSaveExist ? _continueBtn : _newGameBtn;

        public override void OnBackButtonPressed()
        {
        }

        private void OnQuitButtonPressed()
        {
            UI.Navigation.UINavigationManager.NullifySelected();
            
            _btnsContainer.SetActive(false);
            _title.enabled = false;
            _titleShadow.enabled = false;

            if (_vignetteFadeCoroutine != null)
                StopCoroutine(_vignetteFadeCoroutine);

            StartCoroutine(_vignetteFadeCoroutine = FadeVignetteCoroutine(_vignetteOutScale,
                                                                          _vignetteInScale,
                                                                          (0f, 0f),
                                                                          _vignetteInDur,
                                                                          _vignetteInCurve));

            Manager.RampFadeManager.Fade(_grayscaleRamp, _rampFadeOutDatas, (0f, 0f), (fadeIn) => MainMenuManager.Quit());
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
            _newGameBtn.Button.onClick.AddListener(() => throw new System.NotImplementedException("New game"));
            _continueBtn.Button.onClick.AddListener(() => throw new System.NotImplementedException("Continue"));
            _settingsBtn.Button.onClick.AddListener(() => throw new System.NotImplementedException("Settings"));
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
            _btnsContainer.SetActive(false);
            _pressAnyKey.SetActive(false);
            _titleShadow.enabled = false;

            _vignette.transform.localScale = Vector3.one * _vignetteInScale;
            _title.material.SetColor("_Color", new Color(0f, 0f, 0f, 0f));
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
            yield return RSLib.Yield.SharedYields.WaitForSeconds(_titleColorInDelay);
            Color color;

            for (float t = 0f; t <= 1f; t += Time.deltaTime / _titleColorInDur)
            {
                color = _title.materialForRendering.GetColor("_Color");
                _title.materialForRendering.SetColor("_Color", Color.Lerp(Color.black, Color.white, t.Ease(_titleInCurve)).WithA(color.a));
                yield return null;
            }

            color = _title.materialForRendering.GetColor("_Color");
            _title.materialForRendering.SetColor("_Color", Color.white.WithA(color.a));
            _titleShadow.enabled = true;
        }

        private System.Collections.IEnumerator FadeTitleAlphaCoroutine()
        {
            yield return RSLib.Yield.SharedYields.WaitForSeconds(_titleAlphaInDelay);
            Color color;

            for (float t = 0f; t <= 1f; t += Time.deltaTime / _titleAlphaInDur)
            {
                color = _title.materialForRendering.GetColor("_Color");
                _title.materialForRendering.SetColor("_Color", color.WithA(t.Ease(_titleInCurve)));
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
                yield return new RSLib.Yield.WaitForSecondsOrBreakIf(_pressAnyKeyBlinkSpeed,
                    () => Input.anyKeyDown,
                    () => anyKeyDown = true);

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

            StartCoroutine(_vignetteFadeCoroutine = FadeVignetteCoroutine(_vignetteInScale,
                                                                          _vignetteOutScale,
                                                                          (_fadeOutDelay, 0.8f),
                                                                          _vignetteOutDur,
                                                                          _vignetteOutCurve,
                                                                          () => StartCoroutine(_pressAnyKeyCoroutine = PressAnyKeyCoroutine())));
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            MainMenuButton.Selected -= OnMainMenuButtonSelected;
            MainMenuButton.Deselected -= OnMainMenuButtonDeselected;
        }
    }
}