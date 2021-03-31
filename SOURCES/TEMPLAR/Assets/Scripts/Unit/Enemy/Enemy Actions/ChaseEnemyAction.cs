namespace Templar.Unit.Enemy.Actions
{
    public class ChaseEnemyAction : EnemyAction<Datas.Unit.Enemy.ChaseEnemyActionDatas>
    {
        public ChaseEnemyAction(EnemyController enemyCtrl, Datas.Unit.Enemy.ChaseEnemyActionDatas actionDatas)
            : base(enemyCtrl, actionDatas)
        {
        }

        public override bool CanExit()
        {
            return true;
        }

        public override void Execute()
        {
            EnemyCtrl.SetDirection(UnityEngine.Mathf.Sign(EnemyCtrl.Player.transform.position.x - EnemyCtrl.transform.position.x));

            EnemyCtrl.Translate(EnemyCtrl.CurrDir * EnemyCtrl.EnemyDatas.WalkSpeed, 0f);
            EnemyCtrl.EnemyView.FlipX(EnemyCtrl.CurrDir < 0f);
        }
    }
}