namespace Templar.Unit.Enemy.Actions
{
    public class ChargeEnemyAction : EnemyAction<Datas.Unit.Enemy.ChargeEnemyActionDatas>
    {
        private float _chargeTimer = 0f;
        private bool _sideCollisionDetected;

        public ChargeEnemyAction(EnemyController enemyCtrl, Datas.Unit.Enemy.ChargeEnemyActionDatas actionDatas)
            : base(enemyCtrl, actionDatas)
        {
        }

        public float DirectionX { get; private set; }

        public override void Init()
        {
            base.Init();
            EnemyCtrl.CollisionsCtrl.CollisionDetected += OnCollisionDetected;
        }

        public override bool CanExit()
        {
            return _chargeTimer > ActionDatas.MaxDuration;
        }

        public override void Execute()
        {
            EnemyCtrl.SetDirection(DirectionX);

            if (!EnemyCtrl.BeingHurt || _sideCollisionDetected)
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

        private void OnCollisionDetected(Physics.CollisionsController.CollisionInfos collisionInfos)
        {
            if (EnemyCtrl.CurrAction != this)
                return;

            _sideCollisionDetected = EnemyCtrl.CurrDir == 1f && collisionInfos.Origin == Physics.CollisionsController.CollisionOrigin.RIGHT
                              || EnemyCtrl.CurrDir == -1f && collisionInfos.Origin == Physics.CollisionsController.CollisionOrigin.LEFT;

            if (!_sideCollisionDetected)
                return;

            if (collisionInfos.Hit.collider.GetComponent<Player.PlayerController>())
            {
                UnityEngine.Debug.LogError("Collided player, applying charge damage.");
                Manager.GameManager.CameraCtrl.ApplyShakeFromDatas(ActionDatas.PlayerCollisionTrauma);
            }
            else if (!collisionInfos.Hit.collider.GetComponent<EnemyController>())
            {
                UnityEngine.Debug.LogError("Collided wall, stunning enemy.");
                Manager.GameManager.CameraCtrl.ApplyShakeFromDatas(ActionDatas.WallCollisionTrauma);
            }
        }
    }
}