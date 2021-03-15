using UnityEngine;

public class TemplarAttackController : AttackController
{
    private TemplarAttackDatas[] _baseComboDatas;
    private TemplarAttackDatas _airborneAttackDatas;
    private TemplarController _templarController;

    public TemplarAttackController(TemplarController templarController)
        : base(templarController, templarController.AttackHitboxesContainer)
    {
        _templarController = templarController;
        _baseComboDatas = _templarController.ControllerDatas.BaseCombo;
        _airborneAttackDatas = _templarController.ControllerDatas.AirborneAttack;
    }

    public TemplarInputController InputController => _templarController.InputCtrl;

    public TemplarAttackDatas CurrentAttackDatas { get; private set; }
    public bool CanAttackAirborne { get; private set; }
    public bool CanChainAttack { get; private set; }

    public void ResetAirborneAttack()
    {
        CanAttackAirborne = true;
    }

    public void Attack(AttackOverEventHandler comboOverCallback = null)
    {
        _attackCoroutine = ComboCoroutine(comboOverCallback);
        _attackCoroutineRunner.StartCoroutine(_attackCoroutine);
    }

    public void AttackAirborne(AttackOverEventHandler attackOverCallback = null)
    {
        _attackCoroutine = AirborneAttackCoroutine(attackOverCallback);
        _attackCoroutineRunner.StartCoroutine(_attackCoroutine);
        CanAttackAirborne = false;
    }

    protected override void OnAttackHit(AttackHitbox.HitEventArgs hitArgs)
    {
        _templarController.TemplarView.PlayHitVFX(hitArgs.Dir);
        _templarController.CameraController.Shake.SetTrauma(0.25f);
        FreezeFrameController.FreezeFrame(0, 0.05f);
    }

    protected override void ComputeAttackDirection()
    {
        AttackDir = Mathf.Sign(InputController.Horizontal != 0f ? InputController.Horizontal : _templarController.CurrDir);
    }

    private void ComputeVelocity(float t, ref Vector3 vel)
    {
        vel.x = CurrentAttackDatas.MoveSpeedCurve.Evaluate(t) * CurrentAttackDatas.MoveSpeed * AttackDir;
        vel.y -= CurrentAttackDatas.Gravity * Time.deltaTime;
    }

    private System.Collections.IEnumerator ComboCoroutine(AttackOverEventHandler comboOverCallback = null)
    {
        ComputeAttackDirection();
        Vector3 attackVel = new Vector3(0f, 0f);

        CurrentAttackDatas = _baseComboDatas[0]; // Done for attack view.
        _templarController.TemplarView.PlayAttackAnimation(AttackDir);
        if (_templarController.CollisionsCtrl.Below)
            _templarController.TemplarView.PlayAttackVFX(AttackDir, 0.25f);

        for (int i = 0; i < _baseComboDatas.Length; ++i)
        {
            CurrentAttackDatas = _baseComboDatas[i];
            TriggerHit(CurrentAttackDatas);

            // Attack motion.
            for (float t = 0f; t < 1f; t += Time.deltaTime / CurrentAttackDatas.Dur)
            {
                CanChainAttack = t >= CurrentAttackDatas.ChainAllowedTime;
                if (CanChainAttack)
                {
                    if (InputController.CheckInput(TemplarInputController.ButtonCategory.ROLL) || InputController.CheckInput(TemplarInputController.ButtonCategory.JUMP))
                        break;

                    if (InputController.CheckInput(TemplarInputController.ButtonCategory.ATTACK) && i < _baseComboDatas.Length - 1)
                        break;
                }

                if (CurrentAttackDatas.ControlVelocity)
                {
                    ComputeVelocity(t, ref attackVel);
                    _templarController.Translate(attackVel);
                }

                yield return null;
            }

            if (InputController.CheckInput(TemplarInputController.ButtonCategory.ROLL) || InputController.CheckInput(TemplarInputController.ButtonCategory.JUMP))
            {
                CProLogger.Log(this, "Roll or jump input is true, breaking out of combo.", _templarController.gameObject);
                break;
            }

            if (InputController.CheckInput(TemplarInputController.ButtonCategory.ATTACK) && i < _baseComboDatas.Length - 1)
            {
                // Chained attack.
                InputController.ResetDelayedInput(TemplarInputController.ButtonCategory.ATTACK);
                ComputeAttackDirection();

                _templarController.TemplarView.PlayChainAttackAnimation(AttackDir);
                if (_templarController.CollisionsCtrl.Below)
                    _templarController.TemplarView.PlayAttackVFX(AttackDir, 0.25f);
            }
            else
            {
                break;
            }
        }

        CProLogger.Log(this, $"Combo end with a direction of {AttackDir}.", _templarController.gameObject);

        _attackCoroutine = null;
        comboOverCallback?.Invoke(new AttackOverEventArgs(AttackDir));
        _templarController.TemplarView.PlayIdleAnimation();
    }

    private System.Collections.IEnumerator AirborneAttackCoroutine(AttackOverEventHandler attackOverCallback = null)
    {
        ComputeAttackDirection();
        Vector3 attackVel = new Vector3(0f, 0f);

        CurrentAttackDatas = _airborneAttackDatas;
        TriggerHit(CurrentAttackDatas);

        _templarController.TemplarView.PlayAttackAirborneAnimation();

        for (float t = 0f; t < 1f; t += Time.deltaTime / CurrentAttackDatas.Dur)
        {
            CanChainAttack = t >= CurrentAttackDatas.ChainAllowedTime;

            if (CurrentAttackDatas.ControlVelocity)
            {
                ComputeVelocity(t, ref attackVel);
                _templarController.Translate(attackVel);
            }

            yield return null;
        }

        _attackCoroutine = null;
        attackOverCallback?.Invoke(new AttackOverEventArgs(AttackDir));

        // This will lead to fall animation instantly, but this is done to exit the attack animation.
        _templarController.TemplarView.PlayIdleAnimation();
    }
}