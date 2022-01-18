namespace Templar.Unit.Enemy
{
    using RSLib.Extensions;
    using System.Linq;
    using UnityEngine;

    [DisallowMultipleComponent]
    public class EnemyController : UnitController, ICheckpointListener
    {
        private const float SLEEP_DIST = 25f;
        private const float SLEEP_UPDATE_RATE = 3f;
        private const int ACTIONS_TRACK_SIZE = 10;

        [Header("REFERENCES")]
        [SerializeField] private Player.PlayerController _playerCtrl = null;
        [SerializeField] private EnemyView _enemyView = null;

        [Header("BEHAVIOUR")]
        [SerializeField] private string _id = string.Empty;
        [SerializeField] private float _behaviourUpdateRate = 0.5f;

        [Header("DEBUG")]
        [SerializeField] private RSLib.Framework.DisabledString _currBehaviourName = new RSLib.Framework.DisabledString();
        [SerializeField] private RSLib.Framework.DisabledString _currActionName = new RSLib.Framework.DisabledString();

        private float _sleepUpdateTimer;
        private float _behaviourUpdateTimer;

        private Vector3 _initPos;

        private float _gravity;
        private float _jumpVel;
        private float _currVelY;

        private System.Collections.IEnumerator _hurtCoroutine;

        public RSLib.Framework.Collections.FixedSizedConcurrentQueue<Actions.IEnemyAction> LastActions { get; private set; }

        private EnemyBehaviour _currBehaviour;
        public EnemyBehaviour CurrBehaviour
        {
            get => _currBehaviour;
            private set
            {
                _currBehaviour = value;
                _currBehaviourName = new RSLib.Framework.DisabledString(_currBehaviour.BehaviourDatas.Name);
            }
        }

        private Actions.IEnemyAction _currAction;
        public Actions.IEnemyAction CurrAction
        {
            get => _currAction;
            private set
            {
                if (value != null)
                {
                    //Debug.LogError($"Enqueuing action {value.GetType().Name}.");
                    LastActions.Enqueue(value); // Enqueue here to avoid early return.
                }

                if (_currAction == value)
                    return;

                _currAction?.OnExit();
                _currAction = value;
                _currAction?.OnEnter();

                _currActionName = new RSLib.Framework.DisabledString($"{_currAction.GetType().Name} (index: {System.Array.IndexOf(CurrBehaviour.Actions, _currAction)})");
            }
        }

        public bool IsSleeping { get; private set; }

        public Datas.Unit.Enemy.EnemyDatas EnemyDatas { get; private set; }
        public EnemyBehaviour[] Behaviours { get; private set; }

        public Attack.EnemyAttackController AttackCtrl { get; private set; }
        public EnemyHealthController EnemyHealthCtrl => HealthCtrl as EnemyHealthController;

        public override UnitView UnitView => _enemyView;

        public bool IsPlayerAbove { get; private set; }

        public Player.PlayerController PlayerCtrl => _playerCtrl;
        public EnemyView EnemyView => _enemyView;
        
        public bool BeingHurt => _hurtCoroutine != null;

        void ICheckpointListener.OnCheckpointInteracted(Interaction.Checkpoint.CheckpointController checkpointCtrl)
        {
            ResetEnemy();
        }

        public void ForceUpdateCurrentBehaviour()
        {
            Debug.LogError("ForceUpdateCurrentBehaviour");
            _behaviourUpdateTimer = 0f;
            UpdateCurrentBehaviour();
        }

        public void ForceUpdateCurrentAction()
        {
            Debug.LogError("ForceUpdateCurrentAction");
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
            {
                if (!args.HitDatas.AttackDatas.ForceHurt)
                    return;

                AttackCtrl.CancelAttack();
                _currAction.OnExit();
            }

            if (args.HitDatas.ChargeCollision || CurrAction?.CantBeHurt != false)
            {
                if (args.HitDatas.AttackDatas.RecoilDatas != null)
                    _currentRecoil = new Templar.Physics.Recoil(args.HitDatas.AttackDir, args.HitDatas.AttackDatas.RecoilDatas, EnemyDatas.HurtCheckEdge);
    
                StartCoroutine(_hurtCoroutine = HurtCoroutine(args.HitDatas.ChargeCollision));
            }
        }

        private void OnUnitKilled(UnitHealthController.UnitKilledEventArgs args)
        {
            AttackCtrl.CancelAttack();

            if (EnemyDatas.OnKilledLoot != null)
                Manager.LootManager.SpawnLoot(EnemyDatas.OnKilledLoot, transform.position.AddY(0.2f));

            Manager.GameManager.CameraCtrl.GetShake(Templar.Camera.CameraShake.ID_MEDIUM).AddTrauma(EnemyDatas.OnKilledTrauma);
            Manager.FreezeFrameManager.FreezeFrame(0, 0.12f, 0f, true); // [TMP] Hardcoded values.

            EnemyView.PlayDeathAnimation(args.HitDatas.AttackDir);
            BoxCollider2D.enabled = false;

            StartDeadFadeCoroutine();
        }

        private void ResetEnemy()
        {
            EnemyHealthController enemyHealthCtrl = (EnemyHealthController)HealthCtrl;
            enemyHealthCtrl.ResetController();
            AttackCtrl.CancelAttack();

            EnemyView.PlayIdleAnimation();
            transform.position = _initPos;
            BoxCollider2D.enabled = true;
        }

        private void ComputeJumpPhysics()
        {
            // This code is a duplicate from PlayerJumpController.
            // We might want a UnitJumpController to handle this, and the variables,
            // but it's okay for now since those lines are constant maths computations and thus don't need to be changed.

            _gravity = -(2f * EnemyDatas.JumpHeight) / EnemyDatas.JumpApexDurSqr;
            _jumpVel = Mathf.Abs(_gravity) * EnemyDatas.JumpApexDur;
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

        private System.Collections.IEnumerator HurtCoroutine(bool chargeCollision)
        {
            Debug.LogError($"Hurt (chargeCollision:{chargeCollision})");

            if (chargeCollision)
                EnemyView.PlayChargeCollisionAnimation();
            else
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
                _playerCtrl = Manager.GameManager.PlayerCtrl;

                if (_playerCtrl == null)
                {
                    CProLogger.LogWarning(this, "Reference to PlayerController is missing and wasn't find in GameManager neither, trying to find it using FindObjectOfType.");
                    _playerCtrl = FindObjectOfType<Unit.Player.PlayerController>();

                    if (_playerCtrl == null)
                    {
                        CProLogger.LogError(this, "No PlayerController seems to be in the scene.");
                        return;
                    }
                }
            }

            EnemyView.Init(_id);
            StartCoroutine(UpdateSleepCoroutine());

            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command(
                "ResetEnemies",
                "Reset all enemies.",
                () => FindObjectsOfType<EnemyController>().ToList().ForEach(o => o.ResetEnemy())));
        }

        private void Start()
        {
            UnityEngine.Assertions.Assert.IsTrue(Database.EnemyDatabase.EnemiesDatas.ContainsKey(_id), $"Unknown enemy Id {_id}.");
            EnemyDatas = Database.EnemyDatabase.EnemiesDatas[_id];

            ComputeJumpPhysics();

            AttackCtrl = new Attack.EnemyAttackController(this);
            CollisionsCtrl = new Templar.Physics.CollisionsController(BoxCollider2D, CollisionMask);
            CollisionsCtrl.CollisionDetected += OnCollisionDetected;
            CollisionsCtrl.Ground(transform);

            EnemyHealthController enemyHealthCtrl = (EnemyHealthController)HealthCtrl;
            enemyHealthCtrl.Init(this, EnemyDatas.Health, EnemyDatas.Health);
            enemyHealthCtrl.UnitHealthChanged += OnUnitHealthChanged;
            enemyHealthCtrl.UnitKilled += OnUnitKilled;

            _initPos = transform.position;
            SetDirection(EnemyView.GetSpriteRendererFlipX() ? -1f : 1f);

            Behaviours = new EnemyBehaviour[EnemyDatas.Behaviours.Count];
            for (int i = 0; i < Behaviours.Length; ++i)
                Behaviours[i] = new EnemyBehaviour(this, EnemyDatas.Behaviours[i]);

            LastActions = new RSLib.Framework.Collections.FixedSizedConcurrentQueue<Actions.IEnemyAction>(ACTIONS_TRACK_SIZE);
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

            if (CollisionsCtrl.Below)
                _currVelY = 0f;

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

            if (CurrAction.ShouldApplyGravity)
            {
                _currVelY += _gravity * Time.deltaTime;
                Translate(0f, _currVelY);
            }

            CurrAction.Execute();

            ApplyCurrentRecoil();
            CollisionsCtrl.TriggerDetectedCollisionsEvents();
        }
    }
}