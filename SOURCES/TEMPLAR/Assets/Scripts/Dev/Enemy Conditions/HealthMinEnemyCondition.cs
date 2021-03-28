namespace Templar.Dev
{
    public class HealthMinEnemyCondition : EnemyCondition<HealthMinEnemyConditionDatas>
    {
        public HealthMinEnemyCondition(GenericEnemyController enemyCtrl, HealthMinEnemyConditionDatas conditionDatas)
            : base(enemyCtrl, conditionDatas)
        {
        }

        public override bool Check()
        {
            return ApplyNegation(EnemyCtrl.HealthCtrl.HealthSystem.CurrentHealth >= ConditionsDatas.Threshold);
        }
    }
}