using UnityEngine;

public class TemplarRollController
{
    private TemplarRollDatas _rollDatas;
    private TemplarController _templarController;

    private System.Collections.IEnumerator _rollCoroutine;
    private System.Collections.IEnumerator _rollCooldownCoroutine;

    public TemplarRollController(TemplarController templarController)
    {
        _templarController = templarController;
        _rollDatas = _templarController.ControllerDatas.Roll;
    }

    public bool IsRolling => _rollCoroutine != null;
    public bool IsRollingOrInCooldown => IsRolling || _rollCooldownCoroutine != null;

    public void Interrupt()
    {
        if (_rollCoroutine == null)
            return;

        _templarController.StopCoroutine(_rollCoroutine);
        _rollCoroutine = null;

        _templarController.TemplarView.PlayIdleAnimation();
        //_templarController.TemplarView.DBG_Color(Color.white);
    }

    public void Roll(float dir)
    {
        _rollCoroutine = RollCoroutine(dir);
        _templarController.StartCoroutine(_rollCoroutine);
    }

    private System.Collections.IEnumerator RollCoroutine(float dir)
    {
        //_templarView.DBG_Color(RSLib.Extensions.ColorExtensions.BlendColors(Color.yellow, Color.cyan, Color.white));
        _templarController.TemplarView.PlayRollAnimation(dir);

        Vector3 rollVel = new Vector3(0f, 0f);

        for (float t = 0f; t < 1f; t += Time.deltaTime / _rollDatas.Dur)
        {
            rollVel.x = _rollDatas.SpeedCurve.Evaluate(t) * _rollDatas.Speed * dir;
            rollVel.y += _templarController.Gravity * Time.deltaTime * _rollDatas.GravityMult;
            _templarController.Translate(rollVel);

            yield return null;
        }

        if (_rollDatas.HasCooldown)
        {
            UnityEngine.Assertions.Assert.IsTrue(_rollCooldownCoroutine == null, "Roll is about to end and cooldown coroutine is already running.");
            _rollCooldownCoroutine = RollCooldownCoroutine();
            _templarController.StartCoroutine(_rollCooldownCoroutine);
        }

        _rollCoroutine = null;
        _templarController.TemplarView.PlayIdleAnimation();
        _templarController.TemplarView.DBG_Color(Color.white);
    }

    private System.Collections.IEnumerator RollCooldownCoroutine()
    {
        yield return RSLib.Yield.SharedYields.WaitForSeconds(_rollDatas.Cooldown);
        _rollCooldownCoroutine = null;
    }
}