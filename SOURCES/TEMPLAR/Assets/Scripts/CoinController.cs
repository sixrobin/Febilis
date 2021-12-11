namespace Templar
{
    using UnityEngine;

    public class CoinController : LootItemPhysicsController
    {
        [Header("PICKUP")]
        [SerializeField] private GameObject _pickupParticlesPrefab = null;
        [SerializeField] private Attack.HittablePickup _hittablePickup = null;

        public delegate void CoinDisabledEventHandler(CoinController coin);
        public static CoinDisabledEventHandler CoinDisabled;

        public override bool Interactable
        {
            get => base.Interactable;
            protected set
            {
                base.Interactable = value;
                _hittablePickup.Interactable = value;
            }
        }

        public override void OnGetFromPool(params object[] args)
        {
            base.OnGetFromPool(args);
            _hittablePickup.Hit += Pickup;
        }

        protected override void OnLoot()
        {
            Manager.GameManager.InventoryCtrl.AddItem(Item.InventoryController.ITEM_ID_COIN, 1);
        }

        private void Pickup()
        {
            OnLoot();
            Disable();
        }

        private void Disable()
        {
            RSLib.Framework.Pooling.Pool.Get(_pickupParticlesPrefab).transform.position = transform.position;
            gameObject.SetActive(false);

            CoinDisabled?.Invoke(this);
            _hittablePickup.Hit -= Pickup;

            Interactable = false;
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (!Interactable || collider.gameObject == _hittablePickup.gameObject)
                return;

            Pickup();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (KillTrigger.SharedKillTriggers.ContainsKey(collision.collider))
                Disable();
        }
    }
}