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
    [SerializeField, Min(0f)] private float _playerDetectionRange = 4f;
    [SerializeField, Min(0f)] private float _attackAnticipationDur = 0.5f;
    [SerializeField, Min(0f)] private float _attackDur = 0.5f;
    [SerializeField, Min(0f)] private float _hurtDur = 0.25f;

    [Header("DEBUG")]
    [SerializeField] private RSLib.DataColor _debugColor = null;

    private System.Collections.IEnumerator _hurtCoroutine;
    private System.Collections.IEnumerator _backAndForthPauseCoroutine;

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

    private bool IsTargetInRange()
    {
        return (_target.position - transform.position).sqrMagnitude <= _playerDetectionRange * _playerDetectionRange;
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
        _skeletonView.PlayIdleAnimation();
        _hurtCoroutine = null;
    }

    private System.Collections.IEnumerator BackAndForthPauseCoroutine()
    {
        yield return RSLib.Yield.SharedYields.WaitForSeconds(_backAndForthPause);
        yield return new WaitUntil(() => !AttackCtrl.IsAttacking);
        _backAndForthPauseCoroutine = null;
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

        // [TMP]
        if (!AttackCtrl.IsAttacking && Input.GetKeyDown(KeyCode.M))
            AttackCtrl.Attack();

        if (_currentRecoil != null)
        {
            Translate(new Vector3(_currentRecoil.Dir * _currentRecoil.Force, 0f));
            _currentRecoil.Update();
            if (_currentRecoil.IsComplete)
                _currentRecoil = null;
        }

        if (!AttackCtrl.IsAttacking)
        {
            if (IsTargetInRange())
                CurrDir = Mathf.Sign(_target.position.x - transform.position.x);
            else
                MoveBackAndForth();
        }

        _skeletonView.UpdateView(_backAndForthPauseCoroutine != null ? _backAndForthPauseDir < 0f : CurrDir < 0f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = _debugColor != null ? _debugColor.Color : Color.red;

        Gizmos.DrawWireSphere(transform.position, _playerDetectionRange);

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