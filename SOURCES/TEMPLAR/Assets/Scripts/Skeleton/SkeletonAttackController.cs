﻿public class SkeletonAttackController : AttackController
{
    private SkeletonController _skeletonController;
    private SkeletonAttackDatas _currAttackDatas;

    public SkeletonAttackController(SkeletonController skeletonController)
        : base(skeletonController, skeletonController.AttackHitboxesContainer)
    {
        _skeletonController = skeletonController;
    }

    public void Attack(AttackOverEventHandler attackOverCallback = null)
    {
        _currAttackDatas = _skeletonController.ControllerDatas.BaseAttack;
        _attackCoroutine = AttackCoroutine(attackOverCallback);
        _attackCoroutineRunner.StartCoroutine(_attackCoroutine);
    }

    public void AttackAbove(AttackOverEventHandler attackOverCallback = null)
    {
        _currAttackDatas = _skeletonController.ControllerDatas.AboveAttack;
        _attackCoroutine = AttackCoroutine(attackOverCallback);
        _attackCoroutineRunner.StartCoroutine(_attackCoroutine);
    }

    protected override void OnAttackHit(AttackHitbox.HitEventArgs hitArgs)
    {
    }

    protected override void ComputeAttackDirection()
    {
        AttackDir = _skeletonController.CurrDir;
    }

    private System.Collections.IEnumerator AttackCoroutine(AttackOverEventHandler attackOverCallback = null)
    {
        ComputeAttackDirection();
        _skeletonController.SkeletonView.PlayAttackAnticipationAnimation(_currAttackDatas.AnimatorParamsSuffix);

        yield return RSLib.Yield.SharedYields.WaitForSeconds(_currAttackDatas.AttackAnticipationDur);

        _skeletonController.SkeletonView.PlayAttackAnimation(_currAttackDatas.AnimatorParamsSuffix);
        TriggerHit(_currAttackDatas);

        yield return RSLib.Yield.SharedYields.WaitForSeconds(_currAttackDatas.AttackDur);

        _attackCoroutine = null;
        attackOverCallback?.Invoke(new AttackOverEventArgs(AttackDir));

        _skeletonController.SkeletonView.PlayIdleAnimation();
    }
}