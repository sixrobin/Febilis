namespace Templar.Unit.Enemy.Conditions
{
    public class PlayerAliveEnemyCondition : EnemyCondition<Datas.Unit.Enemy.PlayerAliveEnemyConditionDatas>
    {
        public PlayerAliveEnemyCondition(EnemyController enemyCtrl, Datas.Unit.Enemy.PlayerAliveEnemyConditionDatas conditionDatas)
            : base(enemyCtrl, conditionDatas)
        {
        }

        public override bool Check()
        {
            return ApplyNegation(!EnemyCtrl.Player.IsDead);
        }
    }
}