namespace Templar.Dev
{
    public class AttackEnemyAction : EnemyAction<AttackEnemyActionDatas>
    {
        private float _delayTimer;
        private float _nextDelay;

        public AttackEnemyAction(GenericEnemyController enemyCtrl, AttackEnemyActionDatas actionDatas)
            : base(enemyCtrl, actionDatas)
        {
            ComputeNextDelay();
        }

        public override void Execute()
        {
            if (EnemyCtrl.AttackCtrl.IsAttacking)
                return;

            EnemyCtrl.EnemyView.FlipX(EnemyCtrl.Player.transform.position.x - EnemyCtrl.transform.position.x < 0f);

            _delayTimer += UnityEngine.Time.deltaTime;
            if (_delayTimer < _nextDelay)
                return;

            _delayTimer = 0f;

            EnemyCtrl.SetDirection(UnityEngine.Mathf.Sign(EnemyCtrl.Player.transform.position.x - EnemyCtrl.transform.position.x));
            EnemyCtrl.AttackCtrl.Attack();
        }

        public override void Reset()
        {
            base.Reset();

            _delayTimer = 0f;
            ComputeNextDelay();
        }

        private void ComputeNextDelay()
        {
            _nextDelay = UnityEngine.Random.Range(ActionDatas.Delay.min, ActionDatas.Delay.max);
        }
    }
}