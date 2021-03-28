namespace Templar.Dev
{
    public class PlayerInRangeEnemyCondition : EnemyCondition<PlayerInRangeEnemyConditionDatas>
    {
        public PlayerInRangeEnemyCondition(GenericEnemyController enemyCtrl, PlayerInRangeEnemyConditionDatas conditionDatas)
            : base(enemyCtrl, conditionDatas)
        {
        }

        public override bool Check()
        {
            return ApplyNegation((EnemyCtrl.Player.transform.position - EnemyCtrl.transform.position).sqrMagnitude
                <= ConditionsDatas.Range * ConditionsDatas.Range);
        }
    }
}