namespace Templar.Unit.Player
{
    using UnityEngine;

    public class PlayerRollController
    {
        private Datas.Unit.UnitRollDatas _rollDatas;
        private PlayerController _playerCtrl;

        private System.Collections.IEnumerator _rollCoroutine;
        private System.Collections.IEnumerator _rollCooldownCoroutine;

        public PlayerRollController(PlayerController playerCtrl)
        {
            _playerCtrl = playerCtrl;
            _rollDatas = _playerCtrl.CtrlDatas.Roll;
        }

        public bool IsRolling => _rollCoroutine != null;
        public bool IsRollingOrInCooldown => IsRolling || _rollCooldownCoroutine != null;

        public void Interrupt()
        {
            if (_rollCoroutine == null)
                return;

            _playerCtrl.StopCoroutine(_rollCoroutine);
            _rollCoroutine = null;

            if (!_playerCtrl.IsDead)
                _playerCtrl.PlayerView.PlayIdleAnimation();
        }

        public void Roll(float dir)
        {
            _rollCoroutine = RollCoroutine(dir);
            _playerCtrl.StartCoroutine(_rollCoroutine);
        }

        private System.Collections.IEnumerator RollCoroutine(float dir)
        {
            _playerCtrl.PlayerView.PlayRollAnimation(dir);

            Vector3 rollVel = new Vector3(0f, 0f);

            for (float t = 0f; t < 1f; t += Time.deltaTime / _rollDatas.Dur)
            {
                rollVel.x = _rollDatas.SpeedCurve.Evaluate(t) * _rollDatas.Speed * dir;
                rollVel.y += _playerCtrl.Gravity * Time.deltaTime * _rollDatas.GravityMult;
                _playerCtrl.Translate(rollVel);

                yield return null;
            }

            if (_rollDatas.HasCooldown)
            {
                UnityEngine.Assertions.Assert.IsTrue(_rollCooldownCoroutine == null, "Roll is about to end and cooldown coroutine is already running.");
                _rollCooldownCoroutine = RollCooldownCoroutine();
                _playerCtrl.StartCoroutine(_rollCooldownCoroutine);
            }

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