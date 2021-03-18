namespace Templar.Attack
{
    using UnityEngine;

    public class PlayerAttackController : AttackController
    {
        private Datas.PlayerAttackDatas[] _baseComboDatas;
        private Datas.PlayerAttackDatas _airborneAttackDatas;
        private Unit.Player.PlayerController _playerController;

        public PlayerAttackController(Unit.Player.PlayerController templarController)
            : base(templarController, templarController.AttackHitboxesContainer, templarController.transform)
        {
            _playerController = templarController;
            _baseComboDatas = _playerController.CtrlDatas.BaseCombo;
            _airborneAttackDatas = _playerController.CtrlDatas.AirborneAttack;
        }

        public Unit.Player.PlayerInputController InputController => _playerController.InputCtrl;

        public Datas.PlayerAttackDatas CurrentAttackDatas { get; private set; }
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
            _playerController.PlayerView.PlayHitVFX(hitArgs.Dir);
            _playerController.CameraCtrl.Shake.AddTrauma(0.25f);
            FreezeFrameManager.FreezeFrame(0, 0.05f);
        }

        protected override void ComputeAttackDirection()
        {
            AttackDir = Mathf.Sign(InputController.Horizontal != 0f ? InputController.Horizontal : _playerController.CurrDir);
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
            _playerController.PlayerView.PlayAttackAnimation(AttackDir);
            if (_playerController.CollisionsCtrl.Below)
                _playerController.PlayerView.PlayAttackVFX(AttackDir, 0.25f);

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
                        if (InputController.CheckInput(Unit.Player.PlayerInputController.ButtonCategory.ROLL) || InputController.CheckInput(Unit.Player.PlayerInputController.ButtonCategory.JUMP))
                            break;

                        if (InputController.CheckInput(Unit.Player.PlayerInputController.ButtonCategory.ATTACK) && i < _baseComboDatas.Length - 1)
                            break;
                    }

                    if (CurrentAttackDatas.ControlVelocity)
                    {
                        ComputeVelocity(t, ref attackVel);
                        _playerController.Translate(attackVel);
                    }

                    yield return null;
                }

                if (InputController.CheckInput(Unit.Player.PlayerInputController.ButtonCategory.ROLL) || InputController.CheckInput(Unit.Player.PlayerInputController.ButtonCategory.JUMP))
                {
                    CProLogger.Log(this, "Roll or jump input is true, breaking out of combo.", _playerController.gameObject);
                    break;
                }

                if (InputController.CheckInput(Unit.Player.PlayerInputController.ButtonCategory.ATTACK) && i < _baseComboDatas.Length - 1)
                {
                    // Chained attack.
                    InputController.ResetDelayedInput(Unit.Player.PlayerInputController.ButtonCategory.ATTACK);
                    ComputeAttackDirection();

                    _playerController.PlayerView.PlayChainAttackAnimation(AttackDir);
                    if (_playerController.CollisionsCtrl.Below)
                        _playerController.PlayerView.PlayAttackVFX(AttackDir, 0.25f);
                }
                else
                {
                    break;
                }
            }

            CProLogger.Log(this, $"Combo end with a direction of {AttackDir}.", _playerController.gameObject);

            _attackCoroutine = null;
            comboOverCallback?.Invoke(new AttackOverEventArgs(AttackDir));
            _playerController.PlayerView.PlayIdleAnimation();
        }

        private System.Collections.IEnumerator AirborneAttackCoroutine(AttackOverEventHandler attackOverCallback = null)
        {
            ComputeAttackDirection();
            Vector3 attackVel = new Vector3(0f, 0f);

            CurrentAttackDatas = _airborneAttackDatas;
            TriggerHit(CurrentAttackDatas);

            _playerController.PlayerView.PlayAttackAirborneAnimation();

            for (float t = 0f; t < 1f; t += Time.deltaTime / CurrentAttackDatas.Dur)
            {
                CanChainAttack = t >= CurrentAttackDatas.ChainAllowedTime;

                if (CurrentAttackDatas.ControlVelocity)
                {
                    ComputeVelocity(t, ref attackVel);
                    _playerController.Translate(attackVel);
                }

                yield return null;
            }

            _attackCoroutine = null;
            attackOverCallback?.Invoke(new AttackOverEventArgs(AttackDir));

            // This will lead to fall animation instantly, but this is done to exit the attack animation.
            _playerController.PlayerView.PlayIdleAnimation();
        }
    }
}