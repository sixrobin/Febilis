namespace Templar.Unit.Enemy.Conditions
{
    public class HealthMaxEnemyCondition : EnemyCondition<Datas.Unit.Enemy.HealthMaxEnemyConditionDatas>
    {
        public HealthMaxEnemyCondition(EnemyController enemyCtrl, Datas.Unit.Enemy.HealthMaxEnemyConditionDatas conditionDatas)
            : base(enemyCtrl, conditionDatas)
        {
        }

        public override bool Check()
        {
            return ApplyNegation(EnemyCtrl.HealthCtrl.HealthSystem.CurrentHealth <= ConditionsDatas.Threshold);
        }
    }
}