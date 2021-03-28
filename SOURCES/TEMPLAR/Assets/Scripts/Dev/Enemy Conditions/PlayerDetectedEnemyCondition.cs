namespace Templar.Dev
{
    public class PlayerDetectedEnemyCondition : EnemyCondition<PlayerDetectedEnemyConditionDatas>
    {
        public PlayerDetectedEnemyCondition(GenericEnemyController enemyCtrl, PlayerDetectedEnemyConditionDatas conditionDatas)
            : base(enemyCtrl, conditionDatas)
        {
        }

        public override bool Check()
        {
            return ApplyNegation((EnemyCtrl.Player.transform.position - EnemyCtrl.transform.position).sqrMagnitude
                <= EnemyCtrl.EnemyDatas.PlayerDetectionDistSqr);
        }
    }
}