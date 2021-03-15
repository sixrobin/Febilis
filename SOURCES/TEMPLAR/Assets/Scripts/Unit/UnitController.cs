using UnityEngine;

[DisallowMultipleComponent]
public class UnitController : MonoBehaviour
{
    [SerializeField] private BoxCollider2D _boxCollider2D = null;
    [SerializeField] private AttackHitboxesContainer _attackHitboxesContainer = null;
    [SerializeField] private UnitHealthController _healthController = null;
    [SerializeField] private LayerMask _collisionMask = 0;

    protected Recoil _currentRecoil;

    public BoxCollider2D BoxCollider2D => _boxCollider2D;
    public AttackHitboxesContainer AttackHitboxesContainer => _attackHitboxesContainer;
    public UnitHealthController HealthController => _healthController;
    public LayerMask CollisionMask => _collisionMask;

    public CollisionsController CollisionsCtrl { get; protected set; }

    public float CurrDir { get; protected set; }

    public bool IsDead => HealthController.HealthSystem.IsDead;

    public void Translate(Vector3 vel)
    {
        vel = CollisionsCtrl.ComputeCollisions(vel * Time.deltaTime);
        transform.Translate(vel);
    }
}