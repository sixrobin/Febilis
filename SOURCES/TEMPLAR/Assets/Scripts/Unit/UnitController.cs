namespace Templar.Unit
{
    using UnityEngine;

    [DisallowMultipleComponent]
    public class UnitController : MonoBehaviour
    {
        [Header("UNIT BASE")]
        [SerializeField] private BoxCollider2D _boxCollider2D = null;
        [SerializeField] private Attack.AttackHitboxesContainer _attackHitboxesContainer = null;
        [SerializeField] private UnitHealthController _healthCtrl = null;
        [SerializeField] private LayerMask _collisionMask = 0;
        [SerializeField] private string _debugCollisionsState = string.Empty;

        protected Templar.Physics.Recoil _currentRecoil;

        public BoxCollider2D BoxCollider2D => _boxCollider2D;
        public Attack.AttackHitboxesContainer AttackHitboxesContainer => _attackHitboxesContainer;
        public UnitHealthController HealthCtrl => _healthCtrl;
        public LayerMask CollisionMask => _collisionMask;

        public Templar.Physics.CollisionsController CollisionsCtrl { get; protected set; }

        public float CurrDir { get; protected set; }

        public bool IsDead => HealthCtrl.HealthSystem?.IsDead ?? false;

        public void Translate(Vector3 vel, bool checkEdge = false)
        {
            vel = CollisionsCtrl.ComputeCollisions(vel * Time.deltaTime, checkEdge);
            transform.Translate(vel);
        }

        public void Translate(float x, float y, bool checkEdge = false)
        {
            Translate(new Vector3(x, y), checkEdge);
        }

        protected virtual void OnCollisionDetected(Templar.Physics.CollisionsController.CollisionInfos collisionInfos)
        {
            if (collisionInfos.Hit && KillTrigger.SharedKillTriggers.ContainsKey(collisionInfos.Hit.collider))
                HealthCtrl.HealthSystem.Kill();
        }
        
        protected void ApplyCurrentRecoil()
        {
            if (_currentRecoil == null)
                return;

            Translate(new Vector3(_currentRecoil.Dir * _currentRecoil.Force, 0f), _currentRecoil.CheckEdge);
            _currentRecoil.Update();
            if (_currentRecoil.IsComplete)
                _currentRecoil = null;
        }

        protected virtual void Update()
        {
#if UNITY_EDITOR
            _debugCollisionsState = CollisionsCtrl.CurrentStates.ToString();
#endif
        }
    }
}