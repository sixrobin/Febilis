namespace Templar.Attack
{
    public abstract class AttackController
    {
        public class AttackOverEventArgs : System.EventArgs
        {
            public AttackOverEventArgs(float dir)
            {
                Dir = dir;
            }

            public float Dir { get; private set; }
        }

        protected UnityEngine.MonoBehaviour _attackCoroutineRunner;
        protected System.Collections.IEnumerator _attackCoroutine;

        private AttackHitboxesContainer _hitboxesContainer;
        private System.Collections.Generic.Dictionary<string, AttackHitbox> _hitboxesById;

        public AttackController(UnityEngine.MonoBehaviour attackCoroutineRunner, AttackHitboxesContainer hitboxesContainer, UnityEngine.Transform attacksSource)
        {
            _attackCoroutineRunner = attackCoroutineRunner;
            _hitboxesContainer = hitboxesContainer;

            _hitboxesById = new System.Collections.Generic.Dictionary<string, AttackHitbox>();
            for (int i = _hitboxesContainer.AttackHitboxes.Length - 1; i >= 0; --i)
            {
                UnityEngine.Assertions.Assert.IsFalse(
                    _hitboxesById.ContainsKey(_hitboxesContainer.AttackHitboxes[i].Id),
                    $"Duplicate Id {_hitboxesContainer.AttackHitboxes[i].Id} found for attack hitboxes.");

                _hitboxesById.Add(_hitboxesContainer.AttackHitboxes[i].Id, _hitboxesContainer.AttackHitboxes[i]);
                _hitboxesContainer.AttackHitboxes[i].Hit += OnAttackHit;
                _hitboxesContainer.AttackHitboxes[i].SetAttackSource(attacksSource);
            }
        }

        public delegate void AttackOverEventHandler(AttackOverEventArgs args);

        public float AttackDir { get; protected set; }

        public virtual bool IsAttacking => _attackCoroutine != null;

        public virtual void CancelAttack()
        {
            if (_attackCoroutine == null)
                return;

            _attackCoroutineRunner.StopCoroutine(_attackCoroutine);
            _attackCoroutine = null;
        }

        protected virtual void TriggerHit(Datas.Attack.AttackDatas attackDatas, string id)
        {
            UnityEngine.Assertions.Assert.IsNotNull(attackDatas, "Triggering hit with null attack datas.");
            UnityEngine.Assertions.Assert.IsTrue(_hitboxesById.ContainsKey(id), $"Could not find hitbox with Id {id}.");

            _hitboxesContainer.SetDirection(AttackDir);
            _hitboxesById[id].Trigger(AttackDir, attackDatas);

            UnityEngine.Object.FindObjectOfType<Camera.CameraController>().Shake.AddTrauma(attackDatas.TraumaOnAttackFrame); // [TMP] Find.
        }

        protected abstract void ComputeAttackDirection();

        protected abstract void OnAttackHit(AttackHitbox.HitEventArgs hitArgs);
    }
}