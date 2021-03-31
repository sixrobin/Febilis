namespace Templar.Unit.Enemy.Conditions
{
    public class PlayerDetectedEnemyCondition : EnemyCondition<Datas.Unit.Enemy.PlayerDetectedEnemyConditionDatas>
    {
        private bool _isPlayerDetected;

        public PlayerDetectedEnemyCondition(EnemyController enemyCtrl, Datas.Unit.Enemy.PlayerDetectedEnemyConditionDatas conditionDatas)
            : base(enemyCtrl, conditionDatas)
        {
        }

        public override bool Check()
        {
            _isPlayerDetected = _isPlayerDetected ? !CheckPlayerLost() : CheckPlayerDetection();
            return ApplyNegation(_isPlayerDetected);
        }

        private bool CheckPlayerDetection()
        {
            return (EnemyCtrl.Player.transform.position - EnemyCtrl.transform.position).sqrMagnitude <= EnemyCtrl.EnemyDatas.PlayerDetectionDistSqr;
        }

        private bool CheckPlayerLost()
        {
            return (EnemyCtrl.Player.transform.position - EnemyCtrl.transform.position).sqrMagnitude > EnemyCtrl.EnemyDatas.PlayerLoseDistSqr;
        }
    }
}