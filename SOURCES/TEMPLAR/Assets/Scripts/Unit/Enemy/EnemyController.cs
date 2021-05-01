namespace Templar.Unit.Enemy
{
    using UnityEngine;

    [DisallowMultipleComponent]
    public class EnemyController : UnitController, ICheckpointListener
    {
        private const float SLEEP_DIST = 25f;
        private const float SLEEP_UPDATE_RATE = 3f;

        [Header("REFERENCES")]
        [SerializeField] private Player.PlayerController _playerCtrl = null;
        [SerializeField] private EnemyView _enemyView = null;

        [Header("BEHAVIOUR")]
        [SerializeField] private string _id = string.Empty;
        [SerializeField] private float _behaviourUpdateRate = 0.5f;

        [Header("DEBUG")]
        [SerializeField] private string _currBehaviourName = string.Empty;
        [SerializeField] private string _currActionName = string.Empty;
        [SerializeField] public string _tmpAttackDatasId = null;

        private float _sleepUpdateTimer;
        private float _behaviourUpdateTimer;

        private Vector3 _initPos;

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

        public bool IsSleeping { get; private set; }

        public Datas.Unit.Enemy.EnemyDatas EnemyDatas { get; private set; }
        public EnemyBehaviour[] Behaviours { get; private set; }
        public Attack.EnemyAttackController AttackCtrl { get; private set; }

        public override UnitView UnitView => _enemyView;

        public bool IsPlayerAbove { get; private set; }

        public Player.PlayerController PlayerCtrl => _playerCtrl;
        public EnemyView EnemyView => _enemyView;
        
        public bool BeingHurt => _hurtCoroutine != null;

        public void OnCheckpointInteracted(Interaction.Checkpoint.CheckpointController checkpointCtrl)
        {
            EnemyHealthController enemyHealthCtrl = (EnemyHealthController)HealthCtrl;
            enemyHealthCtrl.ResetController();
            AttackCtrl.CancelAttack();

            EnemyView.PlayIdleAnimation();
            transform.position = _initPos;
            BoxCollider2D.enabled = true;
        }

        public void SetDirection(float dir)
        {
            CurrDir = dir;
        }

        public void ForceUpdateCurrentAction()
        {
            _behaviourUpdateTimer = 0f;
            UpdateCurrentAction();
        }

        protected override void OnCollisionDetected(Templar.Physics.CollisionsController.CollisionInfos collisionInfos)
        {
            base.OnCollisionDetected(collisionInfos);

            if (IsDead)
                return;

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
            if (!args.IsLoss)
                return;

            EnemyView.BlinkSpriteColor(delay: 0.025f);

            if (AttackCtrl.IsAttacking)
                return;

            _currentRecoil = new Templar.Physics.Recoil(args.HitDatas.AttackDir, force: 2.5f, dur: 0.15f, true); // [TMP] Hardcoded values.

            _hurtCoroutine = HurtCoroutine();
            StartCoroutine(_hurtCoroutine);
        }

        private void OnUnitKilled(UnitHealthController.UnitKilledEventArgs args)
        {
            AttackCtrl.CancelAttack();

            FindObjectOfType<Templar.Camera.CameraController>().GetShake(Templar.Camera.CameraShake.ID_MEDIUM).AddTrauma(EnemyDatas.OnKilledTrauma); // [TMP] GetComponent.
            Manager.FreezeFrameManager.FreezeFrame(0, 0.12f, 0f, true); // [TMP] Hardcoded values.

            EnemyView.PlayDeathAnimation(args.HitDatas.AttackDir);
            BoxCollider2D.enabled = false;

            StartDeadFadeCoroutine();
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

        private System.Collections.IEnumerator UpdateSleepCoroutine()
        {
            while (true)
            {
                yield return RSLib.Yield.SharedYields.WaitForSeconds(SLEEP_UPDATE_RATE);
                IsSleeping = (transform.position - PlayerCtrl.transform.position).sqrMagnitude > SLEEP_DIST * SLEEP_DIST;
            }
        }

        private System.Collections.IEnumerator HurtCoroutine()
        {
            EnemyView.PlayHurtAnimation();
            yield return RSLib.Yield.SharedYields.WaitForSeconds(EnemyDatas.HurtDur);

            _hurtCoroutine = null;
            if (!IsDead && !AttackCtrl.IsAttacking)
                EnemyView.PlayIdleAnimation();
        }

        private void Awake()
        {
            if (_playerCtrl == null)
            {
                CProLogger.LogWarning(this, "Reference to PlayerController is missing, trying to find it using FindObjectOfType.");
                _playerCtrl = FindObjectOfType<Unit.Player.PlayerController>();

                if (_playerCtrl == null)
                {
                    CProLogger.LogError(this, "No PlayerController seems to be in the scene.");
                    return;
                }
            }

            EnemyView.SetEnemyId(_id);
            StartCoroutine(UpdateSleepCoroutine());
        }

        private void Start()
        {
            UnityEngine.Assertions.Assert.IsTrue(Datas.Unit.Enemy.EnemyDatabase.EnemiesDatas.ContainsKey(_id), $"Unknown enemy Id {_id}.");
            EnemyDatas = Datas.Unit.Enemy.EnemyDatabase.EnemiesDatas[_id];

            AttackCtrl = new Attack.EnemyAttackController(this);
            CollisionsCtrl = new Templar.Physics.CollisionsController(BoxCollider2D, CollisionMask);
            CollisionsCtrl.CollisionDetected += OnCollisionDetected;
            CollisionsCtrl.Ground(transform);

            EnemyHealthController enemyHealthCtrl = (EnemyHealthController)HealthCtrl;
            enemyHealthCtrl.Init(EnemyDatas.Health);
            enemyHealthCtrl.UnitHealthChanged += OnUnitHealthChanged;
            enemyHealthCtrl.UnitKilled += OnUnitKilled;

            _initPos = transform.position;
            CurrDir = EnemyView.GetSpriteRendererFlipX() ? -1f : 1f;

            Behaviours = new EnemyBehaviour[EnemyDatas.Behaviours.Count];
            for (int i = 0; i < Behaviours.Length; ++i)
                Behaviours[i] = new EnemyBehaviour(this, EnemyDatas.Behaviours[i]);

            UpdateCurrentBehaviour();
            UpdateCurrentAction();
        }

        protected override void Update()
        {
            if (IsSleeping)
                return;

            base.Update();

            if (IsDead)
                return;

            IsPlayerAbove &= CollisionsCtrl.CurrentStates.GetCollisionState(Templar.Physics.CollisionsController.CollisionOrigin.ABOVE);

            CollisionsCtrl.BackupCurrentState();
            CollisionsCtrl.ComputeRaycastOrigins();
            CollisionsCtrl.CurrentStates.Reset();

            // Check above. There may be a better way to handle this, but this will do the job for now.
            Vector3 upCheck = new Vector3(0f, Templar.Physics.RaycastsController.SKIN_WIDTH * 2);
            CollisionsCtrl.ComputeVerticalCollisions(ref upCheck, false);

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