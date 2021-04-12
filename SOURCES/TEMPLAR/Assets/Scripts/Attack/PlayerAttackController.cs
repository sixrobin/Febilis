﻿namespace Templar.Attack
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
            _playerCtrl.PlayerView.PlayHitVFX(hitArgs.Dir);

            UnityEngine.Assertions.Assert.IsNotNull(CurrAttackDatas, "An attack hit has been triggered but player attack datas are null.");
            Manager.GameManager.PlayerCtrl.CameraCtrl.Shake.AddTraumaFromDatas(CurrAttackDatas.HitTraumaDatas);
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

            CurrAttackDatas = _baseComboDatas[0]; // Done for attack view.
            _playerCtrl.PlayerView.PlayAttackAnimation(AttackDir);
            if (_playerCtrl.CollisionsCtrl.Below)
                _playerCtrl.PlayerView.PlayAttackVFX(AttackDir, 0.25f);

            for (int i = 0; i < _baseComboDatas.Length; ++i)
            {
                CurrAttackDatas = _baseComboDatas[i];
                TriggerHit(CurrAttackDatas, CurrAttackDatas.Id);

                // Attack motion.
                for (float t = 0f; t < 1f; t += Time.deltaTime / CurrAttackDatas.Dur)
                {
                    CanChainAttack = t >= CurrAttackDatas.ChainAllowedTime;
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
                        _playerCtrl.Translate(attackVel, checkEdge);
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

                    _playerCtrl.PlayerView.PlayChainAttackAnimation(AttackDir);
                    if (_playerCtrl.CollisionsCtrl.Below)
                        _playerCtrl.PlayerView.PlayAttackVFX(AttackDir, 0.25f);
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
            Vector3 attackVel = new Vector3(0f, 0f);

            CurrAttackDatas = _airborneAttackDatas;
            TriggerHit(CurrAttackDatas, CurrAttackDatas.Id);

            _playerCtrl.PlayerView.PlayAttackAirborneAnimation();

            for (float t = 0f; t < 1f; t += Time.deltaTime / CurrAttackDatas.Dur)
            {
                CanChainAttack = t >= CurrAttackDatas.ChainAllowedTime;

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