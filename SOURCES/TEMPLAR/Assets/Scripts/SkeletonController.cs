using UnityEngine;

public class SkeletonController : MonoBehaviour, IHittable
{
    [SerializeField] private SkeletonView _skeletonView = null;
    [SerializeField] private BoxCollider2D _boxCollider2D = null;
    [SerializeField] private Transform _target = null;
    [SerializeField] private LayerMask _collisionMask = 0;

    private Recoil _currentRecoil;

    public CollisionsController CollisionsCtrl { get; private set; }

    public void OnHit()
    {
        _currentRecoil = new Recoil(-1f, force: 2.5f, dur: 0.15f);
        _skeletonView.PlayHurtAnimation();
    }

    private void Translate(Vector3 vel)
    {
        vel = CollisionsCtrl.ComputeCollisions(vel * Time.deltaTime);
        transform.Translate(vel);
    }

    private void Awake()
    {
        CollisionsCtrl = new CollisionsController(_boxCollider2D, _collisionMask);
    }

    private void Update()
    {
        if (_currentRecoil != null)
        {
            Translate(new Vector3(_currentRecoil.Dir * _currentRecoil.Force, 0f));
            _currentRecoil.Update();
            if (_currentRecoil.IsComplete)
                _currentRecoil = null;
        }

        _skeletonView.UpdateView(transform.position.x - _target.position.x > 0f);
    }
}