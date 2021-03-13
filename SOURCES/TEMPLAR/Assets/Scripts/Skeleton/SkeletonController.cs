using RSLib.Extensions;
using UnityEngine;

public class SkeletonController : MonoBehaviour, IHittable
{
    [SerializeField] private SkeletonView _skeletonView = null;
    [SerializeField] private BoxCollider2D _boxCollider2D = null;
    [SerializeField] private Transform _target = null;
    [SerializeField] private SkeletonControllerDatas _controllerDatas = null;
    [SerializeField] private AttackHitboxesContainer _attackHitboxesContainer = null;
    [SerializeField] private LayerMask _collisionMask = 0;

    [Header("MOVEMENT")]
    [SerializeField] private float _moveSpeed = 1.3f;
    [SerializeField] private float _backAndForthRange = 3f;
    [SerializeField] private float _backAndForthPause = 0.5f;

    [Header("FIGHT")]
    [SerializeField, Min(0f)] private float _targetDetectionRange = 4f;
    [SerializeField, Min(0f)] private float _targetAttackRange = 1.5f;
    [SerializeField, Min(0f)] private float _attackAnticipationDur = 0.5f;
    [SerializeField, Min(0f)] private float _attackDur = 0.5f;
    [SerializeField, Min(0f)] private float _beforeAttackDur = 0.3f;
    [SerializeField, Min(0f)] private float _hurtDur = 0.25f;

    [Header("DEBUG")]
    [SerializeField] private RSLib.DataColor _debugColor = null;

    private System.Collections.IEnumerator _hurtCoroutine;
    private System.Collections.IEnumerator _backAndForthPauseCoroutine;
    private System.Collections.IEnumerator _pauseBeforeAttackCoroutine;

    private Recoil _currentRecoil;
    private float _initX;
    private float _backAndForthPauseDir;

    public HitLayer HitLayer => HitLayer.Enemy;

    public SkeletonView SkeletonView => _skeletonView;
    public SkeletonControllerDatas ControllerDatas => _controllerDatas;
    public AttackHitboxesContainer AttackHitboxesContainer => _attackHitboxesContainer;

    public SkeletonAttackController AttackCtrl { get; private set; }
    public CollisionsController CollisionsCtrl { get; private set; }

    public float CurrDir { get; private set; }

    public float HalfBackAndForthRange => _backAndForthRange * 0.5f;

    public void OnHit(AttackDatas attackDatas, float dir)
    {
        if (AttackCtrl.IsAttacking)
            return;

        _currentRecoil = new Recoil(dir, force: 2.5f, dur: 0.15f);

        _hurtCoroutine = HurtCoroutine();
        StartCoroutine(_hurtCoroutine);
    }

    private bool IsTargetValid()
    {
        return !_target.GetComponent<TemplarController>().IsDead; // [TMP] GetComponent!
    }

    private bool IsTargetDetected()
    {
        return (_target.position - transform.position).sqrMagnitude <= _targetDetectionRange * _targetDetectionRange;
    }

    private bool IsTargetInAttackRange()
    {
        return (_target.position - transform.position).sqrMagnitude <= _targetAttackRange * _targetAttackRange;
    }

    private void MoveBackAndForth()
    {
        if (_backAndForthPauseCoroutine != null)
            return;

        bool reachedLimit = CurrDir == 1f
            ? _initX + HalfBackAndForthRange < transform.position.x
            : _initX - HalfBackAndForthRange > transform.position.x;

        if (reachedLimit)
        {
            _backAndForthPauseDir = CurrDir;
            CurrDir *= -1;
            _backAndForthPauseCoroutine = BackAndForthPauseCoroutine();
            StartCoroutine(_backAndForthPauseCoroutine);
        }

        Translate(new Vector3(CurrDir * _moveSpeed, 0f));
    }

    private void Translate(Vector3 vel)
    {
        vel = CollisionsCtrl.ComputeCollisions(vel * Time.deltaTime);
        transform.Translate(vel);
    }

    private System.Collections.IEnumerator HurtCoroutine()
    {
        _skeletonView.PlayHurtAnimation();
        yield return RSLib.Yield.SharedYields.WaitForSeconds(_hurtDur);

        _hurtCoroutine = null;
        if (!AttackCtrl.IsAttacking)
            _skeletonView.PlayIdleAnimation();
    }

    private System.Collections.IEnumerator BackAndForthPauseCoroutine()
    {
        yield return RSLib.Yield.SharedYields.WaitForSeconds(_backAndForthPause);
        yield return new WaitUntil(() => !AttackCtrl.IsAttacking);
        _backAndForthPauseCoroutine = null;
    }

    private System.Collections.IEnumerator PauseBeforeAttackCoroutine()
    {
        yield return RSLib.Yield.SharedYields.WaitForSeconds(_beforeAttackDur);
        yield return new WaitUntil(() => _hurtCoroutine == null);
        _pauseBeforeAttackCoroutine = null;

        // Callback on attack and is a security to avoid attack animator parameter being offset from view.
        if (IsTargetDetected())
            AttackCtrl.Attack((attackOverDir) => _skeletonView.ResetAttackTrigger());
    }

    private void Awake()
    {
        AttackCtrl = new SkeletonAttackController(this);
        CollisionsCtrl = new CollisionsController(_boxCollider2D, _collisionMask);

        _initX = transform.position.x;
        CurrDir = _skeletonView.GetSpriteRendererFlipX() ? -1f : 1f;
    }

    private void Update()
    {
        // [NOTES]
        // Pour connaître la direction, utiliser une méthode ComputeDir qui agit selon la
        // situation actuelle (pause, idle, player à portée, en train d'attaquer, etc.).
        // Si FSM, alors chaque état devrait avoir une telle méthode pour pouvoir
        // faire un truc du style _fsm.CurrentState.GetDir().

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
                        Translate(new Vector3(CurrDir * _moveSpeed, 0f));
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

        Gizmos.DrawWireSphere(transform.position, _targetDetectionRange);
        Gizmos.DrawWireSphere(transform.position, _targetAttackRange);

        if (UnityEditor.EditorApplication.isPlaying)
        {
            Gizmos.DrawWireSphere(new Vector3(_initX - HalfBackAndForthRange, transform.position.y + 0.5f), 0.05f);
            Gizmos.DrawWireSphere(new Vector3(_initX + HalfBackAndForthRange, transform.position.y + 0.5f), 0.05f);
            Gizmos.DrawLine(new Vector3(_initX - HalfBackAndForthRange, transform.position.y + 0.5f),
                new Vector3(_initX + HalfBackAndForthRange, transform.position.y + 0.5f));
        }
        else
        {
            Gizmos.DrawWireSphere(transform.position.AddX(-HalfBackAndForthRange).AddY(0.5f), 0.05f);
            Gizmos.DrawWireSphere(transform.position.AddX(HalfBackAndForthRange).AddY(0.5f), 0.05f);
            Gizmos.DrawLine(transform.position.AddX(-HalfBackAndForthRange).AddY(0.5f),
                transform.position.AddX(HalfBackAndForthRange).AddY(0.5f));
        }
    }
}