using UnityEngine;

public class TemplarJumpController
{
    private TemplarJumpDatas _jumpDatas;
    private TemplarController _templarController;

    private System.Collections.IEnumerator _jumpAnticipationCoroutine;
    private System.Collections.IEnumerator _landImpactCoroutine;

    public TemplarJumpController(TemplarController templarController)
    {
        _templarController = templarController;
        _jumpDatas = _templarController.ControllerDatas.Jump;
        ResetJumpsLeft();
    }

    public bool IsAnticipatingJump => _jumpAnticipationCoroutine != null;
    public bool IsInLandImpact => _landImpactCoroutine != null;

    public int JumpsLeft { get; set; }
    public float LandImpactSpeedMult { get; private set; }

    public void ResetJumpsLeft()
    {
        JumpsLeft = _jumpDatas.MaxFollowingJumps;
    }

    public void JumpAfterAnticipation(bool airborne = false)
    {
        _jumpAnticipationCoroutine = JumpAfterAnticipationCoroutine(airborne ? _jumpDatas.AirborneJumpAnticipationDur : _jumpDatas.JumpAnticipationDur);
        _templarController.StartCoroutine(_jumpAnticipationCoroutine);

        if (airborne)
            _templarController.TemplarView.PlayDoubleJumpAnimation();
        else
            _templarController.TemplarView.PlayJumpAnimation(_templarController.InputCtrl.Horizontal);
    }

    public void TriggerLandImpact(float fallSpeedAbs)
    {
        _landImpactCoroutine = WaitForLandImpactCoroutine(fallSpeedAbs);
        _templarController.StartCoroutine(_landImpactCoroutine);

        _templarController.TemplarView.PlayLandAnimation(fallSpeedAbs);
    }

    private System.Collections.IEnumerator JumpAfterAnticipationCoroutine(float dur)
    {
        yield return RSLib.Yield.SharedYields.WaitForSeconds(dur);

        _templarController.Jump();

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

        yield return RSLib.Yield.SharedYields.WaitForSeconds(impactDur);
        _landImpactCoroutine = null;

        // Known potential issue: this might cause the templar to stay grounded without jumping if the store jump
        // input coroutine ends on the exact frame than this coroutine.
        if (!_templarController.InputCtrl.CheckInput(TemplarInputController.ButtonCategory.JUMP)
            && !_templarController.JumpAllowedThisFrame
            && !_templarController.RollCtrl.IsRolling
            && !_templarController.AttackCtrl.IsAttacking)
            _templarController.TemplarView.PlayIdleAnimation();
    }
}