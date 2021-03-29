namespace Templar.Unit.Enemy.Conditions
{
    public class FullHealthEnemyCondition : EnemyCondition<Datas.Unit.Enemy.FullHealthEnemyConditionDatas>
    {
        public FullHealthEnemyCondition(EnemyController enemyCtrl, Datas.Unit.Enemy.FullHealthEnemyConditionDatas conditionDatas)
            : base(enemyCtrl, conditionDatas)
        {
        }

        public override bool Check()
        {
            return ApplyNegation(EnemyCtrl.HealthCtrl.HealthSystem.HealthPercentage == 1f);
        }
    }
}