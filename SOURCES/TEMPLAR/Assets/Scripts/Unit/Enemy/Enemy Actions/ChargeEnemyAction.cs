namespace Templar.Unit.Enemy.Actions
{
    public class ChargeEnemyAction : EnemyAction<Datas.Unit.Enemy.ChargeEnemyActionDatas>
    {
        private float _chargeTimer = 0f;

        public ChargeEnemyAction(EnemyController enemyCtrl, Datas.Unit.Enemy.ChargeEnemyActionDatas actionDatas)
            : base(enemyCtrl, actionDatas)
        {
        }

        public float DirectionX { get; private set; }

        public override bool CanExit()
        {
            return _chargeTimer > ActionDatas.MaxDuration;
        }

        public override void Execute()
        {
            EnemyCtrl.SetDirection(DirectionX);

            if (!EnemyCtrl.BeingHurt)
            {
                EnemyCtrl.Translate(DirectionX * EnemyCtrl.EnemyDatas.RunSpeed, 0f, checkEdge: true);
                EnemyCtrl.EnemyView.FlipX(EnemyCtrl.CurrDir < 0f);
                EnemyCtrl.EnemyView.PlayWalkAnimation(true);
            }

            _chargeTimer += UnityEngine.Time.deltaTime;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            _chargeTimer = 0f;
            DirectionX = UnityEngine.Mathf.Sign(EnemyCtrl.PlayerCtrl.transform.position.x - EnemyCtrl.transform.position.x);
        }

        //private void OnCollisionDetected(Physics.CollisionsController.CollisionInfos collisionInfos)
        //{
        //    if (EnemyCtrl.CurrAction != this)
        //        return;
        //}
    }
}