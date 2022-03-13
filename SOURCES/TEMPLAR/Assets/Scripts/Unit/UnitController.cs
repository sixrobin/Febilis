namespace Templar.Unit
{
    using UnityEngine;

    [DisallowMultipleComponent]
    public abstract class UnitController : MonoBehaviour, Templar.Physics.MovingPlatform.IMovingPlatformPassenger
    {
        [Header("UNIT BASE")]
        [SerializeField] private BoxCollider2D _boxCollider2D = null;
        [SerializeField] private Attack.AttackHitboxesContainer _attackHitboxesContainer = null;
        [SerializeField] private UnitHealthController _healthCtrl = null;
        [SerializeField] private LayerMask _collisionMask = 0;
        [SerializeField] private RSLib.Framework.DisabledString _debugCollisionsState = new RSLib.Framework.DisabledString();

        public BoxCollider2D BoxCollider2D => _boxCollider2D;
        public Attack.AttackHitboxesContainer AttackHitboxesContainer => _attackHitboxesContainer;
        public UnitHealthController HealthCtrl => _healthCtrl;
        public LayerMask CollisionMask => _collisionMask;

        public abstract UnitView UnitView { get; }

        protected Templar.Physics.Recoil _currentRecoil;
        private System.Collections.IEnumerator _deadFadeCoroutine;
        private System.Collections.IEnumerator _stunCoroutine;

        public Templar.Physics.CollisionsController CollisionsCtrl { get; protected set; }

        public float CurrDir { get; protected set; }

        public bool IsDead => HealthCtrl.HealthSystem?.IsDead ?? false;

        public bool IsOnMovingPlatform { get; set; }

        public bool IsStunned { get; private set; }

        void Templar.Physics.MovingPlatform.IMovingPlatformPassenger.OnPlatformMoved(Vector3 vel, bool standingOnPlatform)
        {
            if (float.IsNaN(vel.x) || float.IsNaN(vel.y))
            {
                // This is a hack to avoid this issue: https://app.hacknplan.com/p/148469/kanban?categoryId=8&boardId=392835&taskId=150&tabId=basicinfo
                // Should check why the vector has such a value instead of just returning.
                CProLogger.LogWarning(this, $"Translating {transform.name} on MovingPlatform by a NaN vector.", gameObject);
                return;
            }

            IsOnMovingPlatform = true;
            Translate(vel, triggerEvents: false, standingOnPlatform: standingOnPlatform);
        }

        public void SetDirection(float dir)
        {
            CurrDir = dir;
        }

        public virtual void Translate(Vector3 vel, bool triggerEvents = true, bool checkEdge = false, bool effectorDown = false, bool standingOnPlatform = false)
        {
            vel = CollisionsCtrl.ComputeCollisions(vel * Time.deltaTime, triggerEvents, checkEdge, effectorDown, standingOnPlatform);
            transform.Translate(vel);
        }

        public void Translate(float x, float y, bool triggerEvents = true, bool checkEdge = false, bool effectorDown = false, bool standingOnPlatform = false)
        {
            Translate(new Vector3(x, y), triggerEvents, checkEdge, effectorDown, standingOnPlatform);
        }

        public void LookAt(Vector3 target)
        {
            SetDirection(Mathf.Sign(target.x - transform.position.x));
            UnitView.FlipX(CurrDir < 0f);
        }

        public void Stun(float dur, float delay, System.Func<bool> conditionalDelay, System.Action callback = null)
        {
            StartCoroutine(_stunCoroutine = StunCoroutine(dur, delay, conditionalDelay, callback));
        }

        protected virtual void OnCollisionDetected(Templar.Physics.CollisionsController.CollisionInfos collisionInfos)
        {
            if (collisionInfos.Hit && KillTrigger.SharedKillTriggers.ContainsKey(collisionInfos.Hit.collider))
                HealthCtrl.Kill();
        }
        
        protected void ApplyCurrentRecoil()
        {
            if (_currentRecoil == null)
                return;

            float recoilX = _currentRecoil.Dir * _currentRecoil.Force;
            if (!CollisionsCtrl.Below)
                recoilX *= _currentRecoil.AirborneMult;

            Translate(new Vector3(recoilX, 0f), triggerEvents: false, checkEdge: _currentRecoil.CheckEdge);

            _currentRecoil.Update();
            if (_currentRecoil.IsComplete)
                _currentRecoil = null;
        }

        protected Vector3 GetCurrentRecoil()
        {
            if (_currentRecoil == null)
                return Vector3.zero;

            Vector3 recoil = new Vector3(_currentRecoil.Dir * _currentRecoil.Force, 0f);
            _currentRecoil.Update();
            if (_currentRecoil.IsComplete)
                _currentRecoil = null;

            return recoil;
        }

        protected void StartDeadFadeCoroutine()
        {
            StartCoroutine(_deadFadeCoroutine = DeadFadeCoroutine());
        }

        private System.Collections.IEnumerator DeadFadeCoroutine()
        {
            yield return RSLib.Yield.SharedYields.WaitForSeconds(UnitView.DeadFadeDelay);

            _deadFadeCoroutine = null;
            if (IsDead)
                UnitView.PlayDeadFadeAnimation();
        }

        private System.Collections.IEnumerator StunCoroutine(float dur, float delay, System.Func<bool> conditionalDelay, System.Action callback = null)
        {
            yield return RSLib.Yield.SharedYields.WaitForSeconds(delay);
            yield return new WaitUntil(conditionalDelay);

            UnitView.PlayStunAnimation(CurrDir);

            IsStunned = true;
            yield return RSLib.Yield.SharedYields.WaitForSeconds(dur);
            IsStunned = false;

            UnitView.OnStunAnimationOver();

            callback?.Invoke();
        }

        protected virtual void Update()
        {
#if UNITY_EDITOR
            _debugCollisionsState = new RSLib.Framework.DisabledString(CollisionsCtrl.CurrentStates.ToString());
#endif
        }

        protected virtual void LateUpdate()
        {
            IsOnMovingPlatform = false;
        }
    }
}