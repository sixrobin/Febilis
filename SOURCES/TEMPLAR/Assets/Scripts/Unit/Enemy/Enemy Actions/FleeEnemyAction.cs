namespace Templar.Unit.Enemy.Actions
{
    public class FleeEnemyAction : EnemyAction<Datas.Unit.Enemy.FleeEnemyActionDatas>
    {
        public FleeEnemyAction(EnemyController enemyCtrl, Datas.Unit.Enemy.FleeEnemyActionDatas actionDatas)
            : base(enemyCtrl, actionDatas)
        {
        }

        public override bool CanExit()
        {
            return true;
        }

        public override void Execute()
        {
            CProLogger.Log(this, "Fleeing.");
        }
    }
}