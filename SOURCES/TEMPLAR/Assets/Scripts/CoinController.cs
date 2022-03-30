namespace Templar
{
    using UnityEngine;

    public class CoinController : LootItemPhysicsController
    {
        [Header("PICKUP")]
        [SerializeField] private GameObject _pickupParticlesPrefab = null;
        [SerializeField] private Attack.HittablePickup _hittablePickup = null;

        [Header("AUDIO")]
        [SerializeField] private RSLib.Audio.ClipProvider _pickupClipProvider = null;

        private static int s_pickedUpCoinsThisFrame = 0;
        
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
            
            if (s_pickedUpCoinsThisFrame == 0)
                RSLib.Audio.AudioManager.PlaySound(_pickupClipProvider);
    
            s_pickedUpCoinsThisFrame++;
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

        private void LateUpdate()
        {
            s_pickedUpCoinsThisFrame = 0;
        }
    }
}