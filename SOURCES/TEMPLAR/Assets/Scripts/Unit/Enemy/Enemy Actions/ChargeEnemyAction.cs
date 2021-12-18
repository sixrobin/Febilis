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
                EnemyCtrl.Translate(DirectionX * ActionDatas.Speed, 0f, checkEdge: true);
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

            if (collisionInfos.Hit.collider.TryGetComponent(out Player.PlayerController playerCtrl))
            {
                string attackId = ActionDatas.PlayerCollisionDatas.AttackId;

                UnityEngine.Assertions.Assert.IsTrue(
                    string.IsNullOrEmpty(attackId) || Database.AttackDatabase.EnemyAttacksDatas.ContainsKey(attackId),
                    $"Collision on player has been detected while charging but enemy attack Id {attackId} has not been found in {nameof(Database.AttackDatabase.EnemyAttacksDatas)}");

                if (!string.IsNullOrEmpty(attackId))
                {
                    UnityEngine.Debug.Log($"{EnemyCtrl.EnemyDatas.Id} collided player, applying charge damage using attack Id {attackId}.");
                    playerCtrl.HealthCtrl.OnHit(new Attack.HitInfos(Database.AttackDatabase.EnemyAttacksDatas[attackId], EnemyCtrl.CurrDir, EnemyCtrl.transform)); ;
                }
    
                Manager.GameManager.CameraCtrl.ApplyShakeFromDatas(ActionDatas.PlayerCollisionDatas.Trauma);
                // [TODO] Update CurrentAction to some "Wait" action.
            }
            else if (!collisionInfos.Hit.collider.GetComponent<EnemyController>())
            {
                string attackId = ActionDatas.WallCollisionDatas.AttackId;

                UnityEngine.Assertions.Assert.IsTrue(
                    string.IsNullOrEmpty(attackId) || Database.AttackDatabase.EnemyAttacksDatas.ContainsKey(attackId),
                    $"Collision on player has been detected while charging but enemy attack Id {attackId} has not been found in {nameof(Database.AttackDatabase.EnemyAttacksDatas)}");
                
                if (!string.IsNullOrEmpty(attackId))
                {
                    UnityEngine.Debug.Log($"{EnemyCtrl.EnemyDatas.Id} collided wall, applying self stun damage using attack Id {attackId}.");
                    EnemyCtrl.HealthCtrl.OnHit(new Attack.HitInfos(Database.AttackDatabase.EnemyAttacksDatas[attackId], EnemyCtrl.CurrDir, EnemyCtrl.transform));
                }

                Manager.GameManager.CameraCtrl.ApplyShakeFromDatas(ActionDatas.WallCollisionDatas.Trauma);
                // [TODO] Update CurrentAction to some "Stunned" action.
            }
        }
    }
}