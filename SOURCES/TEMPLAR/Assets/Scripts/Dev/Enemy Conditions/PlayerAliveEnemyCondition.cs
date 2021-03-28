namespace Templar.Dev
{
    public class PlayerAliveEnemyCondition : EnemyCondition<PlayerAliveEnemyConditionDatas>
    {
        public PlayerAliveEnemyCondition(GenericEnemyController enemyCtrl, PlayerAliveEnemyConditionDatas conditionDatas)
            : base(enemyCtrl, conditionDatas)
        {
        }

        public override bool Check()
        {
            return ApplyNegation(!EnemyCtrl.Player.IsDead);
        }
    }
}