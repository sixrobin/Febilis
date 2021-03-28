namespace Templar.Dev
{
    public class FullHealthEnemyCondition : EnemyCondition<FullHealthEnemyConditionDatas>
    {
        public FullHealthEnemyCondition(GenericEnemyController enemyCtrl, FullHealthEnemyConditionDatas conditionDatas)
            : base(enemyCtrl, conditionDatas)
        {
        }

        public override bool Check()
        {
            return ApplyNegation(EnemyCtrl.HealthCtrl.HealthSystem.HealthPercentage == 1f);
        }
    }
}