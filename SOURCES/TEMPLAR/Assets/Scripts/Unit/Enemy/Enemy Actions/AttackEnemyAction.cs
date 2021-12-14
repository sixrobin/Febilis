namespace Templar.Unit.Enemy.Actions
{
    public class AttackEnemyAction : EnemyAction<Datas.Unit.Enemy.AttackEnemyActionDatas>
    {
        private float _delayTimer;
        private float _nextDelay;

        public AttackEnemyAction(EnemyController enemyCtrl, Datas.Unit.Enemy.AttackEnemyActionDatas actionDatas)
            : base(enemyCtrl, actionDatas)
        {
            ComputeNextDelay();
        }

        public override bool CanExit()
        {
            return !EnemyCtrl.AttackCtrl.IsAttacking;
        }

        public override void Execute()
        {
            if (EnemyCtrl.AttackCtrl.IsAttacking)
                return;

            EnemyCtrl.EnemyView.FlipX(EnemyCtrl.PlayerCtrl.transform.position.x - EnemyCtrl.transform.position.x < 0f);
            EnemyCtrl.EnemyView.PlayWalkAnimation(false);

            _delayTimer += UnityEngine.Time.deltaTime;
            if (_delayTimer < _nextDelay || EnemyCtrl.BeingHurt)
                return;

            _delayTimer = 0f;

            EnemyCtrl.SetDirection(UnityEngine.Mathf.Sign(EnemyCtrl.PlayerCtrl.transform.position.x - EnemyCtrl.transform.position.x));
            EnemyCtrl.AttackCtrl.Attack(this, (dir) => EnemyCtrl.ForceUpdateCurrentAction());
        }

        public override void OnExit()
        {
            base.OnExit();

            _delayTimer = 0f;
            ComputeNextDelay();
        }

        private void ComputeNextDelay()
        {
            _nextDelay = UnityEngine.Random.Range(ActionDatas.Delay.min, ActionDatas.Delay.max);
        }
    }
}