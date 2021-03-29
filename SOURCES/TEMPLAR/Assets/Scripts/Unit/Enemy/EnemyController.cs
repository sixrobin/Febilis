namespace Templar.Unit.Enemy
{
    using UnityEngine;

    [DisallowMultipleComponent]
    public class EnemyController : UnitController
    {
        [Header("REFERENCES")]
        [SerializeField] private Player.PlayerController _player = null;
        [SerializeField] private EnemyView _enemyView = null;

        [Header("BEHAVIOUR")]
        [SerializeField] private string _id = string.Empty;
        [SerializeField] private float _behaviourUpdateRate = 1f;

        [Header("DEBUG")]
        [SerializeField] private string _currBehaviourName = string.Empty;
        [SerializeField] private string _currActionName = string.Empty;
        [SerializeField] public Datas.Attack.SkeletonAttackDatas _tmpAttackDatas = null;

        private float _behaviourUpdateTimer;

        private System.Collections.IEnumerator _hurtCoroutine;

        private EnemyBehaviour _currBehaviour;
        public EnemyBehaviour CurrBehaviour
        {
            get => _currBehaviour;
            private set
            {
                _currBehaviour = value;
                _currBehaviourName = _currBehaviour.BehaviourDatas.Name;
            }
        }

        private Actions.IEnemyAction _currAction;
        public Actions.IEnemyAction CurrAction
        {
            get => _currAction;
            private set
            {
                if (_currAction == value)
                    return;

                _currAction?.Reset();
                _currAction = value;
                _currActionName = $"{_currAction.GetType().Name} (index: {System.Array.IndexOf(CurrBehaviour.Actions, _currAction)})";
            }
        }

        public Datas.Unit.Enemy.EnemyDatas EnemyDatas { get; private set; }
        public EnemyBehaviour[] Behaviours { get; private set; }
        public Attack.EnemyAttackController AttackCtrl { get; private set; }

        public bool IsPlayerAbove { get; private set; }

        public Player.PlayerController Player => _player;
        public EnemyView EnemyView => _enemyView;
        
        public bool BeingHurt => _hurtCoroutine != null;

        public void SetDirection(float dir)
        {
            CurrDir = dir;
        }

        public void ForceUpdateCurrentAction()
        {
            _behaviourUpdateTimer = 0f;
            UpdateCurrentAction();
        }

        private void OnCollisionDetected(Templar.Physics.CollisionsController.CollisionInfos collisionInfos)
        {
            // Avoid triggering event if there was a collision from the same origin at the previous frame.
            if (CollisionsCtrl.PreviousStates.GetCollisionState(collisionInfos.Origin))
                return;

            switch (collisionInfos.Origin)
            {
                case Templar.Physics.CollisionsController.CollisionOrigin.ABOVE:
                {
                    // [TODO] Pool/ref.
                    IsPlayerAbove = collisionInfos.Hit.collider.GetComponent<Player.PlayerController>();
                    break;
                }

                default:
                    break;
            }
        }

        private void OnUnitHealthChanged(UnitHealthController.UnitHealthChangedEventArgs args)
        {
            if (AttackCtrl.IsAttacking)
                return;

            _currentRecoil = new Templar.Physics.Recoil(args.HitDatas.AttackDir, force: 2.5f, dur: 0.15f); // [TMP] Hardcoded values.

            _hurtCoroutine = HurtCoroutine();
            StartCoroutine(_hurtCoroutine);
        }

        private void OnUnitKilled(UnitHealthController.UnitKilledEventArgs args)
        {
            FindObjectOfType<Templar.Camera.CameraController>().Shake.AddTrauma(EnemyDatas.OnKilledTrauma); // [TMP] GetComponent.
            Manager.FreezeFrameManager.FreezeFrame(0, 0.12f, 0f, true);

            _enemyView.PlayDeathAnimation(args.HitDatas.AttackDir);
            BoxCollider2D.enabled = false;
        }

        private void UpdateCurrentBehaviour()
        {
            for (int i = 0; i < Behaviours.Length; ++i)
            {
                if (Behaviours[i].CheckConditions())
                {
                    CurrBehaviour = Behaviours[i];
                    return;
                }
            }

            CProLogger.LogError(this, $"No behaviour has validated its conditions for enemy {_id}.", gameObject);
        }

        private void UpdateCurrentAction()
        {
            for (int i = 0; i < CurrBehaviour.Actions.Length; ++i)
            {
                if (CurrBehaviour.Actions[i].CheckConditions())
                {
                    CurrAction = CurrBehaviour.Actions[i];
                    return;
                }
            }

            CProLogger.LogError(this, $"No action has validated its conditions for enemy {_id} (current behaviour: {CurrBehaviour.BehaviourDatas.Name}).", gameObject);
        }

        private System.Collections.IEnumerator HurtCoroutine()
        {
            _enemyView.PlayHurtAnimation();
            yield return RSLib.Yield.SharedYields.WaitForSeconds(EnemyDatas.HurtDur);

            _hurtCoroutine = null;
            if (!IsDead && !AttackCtrl.IsAttacking)
                _enemyView.PlayIdleAnimation();
        }

        private void Awake()
        {
            AttackCtrl = new Attack.EnemyAttackController(this);
            CollisionsCtrl = new Templar.Physics.CollisionsController(BoxCollider2D, CollisionMask);
            CollisionsCtrl.CollisionDetected += OnCollisionDetected;
            CollisionsCtrl.Ground(transform);

            if (HealthCtrl is EnemyHealthController enemyHealthCtrl)
            {
                enemyHealthCtrl.Init();
                enemyHealthCtrl.UnitHealthChanged += OnUnitHealthChanged;
                enemyHealthCtrl.UnitKilled += OnUnitKilled;
            }

            CurrDir = _enemyView.GetSpriteRendererFlipX() ? -1f : 1f;

            UnityEngine.Assertions.Assert.IsTrue(Datas.Unit.Enemy.EnemyDatabase.EnemiesDatas.ContainsKey(_id), $"Unknown enemy Id {_id}.");
            EnemyDatas = Datas.Unit.Enemy.EnemyDatabase.EnemiesDatas[_id];

            Behaviours = new EnemyBehaviour[EnemyDatas.Behaviours.Count];
            for (int i = 0; i < Behaviours.Length; ++i)
                Behaviours[i] = new EnemyBehaviour(this, EnemyDatas.Behaviours[i]);

            UpdateCurrentBehaviour();
            UpdateCurrentAction();
        }

        protected override void Update()
        {
            base.Update();

            if (IsDead)
                return;

            IsPlayerAbove &= CollisionsCtrl.CurrentStates.GetCollisionState(Templar.Physics.CollisionsController.CollisionOrigin.ABOVE);

            CollisionsCtrl.BackupCurrentState();
            CollisionsCtrl.ComputeRaycastOrigins();
            CollisionsCtrl.CurrentStates.Reset();

            // Check above. There may be a better way to handle this, but this will do the job for now.
            Vector3 upCheck = new Vector3(0f, Templar.Physics.RaycastsController.SKIN_WIDTH * 2);
            CollisionsCtrl.ComputeVerticalCollisions(ref upCheck);

            _behaviourUpdateTimer += Time.deltaTime;
            if (_behaviourUpdateTimer > _behaviourUpdateRate)
            {
                _behaviourUpdateTimer = 0f;

                if (CurrAction.CanExit())
                {
                    UpdateCurrentBehaviour();
                    UpdateCurrentAction();
                }
            }

            CurrAction.Execute();

            ApplyCurrentRecoil();
            CollisionsCtrl.TriggerDetectedCollisionsEvents();
        }
    }
}