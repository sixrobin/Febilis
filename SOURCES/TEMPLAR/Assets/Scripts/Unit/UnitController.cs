namespace Templar.Unit
{
    using UnityEngine;

    [DisallowMultipleComponent]
    public class UnitController : MonoBehaviour
    {
        [SerializeField] private BoxCollider2D _boxCollider2D = null;
        [SerializeField] private Attack.AttackHitboxesContainer _attackHitboxesContainer = null;
        [SerializeField] private UnitHealthController _healthCtrl = null;
        [SerializeField] private LayerMask _collisionMask = 0;

        protected Templar.Physics.Recoil _currentRecoil;

        public BoxCollider2D BoxCollider2D => _boxCollider2D;
        public Attack.AttackHitboxesContainer AttackHitboxesContainer => _attackHitboxesContainer;
        public UnitHealthController HealthCtrl => _healthCtrl;
        public LayerMask CollisionMask => _collisionMask;

        public Templar.Physics.CollisionsController CollisionsCtrl { get; protected set; }

        public float CurrDir { get; protected set; }

        public bool IsDead => HealthCtrl.HealthSystem.IsDead;

        public void Translate(Vector3 vel)
        {
            vel = CollisionsCtrl.ComputeCollisions(vel * Time.deltaTime);
            transform.Translate(vel);
        }
    }
}