namespace Templar.Unit.Enemy.Conditions
{
    public class RandomChanceEnemyCondition : EnemyCondition<Datas.Unit.Enemy.RandomChanceConditionDatas>
    {
        public RandomChanceEnemyCondition(EnemyController enemyCtrl, Datas.Unit.Enemy.RandomChanceConditionDatas conditionDatas)
            : base(enemyCtrl, conditionDatas)
        {
        }

        public override bool Check()
        {
            return ApplyNegation(UnityEngine.Random.value < ConditionsDatas.Chance);
        }
    }
}