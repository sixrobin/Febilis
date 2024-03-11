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

        public override void OnEnter()
        {
            base.OnEnter();
            
            if (ActionDatas.FacePlayer)
                EnemyCtrl.EnemyView.SetWalkMultiplier(ActionDatas.WalkAnimationMultiplier);
        }

        public override void OnExit()
        {
            base.OnExit();
            
            if (ActionDatas.FacePlayer)
                EnemyCtrl.EnemyView.SetWalkMultiplier(1f);
        }

        public override void Execute()
        {
            EnemyCtrl.SetDirection(UnityEngine.Mathf.Sign(EnemyCtrl.transform.position.x - EnemyCtrl.PlayerCtrl.transform.position.x));

            EnemyCtrl.Translate(EnemyCtrl.CurrDir * EnemyCtrl.EnemyDatas.WalkSpeed, 0f, checkEdge: true);
            EnemyCtrl.EnemyView.FlipX(ActionDatas.FacePlayer ? -EnemyCtrl.CurrDir < 0f : EnemyCtrl.CurrDir < 0f);
            EnemyCtrl.EnemyView.PlayWalkAnimation(true);
        }
    }
}