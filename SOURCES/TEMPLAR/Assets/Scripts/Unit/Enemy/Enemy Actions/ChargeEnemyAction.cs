namespace Templar.Unit.Enemy.Actions
{
    public class ChargeEnemyAction : EnemyAction<Datas.Unit.Enemy.ChargeEnemyActionDatas>
    {
        private float _anticipationTimer;
        private float _chargeTimer;
        private float _currSpeed;
        private bool _chargeAnimationSequenceStarted;
        private bool _sideCollision;

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
            if (EnemyCtrl.BeingHurt || EnemyCtrl.IsStunned || _sideCollision)
                return;

            if (!_chargeAnimationSequenceStarted)
            {
                EnemyCtrl.EnemyView.PlayChargeAnticipationAnimation();
                _chargeAnimationSequenceStarted = true;
            }

            if (_anticipationTimer < ActionDatas.AnticipationDuration)
            {
                _anticipationTimer += UnityEngine.Time.deltaTime;
                if (_anticipationTimer >= ActionDatas.AnticipationDuration)
                    OnChargeAnticipationOver();
                else
                    return; // Anticipation not done -> do not execute actual charge.
            }

            EnemyCtrl.SetDirection(DirectionX);
            EnemyCtrl.Translate(DirectionX * _currSpeed, 0f, checkEdge: true);
            EnemyCtrl.EnemyView.FlipX(EnemyCtrl.CurrDir < 0f);

            _chargeTimer += UnityEngine.Time.deltaTime;
            _currSpeed += ActionDatas.Acceleration * UnityEngine.Time.deltaTime;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            // Reset datas.
            _sideCollision = false;
            _anticipationTimer = 0f;
            _chargeTimer = 0f;
            _currSpeed = ActionDatas.InitSpeed;

            DirectionX = UnityEngine.Mathf.Sign(EnemyCtrl.PlayerCtrl.transform.position.x - EnemyCtrl.transform.position.x);
            EnemyCtrl.SetDirection(DirectionX);
            EnemyCtrl.EnemyView.FlipX(EnemyCtrl.CurrDir < 0f);

            _chargeAnimationSequenceStarted = false;
            EnemyCtrl.EnemyView.ResetChargeTriggers();
        }

        private void OnCollisionDetected(Physics.CollisionsController.CollisionInfos collisionInfos)
        {
            if (!IsCurrentAction || _sideCollision)
                return;

            _sideCollision = EnemyCtrl.CurrDir == 1f && collisionInfos.Origin == Physics.CollisionsController.CollisionOrigin.RIGHT
                          || EnemyCtrl.CurrDir == -1f && collisionInfos.Origin == Physics.CollisionsController.CollisionOrigin.LEFT;

            if (!_sideCollision)
                return;

            Datas.Unit.Enemy.ChargeActionCollisionDatas collisionDatas = null;
            if (collisionInfos.Hit.collider.TryGetComponent(out Player.PlayerController playerCtrl))
                collisionDatas = ActionDatas.PlayerCollisionDatas;
            else if (!collisionInfos.Hit.collider.TryGetComponent<EnemyController>(out _))
                collisionDatas = ActionDatas.WallCollisionDatas;

            if (collisionDatas == null)
                return;
            
            string attackId = collisionDatas.AttackId;
            UnityEngine.Assertions.Assert.IsTrue(
                string.IsNullOrEmpty(attackId) || Database.AttackDatabase.EnemyAttacksDatas.ContainsKey(attackId),
                $"Collision has been detected while charging but enemy attack Id {attackId} has not been found in {nameof(Database.AttackDatabase.EnemyAttacksDatas)}");

            if (!string.IsNullOrEmpty(attackId))
            {
                Attack.HitInfos hitInfos = new Attack.HitInfos(Database.AttackDatabase.EnemyAttacksDatas[attackId], EnemyCtrl.CurrDir, EnemyCtrl.transform, EnemyCtrl.AttackCtrl, collisionDatas);

                if (playerCtrl != null)
                    playerCtrl.HealthCtrl.OnHit(hitInfos);
                else
                    EnemyCtrl.HealthCtrl.OnHit(hitInfos);
            }

            void StunCallback()
            {
                EnemyCtrl.EnemyView.PlayIdleAnimation();
                EnemyCtrl.ForceUpdateCurrentBehaviour();
                EnemyCtrl.ForceUpdateCurrentAction();
            }
            
            if (collisionDatas.StunDur > 0f)
            {
                EnemyCtrl.Stun(
                    dur: collisionDatas.StunDur,
                    delay: collisionDatas.StunDelay,
                    conditionalDelay: () => !EnemyCtrl.BeingHurt,
                    callback: StunCallback);
            }
            else
            {
                StunCallback();
            }

            Manager.GameManager.CameraCtrl.ApplyShakeFromDatas(playerCtrl != null ? ActionDatas.PlayerCollisionDatas.Trauma : ActionDatas.WallCollisionDatas.Trauma);
        }
    
        private void OnChargeAnticipationOver()
        {
            EnemyCtrl.EnemyView.PlayChargeAnimation();
        }
    }
}