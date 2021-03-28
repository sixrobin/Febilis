namespace Templar.Dev
{
    public class HealthMaxEnemyCondition : EnemyCondition<HealthMaxEnemyConditionDatas>
    {
        public HealthMaxEnemyCondition(GenericEnemyController enemyCtrl, HealthMaxEnemyConditionDatas conditionDatas)
            : base(enemyCtrl, conditionDatas)
        {
        }

        public override bool Check()
        {
            return ApplyNegation(EnemyCtrl.HealthCtrl.HealthSystem.CurrentHealth <= ConditionsDatas.Threshold);
        }
    }
}