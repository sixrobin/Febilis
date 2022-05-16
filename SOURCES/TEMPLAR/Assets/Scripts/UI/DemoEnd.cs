﻿namespace Templar.UI
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
        
        [Header("TEXT")]
        [SerializeField] private float _textAppearenceDelay = 1f;
        [SerializeField] private TMPro.TextMeshProUGUI _demoEndText = null;
        [SerializeField] private RSLib.Audio.ClipProvider _demoEndTextAppearClipProvider = null;

        [Header("PRESS ANY KEY")]
        [SerializeField] private GameObject _pressAnyKey = null;
        [SerializeField] private TMPro.TextMeshProUGUI _pressAnyKeyText = null;
        [SerializeField] private float _pressAnyKeyAppearenceDelay = 1f;
        [SerializeField] private float _pressAnyKeyBlinkSpeed = 0.5f;

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
            
            yield return RSLib.Yield.SharedYields.WaitForSeconds(_logoMoveDelay);

            float logoInitY = _logoRectTransform.anchoredPosition.y;
            for (float t = 0f; t < 1f; t += Time.deltaTime / _logoMoveDuration)
            {
                _logoRectTransform.anchoredPosition = _logoRectTransform.anchoredPosition.WithY(Mathf.Lerp(logoInitY, _logoTargetY, t.Ease(_logoMoveCurve)));
                yield return null;
            }
            
            _logoRectTransform.anchoredPosition = _logoRectTransform.anchoredPosition.WithY(_logoTargetY);

            yield return RSLib.Yield.SharedYields.WaitForSeconds(_textAppearenceDelay);
            _demoEndText.text = Localizer.Get(Localization.Menu.DEMO_END_THANKS);
            _demoEndText.enabled = true;
            RSLib.Audio.AudioManager.PlaySound(_demoEndTextAppearClipProvider);
            
            yield return RSLib.Yield.SharedYields.WaitForSeconds(_pressAnyKeyAppearenceDelay);
            _pressAnyKeyText.text = Localizer.Get(Localization.Menu.PRESS_ANY_KEY);
            _pressAnyKeyText.enabled = true;

            yield return PressAnyKeyCoroutine();

            UnityEngine.SceneManagement.SceneManager.LoadScene(_sceneToLoad);
        }
        
        private System.Collections.IEnumerator PressAnyKeyCoroutine()
        {
            bool anyKeyDown = false;
            _pressAnyKeyText.text = Localizer.Get(Localization.Menu.PRESS_ANY_KEY);
            
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
            StartCoroutine(DemoEndPanelCoroutine());
        }
    }
}