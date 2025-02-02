﻿namespace Templar.Unit.Player
{
    using UnityEngine;

    public class PlayerRollController
    {
        public class RollOverEventArgs : System.EventArgs
        {
            public RollOverEventArgs(Vector3 vel)
            {
                Vel = vel;
            }

            public Vector3 Vel { get; private set; }
        }

        private Datas.Unit.UnitRollDatas _rollDatas;
        private PlayerController _playerCtrl;

        private System.Collections.IEnumerator _rollCoroutine;
        private System.Collections.IEnumerator _rollCooldownCoroutine;

        public PlayerRollController(PlayerController playerCtrl)
        {
            _playerCtrl = playerCtrl;
            _rollDatas = _playerCtrl.CtrlDatas.Roll;

            _playerCtrl.CtrlDatas.ValuesValidated += OnDatasValuesChanged;
        }

        ~PlayerRollController()
        {
            _playerCtrl.CtrlDatas.ValuesValidated -= OnDatasValuesChanged;
        }

        public delegate void RollOverEventHandler(RollOverEventArgs args);
        public delegate void RolledEventHandler();

        public event RolledEventHandler Rolled;
        
        public bool IsRolling => _rollCoroutine != null;
        public bool IsRollingOrInCooldown => IsRolling || _rollCooldownCoroutine != null;

        public bool CanRoll()
        {
            return !IsRollingOrInCooldown
                && !_playerCtrl.AttackCtrl.IsAttacking
                && _playerCtrl.CollisionsCtrl.Below
                && !_playerCtrl.JumpCtrl.IsInLandImpact
                && !_playerCtrl.IsBeingHurt
                && !_playerCtrl.IsHealing
                && _playerCtrl.InputCtrl.CheckInput(PlayerInputController.ButtonCategory.ROLL);
        }

        public void Interrupt()
        {
            if (!IsRolling)
                return;

            _playerCtrl.StopCoroutine(_rollCoroutine);
            _rollCoroutine = null;

            if (!_playerCtrl.IsDead)
                _playerCtrl.PlayerView.PlayIdleAnimation();
        }

        public void Roll(float dir, RollOverEventHandler rollOverCallback = null)
        {
            _playerCtrl.StartCoroutine(_rollCoroutine = RollCoroutine(dir, rollOverCallback));
        }

        private void OnDatasValuesChanged()
        {
            if (_rollDatas != _playerCtrl.CtrlDatas.Roll)
                _rollDatas = _playerCtrl.CtrlDatas.Roll;
        }

        private System.Collections.IEnumerator RollCoroutine(float dir, RollOverEventHandler rollOverCallback = null)
        {
            Rolled?.Invoke();
            
            _playerCtrl.PlayerView.PlayRollAnimation(dir);

            Vector3 rollVel = new Vector3(0f, 0f);

            for (float t = 0f; t < 1f; t += Time.deltaTime / _rollDatas.Dur)
            {
                rollVel.x = _rollDatas.SpeedCurve.Evaluate(t) * _rollDatas.Speed * dir;
                rollVel.y += _playerCtrl.JumpCtrl.Gravity * Time.deltaTime * _rollDatas.GravityMultCurve.Evaluate(t);
                _playerCtrl.Translate(rollVel, checkEdge: t * _rollDatas.Dur >= _rollDatas.EdgeDetectionThreshold);
                
                yield return null;
            }

            if (_rollDatas.HasCooldown)
            {
                UnityEngine.Assertions.Assert.IsTrue(_rollCooldownCoroutine == null, "Roll is about to end and cooldown coroutine is already running.");
                _playerCtrl.StartCoroutine(_rollCooldownCoroutine = RollCooldownCoroutine());
            }

            rollOverCallback?.Invoke(new RollOverEventArgs(rollVel));

            _rollCoroutine = null;
            _playerCtrl.PlayerView.PlayIdleAnimation();
        }

        private System.Collections.IEnumerator RollCooldownCoroutine()
        {
            yield return RSLib.Yield.SharedYields.WaitForSeconds(_rollDatas.Cooldown);
            _rollCooldownCoroutine = null;
        }
    }
}