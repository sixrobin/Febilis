namespace Templar.Unit.Player
{
    using UnityEngine;

    public class PlayerHealthController : UnitHealthController
    {
        [SerializeField] private UnityEngine.UI.Image _healthBar = null;
        [SerializeField] private UnityEngine.UI.Image _healthBarBlink = null;
        [SerializeField] private float _healthBarBlinkPauseDur = 0.15f;
        [SerializeField] private float _healthBarBlinkUpdateSpeed = 4f;

        private System.Collections.IEnumerator _healthBarUpdateCoroutine;

        public PlayerController PlayerCtrl { get; set; }

        public override Attack.HitLayer HitLayer => Attack.HitLayer.PLAYER;

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

            _healthBar.fillAmount = HealthSystem.HealthPercentage;

            _healthBarUpdateCoroutine = BlinkHealthBarCoroutine();
            StartCoroutine(_healthBarUpdateCoroutine);
        }

        protected override void OnKilled()
        {
            base.OnKilled();

            // [TMP] We may want to do something special on the HUD on death.
            _healthBar.fillAmount = 0;
            StartCoroutine(BlinkHealthBarCoroutine());
        }

        private void SkipHealthBarUpdateCoroutine()
        {
            UnityEngine.Assertions.Assert.IsNotNull(_healthBarUpdateCoroutine, "Trying to stop a coroutine that is not running.");
            StopCoroutine(_healthBarUpdateCoroutine);
            _healthBarBlink.fillAmount = _healthBar.fillAmount;
        }

        private System.Collections.IEnumerator BlinkHealthBarCoroutine()
        {
            yield return RSLib.Yield.SharedYields.WaitForSeconds(_healthBarBlinkPauseDur);

            float targetValue = HealthSystem.HealthPercentage;
            float sign = Mathf.Sign(targetValue - _healthBarBlink.fillAmount);

            while (sign > 0f ? _healthBarBlink.fillAmount < targetValue : _healthBarBlink.fillAmount > targetValue)
            {
                _healthBarBlink.fillAmount += _healthBarBlinkUpdateSpeed * Time.deltaTime * sign;
                yield return null;
            }

            _healthBarBlink.fillAmount = targetValue;
            _healthBarUpdateCoroutine = null;
        }
    }
}