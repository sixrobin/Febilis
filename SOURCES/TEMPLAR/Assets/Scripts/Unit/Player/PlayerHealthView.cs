namespace Templar.Unit.Player
{
    using RSLib.Extensions;
    using UnityEngine;

    public class PlayerHealthView : MonoBehaviour
    {
        [SerializeField] private PlayerHealthController _playerHealthCtrl = null;

        [Header("HEALTH BAR UPDATE")]
        [SerializeField] private Canvas _healthBarCanvas = null;
        [SerializeField] private UnityEngine.UI.Image _healthFill = null;
        [SerializeField] private UnityEngine.UI.Image _healthBlink = null;
        [SerializeField] private float _healthBarBlinkPauseDur = 0.15f;
        [SerializeField] private float _healthBarBlinkUpdateSpeed = 4f;

        [Header("HEALS")]
        [SerializeField] private RectTransform _healsContainer = null;
        [SerializeField] private GameObject _healViewPrefab = null;

        //[Header("HEALTH BAR SHINE")]
        //[SerializeField] private RectTransform _shineRectTransform = null;
        //[SerializeField] private float _shineDist = 250f;
        //[SerializeField] private float _shineDur = 0.2f;
        //[SerializeField] private RSLib.Maths.Curve _shineCurve = RSLib.Maths.Curve.Linear;

        private System.Collections.Generic.List<GameObject> _healViews = new System.Collections.Generic.List<GameObject>();

        private System.Collections.IEnumerator _healthBarUpdateCoroutine;
        private System.Collections.IEnumerator _shineCoroutine;

        private void InitHealViews()
        {
            _healsContainer.DestroyChildren();
            _healViews.Clear();

            for (int i = 0; i < _playerHealthCtrl.HealsLeft; ++i)
                _healViews.Add(Instantiate(_healViewPrefab, _healsContainer));

            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(_healsContainer);
        }

        private void AddHealViews(int count)
        {
            for (int i = 0; i < count; ++i)
                _healViews.Add(Instantiate(_healViewPrefab, _healsContainer));

            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(_healsContainer);
        }

        private void OnHealsLeftChanged(int healsLeft)
        {
            for (int i = 0; i < _healViews.Count; ++i)
            {
                if (i == _healViews.Count)
                    AddHealViews(1);

                _healViews[i].SetActive(i < healsLeft);
            }
        }

        private void OnFadeBegan()
        {
            _healthBarCanvas.enabled = false;
        }

        private void OnFadeOver()
        {
            // [TMP] Should have a better HUD management. This case is too specific.
            if (!_playerHealthCtrl.HealthSystem.IsDead)
                _healthBarCanvas.enabled = true;
        }

        private void OnHealthChanged(RSLib.HealthSystem.HealthChangedEventArgs args)
        {
            if (_healthBarUpdateCoroutine != null)
                SkipHealthBarUpdateCoroutine();

            if (args.IsLoss)
                _healthFill.fillAmount = _playerHealthCtrl.HealthSystem.HealthPercentage;
            else
                _healthBlink.fillAmount = _playerHealthCtrl.HealthSystem.HealthPercentage;

            _healthBarUpdateCoroutine = BlinkHealthBarCoroutine(args.IsLoss);
            StartCoroutine(_healthBarUpdateCoroutine);
        }

        private void OnKilled()
        {
            // [TMP] We may want to do something special on the HUD on death.
            _healthFill.fillAmount = 0;
            StartCoroutine(BlinkHealthBarCoroutine(true));
        }

        private void SkipHealthBarUpdateCoroutine()
        {
            UnityEngine.Assertions.Assert.IsNotNull(_healthBarUpdateCoroutine, "Trying to stop a coroutine that is not running.");
            StopCoroutine(_healthBarUpdateCoroutine);
            _healthBlink.fillAmount = _healthFill.fillAmount;
        }

        private System.Collections.IEnumerator BlinkHealthBarCoroutine(bool isLoss)
        {
            yield return RSLib.Yield.SharedYields.WaitForSeconds(_healthBarBlinkPauseDur);

            UnityEngine.UI.Image barToFill = isLoss ? _healthBlink : _healthFill;

            float targetValue = _playerHealthCtrl.HealthSystem.HealthPercentage;
            float sign = Mathf.Sign(targetValue - (isLoss ? _healthBlink.fillAmount : _healthFill.fillAmount));

            while (sign > 0f ? barToFill.fillAmount < targetValue : barToFill.fillAmount > targetValue)
            {
                barToFill.fillAmount += _healthBarBlinkUpdateSpeed * Time.deltaTime * sign;
                yield return null;
            }

            barToFill.fillAmount = targetValue;
            _healthBarUpdateCoroutine = null;
        }

        //private System.Collections.IEnumerator ShineCoroutine()
        //{
        //    _shineRectTransform.anchoredPosition = _shineRectTransform.anchoredPosition.WithX(0f);

        //    for (float t = 0f; t < 1f; t += Time.deltaTime / _shineDur)
        //    {
        //        _shineRectTransform.anchoredPosition = _shineRectTransform.anchoredPosition.WithX(_shineDist * RSLib.Maths.Easing.Ease(t, _shineCurve));
        //        yield return null;
        //    }

        //    _shineRectTransform.anchoredPosition = _shineRectTransform.anchoredPosition.WithX(_shineDist);
        //}

        private System.Collections.IEnumerator InitHealth()
        {
            yield return new WaitUntil(() => _playerHealthCtrl.HealthSystem != null);

            _playerHealthCtrl.HealsLeftChanged += OnHealsLeftChanged;
            _playerHealthCtrl.HealthSystem.HealthChanged += OnHealthChanged;
            _playerHealthCtrl.HealthSystem.Killed += OnKilled;

            InitHealViews();
        }

        private void Awake()
        {
            Manager.RampFadeManager.Instance.FadeBegan += OnFadeBegan;
            Manager.RampFadeManager.Instance.FadeOver += OnFadeOver;

            StartCoroutine(InitHealth());
        }

        private void OnDestroy()
        {
            _playerHealthCtrl.HealsLeftChanged -= OnHealsLeftChanged;
            _playerHealthCtrl.HealthSystem.HealthChanged -= OnHealthChanged;
            _playerHealthCtrl.HealthSystem.Killed -= OnKilled;

            if (Manager.RampFadeManager.Exists())
            {
                Manager.RampFadeManager.Instance.FadeBegan -= OnFadeBegan;
                Manager.RampFadeManager.Instance.FadeOver -= OnFadeOver;
            }
        }
    }
}