using UnityEngine;

public class TemplarAttackController
{
    private TemplarAttackDatas[] _attacksDatas;
    private TemplarAttackDatas _airborneAttackDatas;
    private TemplarController _templarController;

    private System.Collections.Generic.Dictionary<string, AttackHitbox> _attackHitboxesById;

    private System.Collections.IEnumerator _attackCoroutine;

    public TemplarAttackController(TemplarController templarController)
    {
        _templarController = templarController;
        _attacksDatas = _templarController.ControllerDatas.BaseCombo;
        _airborneAttackDatas = _templarController.ControllerDatas.AirborneAttack;

        _attackHitboxesById = new System.Collections.Generic.Dictionary<string, AttackHitbox>();
        for (int i = _templarController.AttackHitboxesContainer.AttackHitboxes.Length - 1; i >= 0; --i)
        {
            UnityEngine.Assertions.Assert.IsFalse(
                _attackHitboxesById.ContainsKey(_templarController.AttackHitboxesContainer.AttackHitboxes[i].Id),
                $"Duplicate Id {_templarController.AttackHitboxesContainer.AttackHitboxes[i].Id} found for attack hitboxes.");

            _attackHitboxesById.Add(_templarController.AttackHitboxesContainer.AttackHitboxes[i].Id, _templarController.AttackHitboxesContainer.AttackHitboxes[i]);
            _templarController.AttackHitboxesContainer.AttackHitboxes[i].Hit += OnHit;
        }
    }

    public TemplarInputController InputController => _templarController.InputCtrl;

    public float AttackDir { get; private set; }
    public TemplarAttackDatas CurrentAttackDatas { get; private set; }
    public bool CanAttackAirborne { get; private set; }
    public bool CanChainAttack { get; private set; }
    public bool IsAttacking => _attackCoroutine != null;

    public void ResetAirborneAttack()
    {
        CanAttackAirborne = true;
    }

    public void Attack(System.Action<float> comboOverCallback = null)
    {
        _attackCoroutine = ComboCoroutine(comboOverCallback);
        _templarController.StartCoroutine(_attackCoroutine);
    }

    public void AttackAirborne(System.Action attackOverCallback = null)
    {
        _attackCoroutine = AirborneAttackCoroutine(attackOverCallback);
        _templarController.StartCoroutine(_attackCoroutine);
        CanAttackAirborne = false;
    }

    private void OnHit(AttackHitbox.HitEventArgs hitArgs)
    {
        _templarController.TemplarView.PlayHitVFX(hitArgs.Dir);
        _templarController.CameraController.Shake.SetTrauma(0.25f);
    }

    private void ComputeAttackDirection()
    {
        AttackDir = Mathf.Sign(InputController.Horizontal != 0f ? InputController.Horizontal : _templarController.CurrDir);
    }

    private void ComputeVelocity(float t, ref Vector3 vel)
    {
        vel.x = CurrentAttackDatas.MoveSpeedCurve.Evaluate(t) * CurrentAttackDatas.MoveSpeed * AttackDir;
        vel.y -= CurrentAttackDatas.Gravity * Time.deltaTime;
    }

    private void TriggerHit()
    {
        _templarController.AttackHitboxesContainer.SetDirection(AttackDir);

        if (_attackHitboxesById.TryGetValue(CurrentAttackDatas.Id, out AttackHitbox hitbox))
            hitbox.Trigger(AttackDir, CurrentAttackDatas);
        else
            CProLogger.LogError(this, $"Could not find hitbox with Id {CurrentAttackDatas.Id}.");
    }

    private System.Collections.IEnumerator ComboCoroutine(System.Action<float> comboOverCallback = null)
    {
        ComputeAttackDirection();
        Vector3 attackVel = new Vector3(0f, 0f);

        CurrentAttackDatas = _attacksDatas[0]; // Done for attack view.
        _templarController.TemplarView.PlayAttackAnimation(AttackDir);
        if (_templarController.CollisionsCtrl.Below)
            _templarController.TemplarView.PlayAttackVFX(AttackDir, 0.25f);

        for (int i = 0; i < _attacksDatas.Length; ++i)
        {
            CurrentAttackDatas = _attacksDatas[i];
            TriggerHit();

            // Attack motion.
            for (float t = 0f; t < 1f; t += Time.deltaTime / CurrentAttackDatas.Dur)
            {
                CanChainAttack = t >= CurrentAttackDatas.ChainAllowedTime;
                if (CanChainAttack)
                {
                    if (InputController.CheckInput(TemplarInputController.ButtonCategory.ROLL) || InputController.CheckInput(TemplarInputController.ButtonCategory.JUMP))
                        break;

                    if (InputController.CheckInput(TemplarInputController.ButtonCategory.ATTACK) && i < _attacksDatas.Length - 1)
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

            if (InputController.CheckInput(TemplarInputController.ButtonCategory.ATTACK) && i < _attacksDatas.Length - 1)
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
        comboOverCallback?.Invoke(AttackDir);
        _templarController.TemplarView.PlayIdleAnimation();
    }

    private System.Collections.IEnumerator AirborneAttackCoroutine(System.Action attackOverCallback = null)
    {
        ComputeAttackDirection();
        Vector3 attackVel = new Vector3(0f, 0f);

        CurrentAttackDatas = _airborneAttackDatas;
        TriggerHit();

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
        attackOverCallback?.Invoke();

        // This will lead to fall animation instantly, but this is done to exit the attack animation.
        _templarController.TemplarView.PlayIdleAnimation();
    }
}