namespace Templar.Unit.Enemy.Actions
{
    public class ChargeEnemyAction : EnemyAction<Datas.Unit.Enemy.ChargeEnemyActionDatas>
    {
        private float _chargeTimer = 0f;
        private float _currSpeed = 0f;
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

            if (EnemyCtrl.IsStunned)
                return;

            if (!EnemyCtrl.BeingHurt && !_sideCollisionDetected)
            {
                EnemyCtrl.Translate(DirectionX * _currSpeed, 0f, checkEdge: true);
                EnemyCtrl.EnemyView.FlipX(EnemyCtrl.CurrDir < 0f);
                EnemyCtrl.EnemyView.PlayWalkAnimation(true);
            }

            _chargeTimer += UnityEngine.Time.deltaTime;
            _currSpeed += ActionDatas.Acceleration * UnityEngine.Time.deltaTime;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            _chargeTimer = 0f;
            _currSpeed = ActionDatas.InitSpeed;

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

            Datas.Unit.Enemy.ChargeActionCollisionDatas collisionDatas = null;
            if (collisionInfos.Hit.collider.TryGetComponent(out Player.PlayerController playerCtrl))
                collisionDatas = ActionDatas.PlayerCollisionDatas;
            else if (!collisionInfos.Hit.collider.GetComponent<EnemyController>())
                collisionDatas = ActionDatas.WallCollisionDatas;

            if (collisionDatas == null)
                return;
            
            string attackId = collisionDatas.AttackId;
            UnityEngine.Assertions.Assert.IsTrue(
                string.IsNullOrEmpty(attackId) || Database.AttackDatabase.EnemyAttacksDatas.ContainsKey(attackId),
                $"Collision has been detected while charging but enemy attack Id {attackId} has not been found in {nameof(Database.AttackDatabase.EnemyAttacksDatas)}");

            if (!string.IsNullOrEmpty(attackId))
            {
                if (playerCtrl != null)
                {
                    UnityEngine.Debug.Log($"{EnemyCtrl.EnemyDatas.Id} collided player, applying charge damage using attack Id {attackId}.");
                    playerCtrl.HealthCtrl.OnHit(new Attack.HitInfos(Database.AttackDatabase.EnemyAttacksDatas[attackId], EnemyCtrl.CurrDir, EnemyCtrl.transform)); ;
                }
                else
                {
                    UnityEngine.Debug.Log($"{EnemyCtrl.EnemyDatas.Id} collided wall, applying self stun damage using attack Id {attackId}.");
                    EnemyCtrl.HealthCtrl.OnHit(new Attack.HitInfos(Database.AttackDatabase.EnemyAttacksDatas[attackId], EnemyCtrl.CurrDir, EnemyCtrl.transform));
                }
            }

            if (collisionDatas.StunDur > 0f)
                EnemyCtrl.Stun(collisionDatas.StunDur);

            Manager.GameManager.CameraCtrl.ApplyShakeFromDatas(ActionDatas.PlayerCollisionDatas.Trauma);
        }
    }
}