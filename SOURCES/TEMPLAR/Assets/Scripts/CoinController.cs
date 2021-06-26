namespace Templar
{
    using RSLib.Extensions;
    using UnityEngine;

    [DisallowMultipleComponent]
    public class CoinController : MonoBehaviour, RSLib.Framework.Pooling.IPoolItem
    {
        [Header("REFS")]
        [SerializeField] private Rigidbody2D _rb = null;
        [SerializeField] private Animator _animator = null;

        [Header("BURST OUT")]
        [SerializeField] private Vector2 _forceRange = new Vector2(7f, 10f);
        [SerializeField] private float _initDirRandomAngle = 50f;

        [Header("PICKUP")]
        [SerializeField] private GameObject _pickupParticlesPrefab = null;

        private int _idleStateHash;

        public delegate void CoinDisabledEventHandler(CoinController coin);
        public static CoinDisabledEventHandler CoinDisabled;

        void RSLib.Framework.Pooling.IPoolItem.OnGetFromPool()
        {
            BurstOut();
        }

        private void Loot()
        {
            Manager.GameManager.InventoryCtrl.AddItem(Item.InventoryController.ITEM_ID_COIN, 1);
        }

        private void Disable()
        {
            RSLib.Framework.Pooling.Pool.Get(_pickupParticlesPrefab).transform.position = transform.position;
            gameObject.SetActive(false);

            CoinDisabled?.Invoke(this);
        }

        private void BurstOut()
        {
            _animator.Play(_idleStateHash, 0, Random.value);

            Vector3 dir = Quaternion.Euler(0f, 0f, Random.Range(-_initDirRandomAngle * 0.5f, _initDirRandomAngle * 0.5f)) * Vector2.up;
            _rb.AddForce(dir * _forceRange.Random(), ForceMode2D.Impulse);
        }

        private void Awake()
        {
            _idleStateHash = Animator.StringToHash("Idle");
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            Loot();
            Disable();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (KillTrigger.SharedKillTriggers.ContainsKey(collision.collider))
                Disable();
        }
    }
}