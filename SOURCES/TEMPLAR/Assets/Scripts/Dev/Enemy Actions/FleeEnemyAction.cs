namespace Templar.Dev
{
    public class FleeEnemyAction : EnemyAction<FleeEnemyActionDatas>
    {
        public FleeEnemyAction(GenericEnemyController enemyCtrl, FleeEnemyActionDatas actionDatas)
            : base(enemyCtrl, actionDatas)
        {
        }

        public override void Execute()
        {
            CProLogger.Log(this, "Fleeing.");
        }
    }
}