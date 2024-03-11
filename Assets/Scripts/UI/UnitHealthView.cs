namespace Templar.UI
{
    using UnityEngine;

    public class UnitHealthView : MonoBehaviour
    {
        [Header("HEALTH BAR UPDATE")]
        [SerializeField] private UnityEngine.UI.Image _healthFill = null;
        [SerializeField] private UnityEngine.UI.Image _healthBlink = null;
        [SerializeField] private UnityEngine.UI.Image _healthBackground = null;
        [SerializeField] private float _healthBarBlinkPauseDur = 0.15f;
        [SerializeField] private float _healthBarBlinkUpdateSpeed = 4f;

        private System.Collections.IEnumerator _healthBarUpdateCoroutine;

        public void DisplayBackground(bool show)
        {
            _healthBackground.enabled = show;
        }

        public void SetHealthInstantly(int current, int max)
        {
            if (_healthBarUpdateCoroutine != null)
                SkipHealthBarUpdateCoroutine();

            float currHealthPercentage = (float)current / max;
            _healthFill.fillAmount = currHealthPercentage;
            _healthBlink.fillAmount = 0f;
        }

        public void OnKilled()
        {
            if (_healthBarUpdateCoroutine != null)
                SkipHealthBarUpdateCoroutine();

            _healthFill.fillAmount = 0;
            StartCoroutine(_healthBarUpdateCoroutine = BlinkHealthBarCoroutine(true, 0f));
        }

        public void OnHealthChanged(RSLib.HealthSystem.HealthChangedEventArgs args)
        {
            if (_healthBarUpdateCoroutine != null)
                SkipHealthBarUpdateCoroutine();

            float currHealthPercentage = (float)args.Current / args.Max;

            if (args.IsLoss)
                _healthFill.fillAmount = currHealthPercentage;
            else
                _healthBlink.fillAmount = currHealthPercentage;

            StartCoroutine(_healthBarUpdateCoroutine = BlinkHealthBarCoroutine(args.IsLoss, currHealthPercentage));
        }

        private void SkipHealthBarUpdateCoroutine()
        {
            UnityEngine.Assertions.Assert.IsNotNull(_healthBarUpdateCoroutine, "Trying to stop a coroutine that is not running.");
            StopCoroutine(_healthBarUpdateCoroutine);
            _healthBlink.fillAmount = _healthFill.fillAmount;
        }

        private System.Collections.IEnumerator BlinkHealthBarCoroutine(bool isLoss, float targetValue)
        {
            yield return RSLib.Yield.SharedYields.WaitForSeconds(_healthBarBlinkPauseDur);

            UnityEngine.UI.Image barToFill = isLoss ? _healthBlink : _healthFill;

            float sign = Mathf.Sign(targetValue - (isLoss ? _healthBlink.fillAmount : _healthFill.fillAmount));

            while (sign > 0f ? barToFill.fillAmount < targetValue : barToFill.fillAmount > targetValue)
            {
                barToFill.fillAmount += _healthBarBlinkUpdateSpeed * Time.deltaTime * sign;
                yield return null;
            }

            barToFill.fillAmount = targetValue;
            _healthBarUpdateCoroutine = null;
        }
    }
}