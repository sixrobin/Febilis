namespace Templar.Attack
{
    using UnityEngine;

    public class PlayerAttackController : AttackController
    {
        private Datas.Attack.PlayerAttackDatas[] _baseComboDatas;
        private Datas.Attack.PlayerAttackDatas _airborneAttackDatas;
        private Unit.Player.PlayerController _playerCtrl;

        public PlayerAttackController(Unit.Player.PlayerController playerCtrl)
            : base(playerCtrl, playerCtrl.AttackHitboxesContainer, playerCtrl.transform)
        {
            _playerCtrl = playerCtrl;

            _baseComboDatas = new Datas.Attack.PlayerAttackDatas[_playerCtrl.CtrlDatas.BaseComboIds.Length];
            for (int i = 0; i < _baseComboDatas.Length; ++i)
                _baseComboDatas[i] = Datas.Attack.AttackDatabase.PlayerAttacksDatas[_playerCtrl.CtrlDatas.BaseComboIds[i]];

            _airborneAttackDatas = Datas.Attack.AttackDatabase.PlayerAttacksDatas[_playerCtrl.CtrlDatas.AirborneAttackId];
        }

        public Unit.Player.PlayerInputController InputCtrl => _playerCtrl.InputCtrl;

        public Datas.Attack.PlayerAttackDatas CurrAttackDatas { get; private set; }
        public bool CanAttackAirborne { get; private set; }
        public bool CanChainAttack { get; private set; }

        public bool CanAttack()
        {
            return !_playerCtrl.RollCtrl.IsRolling
                && !IsAttacking
                && !_playerCtrl.JumpCtrl.IsInLandImpact
                && !_playerCtrl.IsBeingHurt
                && !_playerCtrl.IsHealing
                && InputCtrl.CheckInput(Unit.Player.PlayerInputController.ButtonCategory.ATTACK);
        }

        public void ResetAirborneAttack()
        {
            CanAttackAirborne = true;
        }

        public void Attack(AttackOverEventHandler comboOverCallback = null)
        {
            _attackCoroutineRunner.StartCoroutine(_attackCoroutine = ComboCoroutine(comboOverCallback));
        }

        public void AttackAirborne(AttackOverEventHandler attackOverCallback = null)
        {
            _attackCoroutineRunner.StartCoroutine(_attackCoroutine = AirborneAttackCoroutine(attackOverCallback));
            CanAttackAirborne = false;
        }

        protected override void OnAttackHit(AttackHitbox.HitEventArgs hitArgs)
        {
            _playerCtrl.PlayerView.PlayHitVFX(hitArgs.Dir);

            UnityEngine.Assertions.Assert.IsNotNull(CurrAttackDatas, "An attack hit has been triggered but player attack datas are null.");
            Manager.GameManager.PlayerCtrl.CameraCtrl.ApplyShakeFromDatas(CurrAttackDatas.HitTraumaDatas);
            Manager.FreezeFrameManager.FreezeFrame(0, CurrAttackDatas.HitFreezeFrameDur);
        }

        protected override void ComputeAttackDirection()
        {
            AttackDir = Mathf.Sign(InputCtrl.Horizontal != 0f ? InputCtrl.Horizontal : _playerCtrl.CurrDir);
        }

        private void ComputeVelocity(float t, ref Vector3 vel)
        {
            vel.x = Datas.Attack.AttackDatabase.DefaultPlayerAttackCurve.Evaluate(t) * CurrAttackDatas.MoveSpeed * AttackDir;
            vel.y -= CurrAttackDatas.Gravity * Time.deltaTime;
        }

        private System.Collections.IEnumerator ComboCoroutine(AttackOverEventHandler comboOverCallback = null)
        {
            ComputeAttackDirection();
            Vector3 attackVel = new Vector3(0f, 0f);
            bool hasHit = false;

            for (int i = 0; i < _baseComboDatas.Length; ++i)
            {
                CurrAttackDatas = _baseComboDatas[i];
                hasHit = false;

                if (CurrAttackDatas.HitDelay == 0f)
                {
                    TriggerHit(CurrAttackDatas, CurrAttackDatas.Id);
                    hasHit = true;
                }

                _playerCtrl.PlayerView.PlayAttackAnimation(AttackDir, CurrAttackDatas);
                if (_playerCtrl.CollisionsCtrl.Below)
                    _playerCtrl.PlayerView.PlayAttackVFX(AttackDir, 0.25f);

                // Attack motion.
                for (float t = 0f; t < 1f; t += Time.deltaTime / CurrAttackDatas.Dur)
                {
                    if (!hasHit && t * CurrAttackDatas.Dur > CurrAttackDatas.HitDelay)
                    {
                        Debug.Log($"Hitting after {CurrAttackDatas.HitDelay / CurrAttackDatas.Dur}");
                        TriggerHit(CurrAttackDatas, CurrAttackDatas.Id);
                        hasHit = true;
                    }

                    CanChainAttack = t * CurrAttackDatas.Dur >= CurrAttackDatas.ChainAllowedTime;
                    if (CanChainAttack)
                    {
                        if (InputCtrl.CheckInput(Unit.Player.PlayerInputController.ButtonCategory.ROLL) || InputCtrl.CheckInput(Unit.Player.PlayerInputController.ButtonCategory.JUMP))
                            break;

                        if (InputCtrl.CheckInput(Unit.Player.PlayerInputController.ButtonCategory.ATTACK) && i < _baseComboDatas.Length - 1)
                            break;
                    }

                    if (CurrAttackDatas.ControlVelocity)
                    {
                        ComputeVelocity(t, ref attackVel);
                        bool checkEdge = _playerCtrl.CollisionsCtrl.Below;
                        _playerCtrl.Translate(attackVel, checkEdge: checkEdge);
                    }

                    yield return null;
                }

                if (InputCtrl.CheckInput(Unit.Player.PlayerInputController.ButtonCategory.ROLL) || InputCtrl.CheckInput(Unit.Player.PlayerInputController.ButtonCategory.JUMP))
                {
                    CProLogger.Log(this, "Roll or jump input is true, breaking out of combo.", _playerCtrl.gameObject);
                    break;
                }

                if (InputCtrl.CheckInput(Unit.Player.PlayerInputController.ButtonCategory.ATTACK) && i < _baseComboDatas.Length - 1)
                {
                    // Chained attack.
                    InputCtrl.ResetDelayedInput(Unit.Player.PlayerInputController.ButtonCategory.ATTACK);
                    ComputeAttackDirection();
                }
                else
                {
                    break;
                }
            }

            CProLogger.Log(this, $"Combo end with a direction of {AttackDir}.", _playerCtrl.gameObject);

            comboOverCallback?.Invoke(new AttackOverEventArgs(CurrAttackDatas, AttackDir));
            _attackCoroutine = null;
            CurrAttackDatas = null;

            _playerCtrl.PlayerView.PlayIdleAnimation();
        }

        private System.Collections.IEnumerator AirborneAttackCoroutine(AttackOverEventHandler attackOverCallback = null)
        {
            ComputeAttackDirection();
            Vector3 attackVel = Vector3.zero;

            CurrAttackDatas = _airborneAttackDatas;
            TriggerHit(CurrAttackDatas, CurrAttackDatas.Id);

            _playerCtrl.PlayerView.PlayAttackAnimation(AttackDir, CurrAttackDatas);

            for (float t = 0f; t < 1f; t += Time.deltaTime / CurrAttackDatas.Dur)
            {
                CanChainAttack = t * CurrAttackDatas.Dur >= CurrAttackDatas.ChainAllowedTime;

                if (CurrAttackDatas.ControlVelocity)
                {
                    ComputeVelocity(t, ref attackVel);
                    _playerCtrl.Translate(attackVel);
                }

                if (_playerCtrl.CollisionsCtrl.CurrentStates.GetCollisionState(Templar.Physics.CollisionsController.CollisionOrigin.BELOW))
                    break;

                yield return null;
            }

            attackOverCallback?.Invoke(new AttackOverEventArgs(CurrAttackDatas, AttackDir));
            _attackCoroutine = null;
            CurrAttackDatas = null;

            // This will lead to fall animation instantly, but this is done to exit the attack animation.
            _playerCtrl.PlayerView.PlayIdleAnimation();
        }
    }
}