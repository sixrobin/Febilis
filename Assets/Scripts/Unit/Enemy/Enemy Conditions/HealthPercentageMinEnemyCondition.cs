namespace Templar.Unit.Enemy.Conditions
{
    public class HealthPercentageMinEnemyCondition : EnemyCondition<Datas.Unit.Enemy.HealthPercentageMinEnemyConditionDatas>
    {
        public HealthPercentageMinEnemyCondition(EnemyController enemyCtrl, Datas.Unit.Enemy.HealthPercentageMinEnemyConditionDatas conditionDatas)
            : base(enemyCtrl, conditionDatas)
        {
        }

        public override bool Check()
        {
            return ApplyNegation(EnemyCtrl.HealthCtrl.HealthSystem.HealthPercentage >= ConditionsDatas.Threshold);
        }
    }
}