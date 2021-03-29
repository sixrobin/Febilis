namespace Templar.Unit.Enemy.Conditions
{
    public class HealthMinEnemyCondition : EnemyCondition<Datas.Unit.Enemy.HealthMinEnemyConditionDatas>
    {
        public HealthMinEnemyCondition(EnemyController enemyCtrl, Datas.Unit.Enemy.HealthMinEnemyConditionDatas conditionDatas)
            : base(enemyCtrl, conditionDatas)
        {
        }

        public override bool Check()
        {
            return ApplyNegation(EnemyCtrl.HealthCtrl.HealthSystem.CurrentHealth >= ConditionsDatas.Threshold);
        }
    }
}