namespace Templar.Unit.Enemy.Conditions
{
    public class PlayerAboveEnemyCondition : EnemyCondition<Datas.Unit.Enemy.PlayerAboveEnemyConditionDatas>
    {
        public PlayerAboveEnemyCondition(EnemyController enemyCtrl, Datas.Unit.Enemy.PlayerAboveEnemyConditionDatas conditionDatas)
            : base(enemyCtrl, conditionDatas)
        {
        }

        public override bool Check()
        {
            return ApplyNegation(EnemyCtrl.IsPlayerAbove);
        }
    }
}