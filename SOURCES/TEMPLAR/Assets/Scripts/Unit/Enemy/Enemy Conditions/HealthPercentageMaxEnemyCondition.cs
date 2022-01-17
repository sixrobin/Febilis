namespace Templar.Unit.Enemy.Conditions
{
    public class HealthPercentageMaxEnemyCondition : EnemyCondition<Datas.Unit.Enemy.HealthPercentageMaxEnemyConditionDatas>
    {
        public HealthPercentageMaxEnemyCondition(EnemyController enemyCtrl, Datas.Unit.Enemy.HealthPercentageMaxEnemyConditionDatas conditionDatas)
            : base(enemyCtrl, conditionDatas)
        {
        }

        public override bool Check()
        {
            return ApplyNegation(EnemyCtrl.HealthCtrl.HealthSystem.HealthPercentage <= ConditionsDatas.Threshold);
        }
    }
}