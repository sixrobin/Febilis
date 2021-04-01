namespace Templar.Unit.Player
{
    using RSLib.Extensions;
    using UnityEngine;

    public class PlayerHealthController : UnitHealthController
    {
        [SerializeField] private Canvas _healthBarCanvas = null;
        [SerializeField] private UnityEngine.UI.Image _healthBar = null;
        [SerializeField] private UnityEngine.UI.Image _healthBarBlink = null;
        [SerializeField] private float _healthBarBlinkPauseDur = 0.15f;
        [SerializeField] private float _healthBarBlinkUpdateSpeed = 4f;

        [Header("HEALTH BAR SHINE")]
        [SerializeField] private RectTransform _shineRectTransform = null;
        [SerializeField] private float _shineDelay = 3f;
        [SerializeField] private float _shineDist = 250f;
        [SerializeField] private float _shineDur = 0.2f;
        [SerializeField] private RSLib.Maths.Curve _shineCurve = RSLib.Maths.Curve.Linear;

        private System.Collections.IEnumerator _healthBarUpdateCoroutine;
        private System.Collections.IEnumerator _shineCoroutine;

        public PlayerController PlayerCtrl { get; set; }

        public override Attack.HitLayer HitLayer => Attack.HitLayer.PLAYER;

        public override void Init(int health)
        {
            base.Init(health);

            Manager.RampFadeManager.Instance.FadeBegan += OnFadeBegan;
            Manager.RampFadeManager.Instance.FadeOver += OnFadeOver;

            _shineCoroutine = ShineCoroutine();
            StartCoroutine(_shineCoroutine);
        }

        public override void OnHit(Attack.HitInfos hitDatas)
        {
            UnityEngine.Assertions.Assert.IsNotNull(PlayerCtrl, "PlayerController must be referenced to handle PlayerHealthController.");
            if (PlayerCtrl.RollCtrl.IsRolling)
                return;

            base.OnHit(hitDatas);
        }

        protected override void OnHealthChanged(RSLib.HealthSystem.HealthChangedEventArgs args)
        {
            base.OnHealthChanged(args);

            if (_healthBarUpdateCoroutine != null)
                SkipHealthBarUpdateCoroutine();

            if (args.IsLoss)
                _healthBar.fillAmount = HealthSystem.HealthPercentage;
            else
                _healthBarBlink.fillAmount = HealthSystem.HealthPercentage;

            _healthBarUpdateCoroutine = BlinkHealthBarCoroutine(args.IsLoss);
            StartCoroutine(_healthBarUpdateCoroutine);
        }

        protected override void OnKilled()
        {
            base.OnKilled();

            // [TMP] We may want to do something special on the HUD on death.
            _healthBar.fillAmount = 0;
            StartCoroutine(BlinkHealthBarCoroutine(true));
            StopCoroutine(_shineCoroutine);
        }

        private void OnFadeBegan()
        {
            _healthBarCanvas.enabled = false;
        }

        private void OnFadeOver()
        {
            // [TMP] Should have a better HUD management. This case is too specific.
            if (!HealthSystem.IsDead)
                _healthBarCanvas.enabled = true;
        }

        private void SkipHealthBarUpdateCoroutine()
        {
            UnityEngine.Assertions.Assert.IsNotNull(_healthBarUpdateCoroutine, "Trying to stop a coroutine that is not running.");
            StopCoroutine(_healthBarUpdateCoroutine);
            _healthBarBlink.fillAmount = _healthBar.fillAmount;
        }

        private System.Collections.IEnumerator BlinkHealthBarCoroutine(bool isLoss)
        {
            yield return RSLib.Yield.SharedYields.WaitForSeconds(_healthBarBlinkPauseDur);

            UnityEngine.UI.Image barToFill = isLoss ? _healthBarBlink : _healthBar;

            float targetValue = HealthSystem.HealthPercentage;
            float sign = Mathf.Sign(targetValue - (isLoss ? _healthBarBlink.fillAmount : _healthBar.fillAmount));

            while (sign > 0f ? barToFill.fillAmount < targetValue : barToFill.fillAmount > targetValue)
            {
                barToFill.fillAmount += _healthBarBlinkUpdateSpeed * Time.deltaTime * sign;
                yield return null;
            }

            barToFill.fillAmount = targetValue;
            _healthBarUpdateCoroutine = null;
        }

        private System.Collections.IEnumerator ShineCoroutine()
        {
            while (HealthSystem.HealthPercentage > 0f)
            {
                yield return RSLib.Yield.SharedYields.WaitForSeconds(_shineDelay);

                _shineRectTransform.anchoredPosition = _shineRectTransform.anchoredPosition.WithX(0f);

                for (float t = 0f; t < 1f; t += Time.deltaTime / _shineDur)
                {
                    _shineRectTransform.anchoredPosition = _shineRectTransform.anchoredPosition.WithX(_shineDist * RSLib.Maths.Easing.Ease(t, _shineCurve));
                    yield return null;
                }

                _shineRectTransform.anchoredPosition = _shineRectTransform.anchoredPosition.WithX(_shineDist);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (Manager.RampFadeManager.Exists())
            {
                Manager.RampFadeManager.Instance.FadeBegan -= OnFadeBegan;
                Manager.RampFadeManager.Instance.FadeOver -= OnFadeOver;
            }
        }
    }
}