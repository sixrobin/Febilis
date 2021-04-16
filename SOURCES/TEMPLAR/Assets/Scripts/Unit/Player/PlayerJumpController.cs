namespace Templar.Unit.Player
{
    using UnityEngine;

    public class PlayerJumpController
    {
        private Datas.Unit.UnitJumpDatas _jumpDatas;
        private PlayerController _playerCtrl;

        private System.Collections.IEnumerator _jumpAnticipationCoroutine;
        private System.Collections.IEnumerator _landImpactCoroutine;

        public PlayerJumpController(PlayerController playerCtrl)
        {
            _playerCtrl = playerCtrl;
            _jumpDatas = _playerCtrl.CtrlDatas.Jump;
            ResetJumpsLeft();
        }

        public bool IsAnticipatingJump => _jumpAnticipationCoroutine != null;
        public bool IsInLandImpact => _landImpactCoroutine != null;

        public int JumpsLeft { get; set; }
        public float LandImpactSpeedMult { get; private set; }

        public bool CanJump()
        {
            return JumpsLeft > 0
                && _playerCtrl.InputCtrl.CheckInput(PlayerInputController.ButtonCategory.JUMP)
                && !IsInLandImpact
                && !IsAnticipatingJump
                && (_playerCtrl.AttackCtrl.CurrAttackDatas == null || _playerCtrl.AttackCtrl.CanChainAttack)
                && !_playerCtrl.IsBeingHurt
                && !_playerCtrl.IsHealing;
        }

        public void ResetJumpsLeft()
        {
            JumpsLeft = _jumpDatas.MaxFollowingJumps;
        }

        public void JumpAfterAnticipation(bool airborne = false)
        {
            _jumpAnticipationCoroutine = JumpAfterAnticipationCoroutine(airborne ? _jumpDatas.AirborneJumpAnticipationDur : _jumpDatas.JumpAnticipationDur);
            _playerCtrl.StartCoroutine(_jumpAnticipationCoroutine);

            if (airborne)
                _playerCtrl.PlayerView.PlayDoubleJumpAnimation();
            else
                _playerCtrl.PlayerView.PlayJumpAnimation(_playerCtrl.InputCtrl.Horizontal);
        }

        public void TriggerLandImpact(float fallSpeedAbs)
        {
            _landImpactCoroutine = WaitForLandImpactCoroutine(fallSpeedAbs);
            _playerCtrl.StartCoroutine(_landImpactCoroutine);

            _playerCtrl.PlayerView.PlayLandAnimation(fallSpeedAbs);
        }

        private System.Collections.IEnumerator JumpAfterAnticipationCoroutine(float dur)
        {
            yield return RSLib.Yield.SharedYields.WaitForSeconds(dur);

            _playerCtrl.Jump();

            for (int i = 0; i < 2; ++i)
                yield return RSLib.Yield.SharedYields.WaitForEndOfFrame;

            _jumpAnticipationCoroutine = null;
        }

        private System.Collections.IEnumerator WaitForLandImpactCoroutine(float fallSpeedAbs)
        {
            float impactDur = Mathf.Clamp(fallSpeedAbs * _jumpDatas.LandImpactDurFactor, _jumpDatas.LandImpactDurMin, _jumpDatas.LandImpactDurMax);
            LandImpactSpeedMult = RSLib.Maths.Maths.Normalize01(impactDur, _jumpDatas.LandImpactDurMin, _jumpDatas.LandImpactDurMax);
            LandImpactSpeedMult = 1f - LandImpactSpeedMult;
            LandImpactSpeedMult = Mathf.Max(LandImpactSpeedMult, _jumpDatas.LandImpactSpeedMultMin);
            CProLogger.Log(this, $"Landing at a speed of {fallSpeedAbs.ToString("f2")} => Computed impact duration of {impactDur.ToString("f2")}s and a speed mult of {LandImpactSpeedMult.ToString("f2")}.");

            // [TMP] Hardcoded values : shake should probably scale with impact strength.
            _playerCtrl.CameraCtrl.Shake.AddTrauma(0f, 0.3f);

            yield return RSLib.Yield.SharedYields.WaitForSeconds(impactDur);
            _landImpactCoroutine = null;

            // Known potential issue: this might cause the templar to stay grounded without jumping if the store jump
            // input coroutine ends on the exact frame than this coroutine.
            if (!_playerCtrl.InputCtrl.CheckInput(PlayerInputController.ButtonCategory.JUMP)
                && !_playerCtrl.JumpAllowedThisFrame
                && !_playerCtrl.RollCtrl.IsRolling
                && !_playerCtrl.AttackCtrl.IsAttacking)
                _playerCtrl.PlayerView.PlayIdleAnimation();
        }
    }
}