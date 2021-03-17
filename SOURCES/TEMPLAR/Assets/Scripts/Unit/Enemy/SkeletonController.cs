namespace Templar.Unit.Enemy
{
    using RSLib.Extensions;
    using UnityEngine;

    public class SkeletonController : UnitController
    {
        [Header("SKELETON")]
        [SerializeField] private SkeletonView _skeletonView = null;
        [SerializeField] private Transform _target = null;
        [SerializeField] private Datas.SkeletonControllerDatas _ctrlDatas = null;

        [Header("DEBUG")]
        [SerializeField] private RSLib.DataColor _debugColor = null;

        private System.Collections.IEnumerator _hurtCoroutine;
        private System.Collections.IEnumerator _backAndForthPauseCoroutine;
        private System.Collections.IEnumerator _pauseBeforeAttackCoroutine;

        private float _initX;
        private float _backAndForthPauseDir;

        public SkeletonView SkeletonView => _skeletonView;
        public Datas.SkeletonControllerDatas CtrlDatas => _ctrlDatas;
        public Datas.SkeletonFightBehaviourDatas FightBehaviourDatas => CtrlDatas.FightBehaviourDatas;

        public Attack.SkeletonAttackController AttackCtrl { get; private set; }

        public bool IsWalking { get; private set; }

        public void OnTemplarAbove()
        {
            // [TMP] This method is a bit too "hardcoded". Maybe some ITemplarAboveReceiver interface ?
            // Also, we might want to keep a "PlayerAbove" boolean so that every attack triggered while true will be an above one.

            if (AttackCtrl.IsAttacking)
                return;

            if (_pauseBeforeAttackCoroutine != null)
            {
                StopCoroutine(_pauseBeforeAttackCoroutine);
                _pauseBeforeAttackCoroutine = null;
            }

            AttackCtrl.AttackAbove((attackOverDir) => _skeletonView.ResetAttackTrigger());
        }

        private void OnUnitHealthChanged(UnitHealthController.UnitHealthChangedEventArgs args)
        {
            _skeletonView.BlinkSpriteColor();

            if (AttackCtrl.IsAttacking)
                return;

            _currentRecoil = new Templar.Physics.Recoil(args.HitDatas.AttackDir, force: 2.5f, dur: 0.15f);

            _hurtCoroutine = HurtCoroutine();
            StartCoroutine(_hurtCoroutine);
        }

        private void OnUnitKilled(UnitHealthController.UnitKilledEventArgs args)
        {
            // [BUG] One of the skeleton appeared not to play the death animation. Or maybe another anim got triggered right after.

            AttackCtrl.CancelAttack();
            _skeletonView.ResetAttackTrigger();
            if (_pauseBeforeAttackCoroutine != null)
            {
                StopCoroutine(_pauseBeforeAttackCoroutine);
                _pauseBeforeAttackCoroutine = null;
            }

            FindObjectOfType<Templar.Camera.CameraController>().Shake.AddTrauma(0.4f); // [TMP] GetComponent + hard coded values.
            FreezeFrameController.FreezeFrame(0, 0.12f, 0f, true);

            _skeletonView.PlayDeathAnimation(args.HitDatas.AttackDir);
            BoxCollider2D.enabled = false;
        }

        private bool IsTargetValid()
        {
            return !_target.GetComponent<Player.PlayerController>().IsDead; // [TMP] GetComponent!
        }

        private bool IsTargetDetected()
        {
            return (_target.position - transform.position).sqrMagnitude <= FightBehaviourDatas.TargetDetectionRangeSqr;
        }

        private bool IsTargetInAttackRange()
        {
            return (_target.position - transform.position).sqrMagnitude <= FightBehaviourDatas.TargetAttackRangeSqr;
        }

        private void MoveBackAndForth()
        {
            if (_backAndForthPauseCoroutine != null)
                return;

            bool reachedLimit = CurrDir == 1f
                ? _initX + _ctrlDatas.HalfBackAndForthRange < transform.position.x
                : _initX - _ctrlDatas.HalfBackAndForthRange > transform.position.x;

            if (reachedLimit)
            {
                _backAndForthPauseDir = CurrDir;
                CurrDir *= -1;
                _backAndForthPauseCoroutine = BackAndForthPauseCoroutine();
                StartCoroutine(_backAndForthPauseCoroutine);
                return;
            }

            Translate(new Vector3(CurrDir * _ctrlDatas.WalkSpeed, 0f));
            IsWalking = true;
        }

        private System.Collections.IEnumerator HurtCoroutine()
        {
            IsWalking = false;

            _skeletonView.PlayHurtAnimation();
            yield return RSLib.Yield.SharedYields.WaitForSeconds(CtrlDatas.HurtDur);

            _hurtCoroutine = null;
            if (!AttackCtrl.IsAttacking)
                _skeletonView.PlayIdleAnimation();
        }

        private System.Collections.IEnumerator BackAndForthPauseCoroutine()
        {
            IsWalking = false;

            yield return RSLib.Yield.SharedYields.WaitForSeconds(_ctrlDatas.BackAndForthPause);
            yield return new WaitUntil(() => !AttackCtrl.IsAttacking);
            _backAndForthPauseCoroutine = null;
        }

        private System.Collections.IEnumerator PauseBeforeAttackCoroutine()
        {
            IsWalking = false;

            yield return RSLib.Yield.SharedYields.WaitForSeconds(FightBehaviourDatas.BeforeAttackDur);
            yield return new WaitUntil(() => _hurtCoroutine == null);
            _pauseBeforeAttackCoroutine = null;

            // Callback on attack and is a security to avoid attack animator parameter being offset from view.
            if (IsTargetDetected())
                AttackCtrl.Attack((attackOverDir) => _skeletonView.ResetAttackTrigger());
        }

        private void Awake()
        {
            AttackCtrl = new Attack.SkeletonAttackController(this);
            CollisionsCtrl = new Templar.Physics.CollisionsController(BoxCollider2D, CollisionMask);

            if (HealthCtrl is EnemyHealthController enemyHealthCtrl)
            {
                enemyHealthCtrl.Init();
                enemyHealthCtrl.UnitHealthChanged += OnUnitHealthChanged;
                enemyHealthCtrl.UnitKilled += OnUnitKilled;
            }

            _initX = transform.position.x;
            CurrDir = _skeletonView.GetSpriteRendererFlipX() ? -1f : 1f;

            CollisionsCtrl.Ground(transform);

            _skeletonView.SkeletonController = this;
        }

        private void Update()
        {
            // [NOTES]
            // Pour connaître la direction, utiliser une méthode ComputeDir qui agit selon la
            // situation actuelle (pause, idle, player à portée, en train d'attaquer, etc.).
            // Si FSM, alors chaque état devrait avoir une telle méthode pour pouvoir
            // faire un truc du style _fsm.CurrentState.GetDir().

            if (HealthCtrl.HealthSystem.IsDead)
                return;

            if (_currentRecoil != null)
            {
                Translate(new Vector3(_currentRecoil.Dir * _currentRecoil.Force, 0f));
                _currentRecoil.Update();
                if (_currentRecoil.IsComplete)
                    _currentRecoil = null;
            }

            if (!AttackCtrl.IsAttacking)
            {
                if (IsTargetDetected() && IsTargetValid())
                {
                    if (_backAndForthPauseCoroutine != null)
                    {
                        StopCoroutine(_backAndForthPauseCoroutine);
                        _backAndForthPauseCoroutine = null;
                    }

                    CurrDir = Mathf.Sign(_target.position.x - transform.position.x);

                    if (IsTargetInAttackRange())
                    {
                        if (_pauseBeforeAttackCoroutine == null)
                        {
                            _pauseBeforeAttackCoroutine = PauseBeforeAttackCoroutine();
                            StartCoroutine(_pauseBeforeAttackCoroutine);
                        }
                    }
                    else
                    {
                        if (_currentRecoil == null)
                        {
                            Translate(new Vector3(CurrDir * _ctrlDatas.WalkSpeed, 0f));
                            IsWalking = true;
                        }
                    }
                }
                else
                {
                    if (_pauseBeforeAttackCoroutine == null)
                        MoveBackAndForth();
                }
            }

            _skeletonView.UpdateView(_backAndForthPauseCoroutine != null ? _backAndForthPauseDir < 0f : CurrDir < 0f);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = _debugColor != null ? _debugColor.Color : Color.red;

            Gizmos.DrawWireSphere(transform.position, FightBehaviourDatas.TargetDetectionRange);
            Gizmos.DrawWireSphere(transform.position, FightBehaviourDatas.TargetAttackRange);

#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isPlaying)
            {
                Gizmos.DrawWireSphere(new Vector3(_initX - _ctrlDatas.HalfBackAndForthRange, transform.position.y + 0.5f), 0.05f);
                Gizmos.DrawWireSphere(new Vector3(_initX + _ctrlDatas.HalfBackAndForthRange, transform.position.y + 0.5f), 0.05f);
                Gizmos.DrawLine(new Vector3(_initX - _ctrlDatas.HalfBackAndForthRange, transform.position.y + 0.5f),
                    new Vector3(_initX + _ctrlDatas.HalfBackAndForthRange, transform.position.y + 0.5f));
            }
            else
            {
#endif
                Gizmos.DrawWireSphere(transform.position.AddX(-_ctrlDatas.HalfBackAndForthRange).AddY(0.5f), 0.05f);
                Gizmos.DrawWireSphere(transform.position.AddX(_ctrlDatas.HalfBackAndForthRange).AddY(0.5f), 0.05f);
                Gizmos.DrawLine(transform.position.AddX(-_ctrlDatas.HalfBackAndForthRange).AddY(0.5f),
                transform.position.AddX(_ctrlDatas.HalfBackAndForthRange).AddY(0.5f));
#if UNITY_EDITOR
            }
#endif
        }
    }
}