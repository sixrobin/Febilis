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
            EnemyCtrl.SetDirection(UnityEngine.Mathf.Sign(EnemyCtrl.transform.position.x - EnemyCtrl.PlayerCtrl.transform.position.x));

            EnemyCtrl.Translate(EnemyCtrl.CurrDir * EnemyCtrl.EnemyDatas.WalkSpeed, 0f);
            EnemyCtrl.EnemyView.FlipX(EnemyCtrl.CurrDir < 0f);
        }
    }
}