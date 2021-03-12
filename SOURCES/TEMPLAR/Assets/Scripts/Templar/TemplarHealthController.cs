using UnityEngine;

public class TemplarHealthController : UnitHealthController
{
    [SerializeField] private UnityEngine.UI.Image _healthBar = null;
    [SerializeField] private UnityEngine.UI.Image _healthBarBlink = null;
    [SerializeField] private float _healthBarBlinkDur = 0.15f;

    public TemplarController TemplarCtrl { get; set; }

    public override void OnHit(AttackDatas attackDatas, float dir)
    {
        UnityEngine.Assertions.Assert.IsNotNull(TemplarCtrl, "TemplarController must be referenced to handle TemplarHealthController.");
        if (TemplarCtrl.RollCtrl.IsRolling)
            return;

        base.OnHit(attackDatas, dir);
    }

    protected override void OnHealthChanged(RSLib.HealthSystem.HealthChangedEventArgs args)
    {
        base.OnHealthChanged(args);
        _healthBar.fillAmount = HealthSystem.HealthPercentage;
        StartCoroutine(BlinkHealthBarCoroutine());
    }

    protected override void OnKilled()
    {
        base.OnKilled();

        // [TMP] We might maybe want to do something special on the HUD on death.
        _healthBar.fillAmount = 0;
        StartCoroutine(BlinkHealthBarCoroutine());
    }

    private System.Collections.IEnumerator BlinkHealthBarCoroutine()
    {
        yield return RSLib.Yield.SharedYields.WaitForSeconds(_healthBarBlinkDur);

        float targetValue = HealthSystem.HealthPercentage;
        float sign = Mathf.Sign(targetValue - _healthBarBlink.fillAmount);

        while (sign > 0f ? _healthBarBlink.fillAmount < targetValue : _healthBarBlink.fillAmount > targetValue)
        {
            _healthBarBlink.fillAmount += 0.05f * sign;
            yield return null;
        }

        _healthBarBlink.fillAmount = targetValue;
    }
}