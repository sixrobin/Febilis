namespace Templar.Dev
{
    using UnityEngine;

    [DisallowMultipleComponent]
    public class GenericEnemyController : Unit.UnitController
    {
        [Header("REFERENCES")]
        [SerializeField] private Unit.Player.PlayerController _player = null;
        [SerializeField] private GenericEnemyView _enemyView = null;

        [Header("BEHAVIOUR")]
        [SerializeField] private string _id = string.Empty;
        [SerializeField] private float _behaviourUpdateRate = 1f;

        [Header("DEBUG")]
        [SerializeField] private string _currBehaviourName = string.Empty;
        [SerializeField] private string _currActionName = string.Empty;
        [SerializeField] public Templar.Attack.Datas.SkeletonAttackDatas _tmpAttackDatas = null;

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

        private IEnemyAction _currAction;
        public IEnemyAction CurrAction
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

        public EnemyDatas EnemyDatas { get; private set; }
        public EnemyBehaviour[] Behaviours { get; private set; }
        public GenericEnemyAttackController AttackCtrl { get; private set; }

        public bool IsPlayerAbove { get; private set; }

        public Unit.Player.PlayerController Player => _player;
        public GenericEnemyView EnemyView => _enemyView;
        
        public bool BeingHurt => _hurtCoroutine != null;

        public void SetDirection(float dir)
        {
            CurrDir = dir;
        }

        private void OnUnitHealthChanged(Unit.UnitHealthController.UnitHealthChangedEventArgs args)
        {
            _currentRecoil = new Templar.Physics.Recoil(args.HitDatas.AttackDir, force: 2.5f, dur: 0.15f); // [TMP] Hardcoded values.
            _hurtCoroutine = HurtCoroutine();
            StartCoroutine(_hurtCoroutine);
        }

        private void OnUnitKilled(Unit.UnitHealthController.UnitKilledEventArgs args)
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
            //if (!AttackCtrl.IsAttacking)
                _enemyView.PlayIdleAnimation();
        }

        private void Awake()
        {
            AttackCtrl = new GenericEnemyAttackController(this);
            CollisionsCtrl = new Templar.Physics.CollisionsController(BoxCollider2D, CollisionMask);
            CollisionsCtrl.Ground(transform);

            if (HealthCtrl is Unit.Enemy.EnemyHealthController enemyHealthCtrl)
            {
                enemyHealthCtrl.Init();
                enemyHealthCtrl.UnitHealthChanged += OnUnitHealthChanged;
                enemyHealthCtrl.UnitKilled += OnUnitKilled;
            }

            CurrDir = _enemyView.GetSpriteRendererFlipX() ? -1f : 1f;

            UnityEngine.Assertions.Assert.IsTrue(EnemyDatabase.EnemiesDatas.ContainsKey(_id), $"Unknown enemy Id {_id}.");
            EnemyDatas = EnemyDatabase.EnemiesDatas[_id];

            Behaviours = new EnemyBehaviour[EnemyDatas.Behaviours.Count];
            for (int i = 0; i < Behaviours.Length; ++i)
                Behaviours[i] = new EnemyBehaviour(this, EnemyDatas.Behaviours[i]);

            UpdateCurrentBehaviour();
            UpdateCurrentAction();
        }

        private void Update()
        {
            if (IsDead)
                return;

            _behaviourUpdateTimer += Time.deltaTime;
            if (_behaviourUpdateTimer > _behaviourUpdateRate)
            {
                _behaviourUpdateTimer = 0f;
                UpdateCurrentBehaviour();
                UpdateCurrentAction();
            }

            CurrAction.Execute();

            ApplyCurrentRecoil();
        }
    }
}