public class SkeletonAttackController : AttackController
{
    private SkeletonAttackDatas _attackDatas;
    private SkeletonController _skeletonController;

    public SkeletonAttackController(SkeletonController skeletonController)
        : base(skeletonController, skeletonController.AttackHitboxesContainer)
    {
        _skeletonController = skeletonController;
        _attackDatas = _skeletonController.ControllerDatas.BaseAttack;
    }

    public void Attack(AttackOverEventHandler attackOverCallback = null)
    {
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
        _skeletonController.SkeletonView.PlayAttackAnticipationAnimation();

        yield return RSLib.Yield.SharedYields.WaitForSeconds(_attackDatas.AttackAnticipationDur);

        _skeletonController.SkeletonView.PlayAttackAnimation();
        TriggerHit(_attackDatas);

        yield return RSLib.Yield.SharedYields.WaitForSeconds(_attackDatas.AttackDur);

        _attackCoroutine = null;
        attackOverCallback?.Invoke(new AttackOverEventArgs(AttackDir));

        _skeletonController.SkeletonView.PlayIdleAnimation();
    }
}