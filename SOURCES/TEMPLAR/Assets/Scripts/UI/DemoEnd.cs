namespace Templar.UI
{
    using RSLib.Extensions;
    using RSLib.Maths;
    using UnityEngine;

    public class DemoEnd : MonoBehaviour
    {
        [Header("LOGO")]
        [SerializeField] private float _logoAppearenceDelay = 1.5f;
        [SerializeField] private float _logoMoveDelay = 1f;
        [SerializeField] private RectTransform _logoRectTransform = null;
        [SerializeField] private float _logoTargetY = 28f;
        [SerializeField] private float _logoMoveDuration = 2f;
        [SerializeField] private Curve _logoMoveCurve = Curve.InOutQuart;
        [SerializeField] private RSLib.Audio.ClipProvider _logoAppearClipProvider = null;
        [SerializeField] private bool _playMainTheme = true;

        [Header("TEXT")]
        [SerializeField] private float _textAppearenceDelay = 1f;
        [SerializeField] private TMPro.TextMeshProUGUI _demoEndText = null;
        [SerializeField] private RSLib.Audio.ClipProvider _demoEndTextAppearClipProvider = null;

        [Header("PRESS ANY KEY")]
        [SerializeField] private GameObject _pressAnyKey = null;
        [SerializeField] private TMPro.TextMeshProUGUI _pressAnyKeyText = null;
        [SerializeField] private float _pressAnyKeyAppearenceDelay = 1f;
        [SerializeField] private float _pressAnyKeyBlinkSpeed = 0.5f;

        [Header("INIT BLACK MASK")]
        [SerializeField] private SpriteRenderer _blackMaskSpriteRenderer = null;
        [SerializeField] private float _blackMaskFadeOutDuration = 0.5f;
        [SerializeField] private float _blackMaskTargetScale = 4f;
        [SerializeField] private float _blackMaskAlphaFadeDuration = 1f;
        [SerializeField] private Curve _blackMaskFadeCurve = Curve.Linear;
        
        [Header("SCENE LOADING")]
        [SerializeField] private RSLib.Framework.SceneField _sceneToLoad = null;
        
        private void PrepareView()
        {
            _logoRectTransform.gameObject.SetActive(false);
            _demoEndText.enabled = false;
            _pressAnyKeyText.enabled = false;
        }
        
        private System.Collections.IEnumerator DemoEndPanelCoroutine()
        {
            yield return RSLib.Yield.SharedYields.WaitForSeconds(_logoAppearenceDelay);
            _logoRectTransform.gameObject.SetActive(true);
            
            RSLib.Audio.AudioManager.PlaySound(_logoAppearClipProvider);
            if (_playMainTheme)
                Manager.MusicManager.PlayMainTheme();
            
            yield return RSLib.Yield.SharedYields.WaitForSeconds(_logoMoveDelay);

            float logoInitY = _logoRectTransform.anchoredPosition.y;
            for (float t = 0f; t < 1f; t += Time.deltaTime / _logoMoveDuration)
            {
                _logoRectTransform.anchoredPosition = _logoRectTransform.anchoredPosition.WithY(Mathf.Lerp(logoInitY, _logoTargetY, t.Ease(_logoMoveCurve)));
                yield return null;
            }
            
            _logoRectTransform.anchoredPosition = _logoRectTransform.anchoredPosition.WithY(_logoTargetY);

            yield return RSLib.Yield.SharedYields.WaitForSeconds(_textAppearenceDelay);
            _demoEndText.text = RSLib.Localization.Localizer.Get(Localization.Menu.DEMO_END_THANKS);
            _demoEndText.enabled = true;
            RSLib.Audio.AudioManager.PlaySound(_demoEndTextAppearClipProvider);
            
            yield return RSLib.Yield.SharedYields.WaitForSeconds(_pressAnyKeyAppearenceDelay);
            _pressAnyKeyText.text = RSLib.Localization.Localizer.Get(Localization.Menu.PRESS_ANY_KEY);
            _pressAnyKeyText.enabled = true;

            yield return PressAnyKeyCoroutine();

            UnityEngine.SceneManagement.SceneManager.LoadScene(_sceneToLoad);
        }
        
        private System.Collections.IEnumerator FadeBlackMaskCoroutine()
        {
            float blackMaskInitScale = _blackMaskSpriteRenderer.transform.localScale.x;
            
            for (float t = 0f; t <= 1f; t += Time.deltaTime / _blackMaskFadeOutDuration)
            {
                _blackMaskSpriteRenderer.transform.localScale = Vector3.one * Mathf.Lerp(blackMaskInitScale, _blackMaskTargetScale, t.Ease(_blackMaskFadeCurve));
                _blackMaskSpriteRenderer.color = Color.Lerp(Color.black, Color.white, t.Ease(_blackMaskFadeCurve));
                yield return null;
            }
            
            _blackMaskSpriteRenderer.transform.localScale = Vector3.one * _blackMaskTargetScale;
            _blackMaskSpriteRenderer.color = Color.white;
            
            for (float t = 0f; t <= 1f; t += Time.deltaTime / _blackMaskAlphaFadeDuration)
            {
                _blackMaskSpriteRenderer.color = Color.white.WithA(1f - t);
                yield return null;
            }
            
            _blackMaskSpriteRenderer.color = Color.white.WithA(0f);
        }
        
        private System.Collections.IEnumerator PressAnyKeyCoroutine()
        {
            bool anyKeyDown = false;
            _pressAnyKeyText.text = RSLib.Localization.Localizer.Get(Localization.Menu.PRESS_ANY_KEY);
            
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
        }
        
        private void Start()
        {
            PrepareView();

            StartCoroutine(FadeBlackMaskCoroutine());
            StartCoroutine(DemoEndPanelCoroutine());
        }
    }
}