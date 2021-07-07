namespace Templar
{
    using UnityEngine;

    public class CoinController : LootItemPhysicsController
    {
        [Header("PICKUP")]
        [SerializeField] private GameObject _pickupParticlesPrefab = null;

        public delegate void CoinDisabledEventHandler(CoinController coin);
        public static CoinDisabledEventHandler CoinDisabled;

        protected override void OnLoot()
        {
            Manager.GameManager.InventoryCtrl.AddItem(Item.InventoryController.ITEM_ID_COIN, 1);
        }

        private void Disable()
        {
            RSLib.Framework.Pooling.Pool.Get(_pickupParticlesPrefab).transform.position = transform.position;
            gameObject.SetActive(false);

            CoinDisabled?.Invoke(this);
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            OnLoot();
            Disable();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (KillTrigger.SharedKillTriggers.ContainsKey(collision.collider))
                Disable();
        }
    }
}