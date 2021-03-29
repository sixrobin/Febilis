namespace Templar.Unit.Enemy.Conditions
{
    public class PlayerDetectedEnemyCondition : EnemyCondition<Datas.Unit.Enemy.PlayerDetectedEnemyConditionDatas>
    {
        public PlayerDetectedEnemyCondition(EnemyController enemyCtrl, Datas.Unit.Enemy.PlayerDetectedEnemyConditionDatas conditionDatas)
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