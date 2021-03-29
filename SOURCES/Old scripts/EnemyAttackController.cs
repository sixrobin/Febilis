namespace Templar.Attack
{
    public class EnemyAttackController : AttackController
    {
        private Unit.Enemy.SkeletonController _skeletonController;
        private Datas.Attack.SkeletonAttackDatas _currAttackDatas;

        public EnemyAttackController(Unit.Enemy.SkeletonController skeletonController)
            : base(skeletonController, skeletonController.AttackHitboxesContainer, skeletonController.transform)
        {
            _skeletonController = skeletonController;
        }

        public void Attack(AttackOverEventHandler attackOverCallback = null)
        {
            _currAttackDatas = _skeletonController.FightBehaviourDatas.BaseAttack;
            _attackCoroutine = AttackCoroutine(attackOverCallback);
            _attackCoroutineRunner.StartCoroutine(_attackCoroutine);
        }

        public void AttackAbove(AttackOverEventHandler attackOverCallback = null)
        {
            _currAttackDatas = _skeletonController.FightBehaviourDatas.AboveAttack;
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
            TriggerHit(_currAttackDatas, _currAttackDatas.Id);

            yield return RSLib.Yield.SharedYields.WaitForSeconds(_currAttackDatas.AttackDur);

            _attackCoroutine = null;
            attackOverCallback?.Invoke(new AttackOverEventArgs(AttackDir));

            _skeletonController.SkeletonView.PlayIdleAnimation();
        }
    }
}