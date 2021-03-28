namespace Templar.Dev
{
    public class RandomChanceEnemyCondition : EnemyCondition<RandomChanceConditionDatas>
    {
        public RandomChanceEnemyCondition(GenericEnemyController enemyCtrl, RandomChanceConditionDatas conditionDatas)
            : base(enemyCtrl, conditionDatas)
        {
        }

        public override bool Check()
        {
            return ApplyNegation(UnityEngine.Random.value < ConditionsDatas.Chance);
        }
    }
}