namespace Templar.Dev
{
    public class PlayerAboveEnemyCondition : EnemyCondition<PlayerAboveEnemyConditionDatas>
    {
        public PlayerAboveEnemyCondition(GenericEnemyController enemyCtrl, PlayerAboveEnemyConditionDatas conditionDatas)
            : base(enemyCtrl, conditionDatas)
        {
        }

        public override bool Check()
        {
            return ApplyNegation(EnemyCtrl.IsPlayerAbove);
        }
    }
}