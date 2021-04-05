namespace Templar.Unit.Enemy.Conditions
{
    public class PlayerInRangeEnemyCondition : EnemyCondition<Datas.Unit.Enemy.PlayerInRangeEnemyConditionDatas>
    {
        public PlayerInRangeEnemyCondition(EnemyController enemyCtrl, Datas.Unit.Enemy.PlayerInRangeEnemyConditionDatas conditionDatas)
            : base(enemyCtrl, conditionDatas)
        {
        }

        public override bool Check()
        {
            return ApplyNegation((EnemyCtrl.PlayerCtrl.transform.position - EnemyCtrl.transform.position).sqrMagnitude
                <= ConditionsDatas.RangeSqr);
        }
    }
}