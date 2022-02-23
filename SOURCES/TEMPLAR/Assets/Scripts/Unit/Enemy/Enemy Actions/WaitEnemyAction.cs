namespace Templar.Unit.Enemy.Actions
{
    public class WaitEnemyAction : EnemyAction<Datas.Unit.Enemy.WaitEnemyActionDatas>
    {
        public WaitEnemyAction(EnemyController enemyCtrl, Datas.Unit.Enemy.WaitEnemyActionDatas actionDatas)
            : base(enemyCtrl, actionDatas)
        {
        }

        public override bool CanExit()
        {
            return true;
        }

        public override void Execute()
        {
            EnemyCtrl.EnemyView.FlipX(ActionDatas.FacePlayer ? -EnemyCtrl.CurrDir < 0f : EnemyCtrl.CurrDir < 0f);
        }
    }
}