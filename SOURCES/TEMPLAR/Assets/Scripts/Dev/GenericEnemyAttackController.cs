namespace Templar.Dev
{
    public class GenericEnemyAttackController : Attack.AttackController
    {
        private GenericEnemyController _enemyCtrl;
        private Templar.Attack.Datas.SkeletonAttackDatas _currAttackDatas;

        public GenericEnemyAttackController(GenericEnemyController enemyCtrl)
            : base(enemyCtrl, enemyCtrl.AttackHitboxesContainer, enemyCtrl.transform)
        {
            _currAttackDatas = enemyCtrl._tmpAttackDatas;
            _enemyCtrl = enemyCtrl;
        }

        public void Attack(AttackOverEventHandler attackOverCallback = null)
        {
            _attackCoroutine = AttackCoroutine(attackOverCallback);
            _attackCoroutineRunner.StartCoroutine(_attackCoroutine);
        }

        protected override void OnAttackHit(Attack.AttackHitbox.HitEventArgs hitArgs)
        {
        }

        protected override void ComputeAttackDirection()
        {
            AttackDir = _enemyCtrl.CurrDir;
        }

        private System.Collections.IEnumerator AttackCoroutine(AttackOverEventHandler attackOverCallback = null)
        {
            ComputeAttackDirection();
            _enemyCtrl.EnemyView.PlayAttackAnticipationAnimation(_currAttackDatas.AnimatorParamsSuffix);

            yield return RSLib.Yield.SharedYields.WaitForSeconds(_currAttackDatas.AttackAnticipationDur);

            _enemyCtrl.EnemyView.PlayAttackAnimation(_currAttackDatas.AnimatorParamsSuffix);
            TriggerHit(_currAttackDatas);

            yield return RSLib.Yield.SharedYields.WaitForSeconds(_currAttackDatas.AttackDur);

            _attackCoroutine = null;
            attackOverCallback?.Invoke(new AttackOverEventArgs(AttackDir));

            _enemyCtrl.EnemyView.PlayIdleAnimation();
        }
    }
}