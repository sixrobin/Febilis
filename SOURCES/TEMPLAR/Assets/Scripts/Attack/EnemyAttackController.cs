namespace Templar.Attack
{
    public class EnemyAttackController : AttackController
    {
        private Unit.Enemy.EnemyController _enemyCtrl;
        private Datas.Attack.EnemyAttackDatas _currAttackDatas;
        
        public EnemyAttackController(Unit.Enemy.EnemyController enemyCtrl)
            : base(enemyCtrl, enemyCtrl.AttackHitboxesContainer, enemyCtrl.transform)
        {
            _currAttackDatas = Datas.Attack.EnemyAttackDatas.Default;
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
            _enemyCtrl.EnemyView.SetupAttackOverrideClips(attackAction.ActionDatas.AnimatorId);

            ComputeAttackDirection();
            _enemyCtrl.EnemyView.PlayAttackAnticipationAnimation();

            yield return RSLib.Yield.SharedYields.WaitForSeconds(_currAttackDatas.AnticipationDur);

            _enemyCtrl.EnemyView.PlayAttackAnimation();
            TriggerHit(_currAttackDatas, attackAction.ActionDatas.Id);

            yield return RSLib.Yield.SharedYields.WaitForSeconds(_currAttackDatas.AttackDur);

            attackOverCallback?.Invoke(new AttackOverEventArgs(_currAttackDatas, AttackDir));
            _attackCoroutine = null;

            _enemyCtrl.EnemyView.PlayIdleAnimation();
        }
    }
}