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

        [Header("BURST OUT")]
        [SerializeField] private Vector2 _forceRange = new Vector2(7f, 10f);
        [SerializeField] private float _initDirRandomAngle = 50f;

        private int _idleStateHash;

        public virtual void OnGetFromPool(params object[] args)
        {
            BurstOut();
        }

        protected virtual void OnLoot() { }
        
        protected void BurstOut()
        {
            _animator.Play(_idleStateHash, 0, Random.value);

            Vector3 dir = Quaternion.Euler(0f, 0f, Random.Range(-_initDirRandomAngle * 0.5f, _initDirRandomAngle * 0.5f)) * Vector2.up;
            _rb.AddForce(dir * _forceRange.Random(), ForceMode2D.Impulse);
        }

        private void Awake()
        {
            _idleStateHash = Animator.StringToHash("Idle");
        }
    }
}