namespace Templar
{
    using RSLib.Extensions;
    using UnityEngine;

    [DisallowMultipleComponent]
    public abstract class LootItemPhysicsController : MonoBehaviour, RSLib.Framework.Pooling.IPoolItem
    {
        [Header("REFS")]
        [SerializeField] private Rigidbody2D _rb = null;
        [SerializeField] private Animator _animator = null;

        [Header("INTERACTIVITY DELAY")]
        [SerializeField, Min(0f)] private float _interactivityDelay = 0.33f;

        [Header("BURST OUT")]
        [SerializeField] private Vector2 _forceRange = new Vector2(7f, 10f);
        [SerializeField] private float _initDirRandomAngle = 50f;

        private int _idleStateHash;

        private bool _interactable;
        public virtual bool Interactable
        {
            get => _interactable;
            protected set => _interactable = value;
        }

        public virtual void OnGetFromPool(params object[] args)
        {
            Interactable = false;
            StartCoroutine(DelayInteractivityCoroutine());

            BurstOut();
        }

        protected virtual void OnLoot() { }
        
        protected void BurstOut()
        {
            _animator.Play(_idleStateHash, 0, Random.value);

            Vector3 dir = Quaternion.Euler(0f, 0f, Random.Range(-_initDirRandomAngle * 0.5f, _initDirRandomAngle * 0.5f)) * Vector2.up;
            _rb.AddForce(dir * _forceRange.Random(), ForceMode2D.Impulse);
        }

        private System.Collections.IEnumerator DelayInteractivityCoroutine()
        {
            yield return RSLib.Yield.SharedYields.WaitForSeconds(_interactivityDelay);
            Interactable = true;
        }

        private void Awake()
        {
            _idleStateHash = Animator.StringToHash("Idle");
        }
    }
}