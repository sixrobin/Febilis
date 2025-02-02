﻿namespace Templar.Attack
{
    using UnityEngine;

    public class PlayerAttackController : AttackController
    {
        private Datas.Attack.PlayerAttackDatas[] _baseComboDatas;
        private Datas.Attack.PlayerAttackDatas[] _additionalComboDatas;
        private Datas.Attack.PlayerAttackDatas _airborneAttackDatas;
        private Unit.Player.PlayerController _playerCtrl;

        public PlayerAttackController(Unit.Player.PlayerController playerCtrl)
            : base(playerCtrl, playerCtrl.AttackHitboxesContainer, playerCtrl.transform, playerCtrl)
        {
            _playerCtrl = playerCtrl;

            _baseComboDatas = new Datas.Attack.PlayerAttackDatas[_playerCtrl.CtrlDatas.BaseComboIds.Length];
            for (int i = 0; i < _baseComboDatas.Length; ++i)
                _baseComboDatas[i] = Database.AttackDatabase.PlayerAttacksDatas[_playerCtrl.CtrlDatas.BaseComboIds[i]];

            _additionalComboDatas = new Templar.Datas.Attack.PlayerAttackDatas[_playerCtrl.CtrlDatas.AdditionalComboIds.Length];
            for (int i = 0; i < _additionalComboDatas.Length; ++i)
                _additionalComboDatas[i] = Database.AttackDatabase.PlayerAttacksDatas[_playerCtrl.CtrlDatas.AdditionalComboIds[i]];

            _airborneAttackDatas = Database.AttackDatabase.PlayerAttacksDatas[_playerCtrl.CtrlDatas.AirborneAttackId];
        }

        public Unit.Player.PlayerInputController InputCtrl => _playerCtrl.InputCtrl;

        public Datas.Attack.PlayerAttackDatas CurrAttackDatas { get; private set; }
        public bool CanAttackAirborne { get; private set; }
        public bool CanChainAttack { get; private set; }

        protected override Renderer AttackerRenderer => _playerCtrl.PlayerView.Renderer;

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
            if (hitArgs.Hittable.SpawnVFXOnHit)
                _playerCtrl.PlayerView.PlayHitVFX(hitArgs.Direction);

            UnityEngine.Assertions.Assert.IsNotNull(CurrAttackDatas, "An attack hit has been triggered but player attack datas are null.");
            
            if (hitArgs.Hittable.HitLayer != HitLayer.PICKUP)
                Manager.GameManager.CameraCtrl.ApplyShakeFromDatas(CurrAttackDatas.HitTraumaDatas);
            
            Manager.FreezeFrameManager.FreezeFrame(0, CurrAttackDatas.HitFreezeFrameDur);
        }

        protected override void ComputeAttackDirection()
        {
            AttackDir = Mathf.Sign(InputCtrl.Horizontal != 0f ? InputCtrl.Horizontal : _playerCtrl.CurrDir);
        }

        private void ComputeVelocity(float t, ref Vector3 vel)
        {
            vel.x = Database.AttackDatabase.DefaultPlayerAttackCurve.Evaluate(t) * CurrAttackDatas.MoveSpeed * AttackDir;
            vel.y -= CurrAttackDatas.Gravity * Time.deltaTime;
        }

        private System.Collections.IEnumerator ComboCoroutine(AttackOverEventHandler comboOverCallback = null)
        {
            ComputeAttackDirection();
            Vector3 attackVel = Vector3.zero;

            System.Collections.Generic.List<Datas.Attack.PlayerAttackDatas> combo = new System.Collections.Generic.List<Datas.Attack.PlayerAttackDatas>();
            
            // Compute full combo.
            {
                combo.AddRange(_baseComboDatas);

                int additionalAttacks = Manager.GameManager.InventoryCtrl.GetItemQuantity(Templar.Item.InventoryController.ITEM_ID_GODS_EMBLEM);
                if (additionalAttacks > _additionalComboDatas.Length)
                {
                    CProLogger.LogWarning(this, $"Trying to add {additionalAttacks} additional attacks to combo but only {_additionalComboDatas.Length} additional attacks are defined, clamping value.");
                    additionalAttacks = _additionalComboDatas.Length;
                }
                
                for (int i = 0; i < additionalAttacks; ++i)
                    combo.Add(_additionalComboDatas[i]);
            }

            for (int i = 0, length = combo.Count; i < length; ++i)
            {
                CurrAttackDatas = combo[i];
                bool hasHit = false;

                if (CurrAttackDatas.HitDelay == 0f)
                {
                    TriggerHit(CurrAttackDatas, CurrAttackDatas.Id);
                    hasHit = true;
                }

                _playerCtrl.PlayerView.PlayAttackAnimation(AttackDir, CurrAttackDatas);
                if (_playerCtrl.CollisionsCtrl.Below)
                    _playerCtrl.PlayerView.PlayAttackMotionVFX(AttackDir, 0.25f, CurrAttackDatas.OverrideMotionVFXId);

                // Attack motion.
                for (float t = 0f; t < 1f; t += Time.deltaTime / CurrAttackDatas.Dur)
                {
                    if (!hasHit && t * CurrAttackDatas.Dur > CurrAttackDatas.HitDelay)
                    {
                        TriggerHit(CurrAttackDatas, CurrAttackDatas.Id);
                        hasHit = true;
                    }

                    CanChainAttack = t * CurrAttackDatas.Dur >= CurrAttackDatas.ChainAllowedTime;
                    if (CanChainAttack)
                    {
                        if (InputCtrl.CheckInput(Unit.Player.PlayerInputController.ButtonCategory.ROLL) || InputCtrl.CheckInput(Unit.Player.PlayerInputController.ButtonCategory.JUMP))
                            break;

                        if (InputCtrl.CheckInput(Unit.Player.PlayerInputController.ButtonCategory.ATTACK) && i < combo.Count - 1)
                            break;
                    }

                    if (CurrAttackDatas.ControlVelocity)
                    {
                        ComputeVelocity(t, ref attackVel);
                        _playerCtrl.Translate(attackVel, checkEdge: _playerCtrl.CollisionsCtrl.Below);
                    }

                    yield return null;
                }

                // Roll or jump input is true, breaking out of combo.
                if (InputCtrl.CheckInput(Unit.Player.PlayerInputController.ButtonCategory.ROLL) || InputCtrl.CheckInput(Unit.Player.PlayerInputController.ButtonCategory.JUMP))
                    break;

                if (InputCtrl.CheckInput(Unit.Player.PlayerInputController.ButtonCategory.ATTACK) && i < combo.Count - 1)
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