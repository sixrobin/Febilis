namespace Templar.Attack
{
    public abstract class AttackController
    {
        public class AttackOverEventArgs : System.EventArgs
        {
            public AttackOverEventArgs(Datas.Attack.AttackDatas attackDatas, float dir)
            {
                AttackDatas = attackDatas;
                Dir = dir;
            }

            public Datas.Attack.AttackDatas AttackDatas { get; private set; }
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
        public delegate void AttackHitTriggeredEventHandler();

        public event AttackHitTriggeredEventHandler AttackHitTriggered;
        
        public float AttackDir { get; protected set; }

        public virtual bool IsAttacking => _attackCoroutine != null;

        protected abstract UnityEngine.Renderer AttackerRenderer { get; }

        public virtual void CancelAttack()
        {
            if (!IsAttacking)
                return;

            _attackCoroutineRunner.StopCoroutine(_attackCoroutine);
            _attackCoroutine = null;
        }

        protected virtual void TriggerHit(Datas.Attack.AttackDatas attackDatas, string id)
        {
            UnityEngine.Assertions.Assert.IsNotNull(attackDatas, "Triggering hit with null attack datas.");
            UnityEngine.Assertions.Assert.IsTrue(_hitboxesById.ContainsKey(id), $"Could not find hitbox with Id {id}.");

            AttackHitTriggered?.Invoke();
            
            _hitboxesContainer.SetDirection(AttackDir);
            _hitboxesById[id].Trigger(AttackDir, attackDatas);

            Manager.GameManager.CameraCtrl.ApplyShakeFromDatas(attackDatas.BaseTraumaDatas, AttackerRenderer);
        }

        protected virtual void OnAttackHit(AttackHitbox.HitEventArgs hitArgs)
        {
        }

        protected abstract void ComputeAttackDirection();
    }
}