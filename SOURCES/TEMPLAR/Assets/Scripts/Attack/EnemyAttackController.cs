namespace Templar.Attack
{
    public class EnemyAttackController : AttackController
    {
        private Unit.Enemy.EnemyController _enemyCtrl;
        private Datas.Attack.SkeletonAttackDatas _currAttackDatas;

        public EnemyAttackController(Unit.Enemy.EnemyController enemyCtrl)
            : base(enemyCtrl, enemyCtrl.AttackHitboxesContainer, enemyCtrl.transform)
        {
            _currAttackDatas = enemyCtrl._tmpAttackDatas;
            _enemyCtrl = enemyCtrl;
        }

        public void Attack(Unit.Enemy.Actions.AttackEnemyAction attackAction, AttackOverEventHandler attackOverCallback = null)
        {
            _attackCoroutine = AttackCoroutine(attackAction, attackOverCallback);
            _attackCoroutineRunner.StartCoroutine(_attackCoroutine);
        }

        protected override void OnAttackHit(AttackHitbox.HitEventArgs hitArgs)
        {
        }

        protected override void ComputeAttackDirection()
        {
            AttackDir = _enemyCtrl.CurrDir;
        }

        private System.Collections.IEnumerator AttackCoroutine(Unit.Enemy.Actions.AttackEnemyAction attackAction, AttackOverEventHandler attackOverCallback = null)
        {
            ComputeAttackDirection();
            _enemyCtrl.EnemyView.PlayAttackAnticipationAnimation(attackAction.ActionDatas.AnimatorSuffix);

            yield return RSLib.Yield.SharedYields.WaitForSeconds(_currAttackDatas.AttackAnticipationDur);

            _enemyCtrl.EnemyView.PlayAttackAnimation(attackAction.ActionDatas.AnimatorSuffix);
            TriggerHit(_currAttackDatas, attackAction.ActionDatas.Id);

            yield return RSLib.Yield.SharedYields.WaitForSeconds(_currAttackDatas.AttackDur);

            _attackCoroutine = null;
            attackOverCallback?.Invoke(new AttackOverEventArgs(AttackDir));

            _enemyCtrl.EnemyView.PlayIdleAnimation();
        }
    }
}