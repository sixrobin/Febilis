using UnityEngine;

public class SkeletonController : MonoBehaviour, IHittable
{
    [SerializeField] private SkeletonView _skeletonView = null;
    [SerializeField] private BoxCollider2D _boxCollider2D = null;
    [SerializeField] private Transform _target = null;
    [SerializeField] private SkeletonControllerDatas _controllerDatas = null;
    [SerializeField] private AttackHitboxesContainer _attackHitboxesContainer = null;
    [SerializeField] private LayerMask _collisionMask = 0;

    [Header("FIGHT")]
    [SerializeField, Min(0f)] private float _attackAnticipationDur = 0.5f;
    [SerializeField, Min(0f)] private float _attackDur = 0.5f;
    [SerializeField, Min(0f)] private float _hurtDur = 0.25f;

    private Recoil _currentRecoil;

    public HitLayer HitLayer => HitLayer.Enemy;

    public SkeletonView SkeletonView => _skeletonView;
    public SkeletonControllerDatas ControllerDatas => _controllerDatas;
    public AttackHitboxesContainer AttackHitboxesContainer => _attackHitboxesContainer;

    public SkeletonAttackController AttackCtrl { get; private set; }
    public CollisionsController CollisionsCtrl { get; private set; }

    public float Dir { get; private set; }

    public void OnHit(AttackDatas attackDatas, float dir)
    {
        if (AttackCtrl.IsAttacking)
            return;

        _currentRecoil = new Recoil(-1f, force: 2.5f, dur: 0.15f);
        StartCoroutine(HurtCoroutine());
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
    }

    private void Awake()
    {
        AttackCtrl = new SkeletonAttackController(this);
        CollisionsCtrl = new CollisionsController(_boxCollider2D, _collisionMask);
    }

    private void Update()
    {
        if (!AttackCtrl.IsAttacking && Input.GetKeyDown(KeyCode.M))
            AttackCtrl.Attack();

        if (_currentRecoil != null)
        {
            Translate(new Vector3(_currentRecoil.Dir * _currentRecoil.Force, 0f));
            _currentRecoil.Update();
            if (_currentRecoil.IsComplete)
                _currentRecoil = null;
        }

        Dir = Mathf.Sign(_target.position.x - transform.position.x);
        _skeletonView.UpdateView(Dir < 0f);
    }
}