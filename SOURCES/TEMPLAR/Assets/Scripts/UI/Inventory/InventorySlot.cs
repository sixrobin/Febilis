namespace Templar.UI.Inventory
{
    using UnityEngine;

    [DisallowMultipleComponent]
    public class InventorySlot : MonoBehaviour
    {
        [SerializeField] private UnityEngine.UI.Image _slotBackgroundImage = null;
        [SerializeField] private UnityEngine.UI.Image _itemImage = null;
        [SerializeField] private TMPro.TextMeshProUGUI _quantityText = null;
        [SerializeField] private GameObject _selector = null;

        [SerializeField] private Sprite _emptySlotSprite = null;
        [SerializeField] private Sprite _takenSlotSprite = null;

        public string ItemId { get; private set; }
        public int Quantity { get; private set; }

        public bool IsEmpty => string.IsNullOrEmpty(ItemId);

        public void Clear()
        {
            ItemId = null;
            Quantity = 0;

            Refresh();
        }

        public void SetItem(string id, int quantity)
        {
            ItemId = id;
            Quantity = quantity;

            if (Quantity == 0 && !Database.ItemDatabase.ItemsDatas[ItemId].AlwaysInInventory)
            {
                Clear();
                return;
            }

            Refresh();
        }

        private void Refresh()
        {
            if (IsEmpty)
            {
                _slotBackgroundImage.sprite = _emptySlotSprite;
                _itemImage.enabled = false;
                _quantityText.enabled = false;

                return;
            }

            _slotBackgroundImage.sprite = _takenSlotSprite;
            _itemImage.enabled = true;
            _itemImage.sprite = Database.ItemDatabase.GetItemSprite(ItemId);

            _quantityText.enabled = Quantity > 1 || Database.ItemDatabase.ItemsDatas[ItemId].AlwaysShowQuantity;
            if (_quantityText.enabled)
                _quantityText.text = Quantity.ToString();
        }
    }
}