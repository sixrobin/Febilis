namespace Templar.UI.Inventory
{
    using UnityEngine;

    [DisallowMultipleComponent]
    public class InventorySlotView : MonoBehaviour
    {
        [SerializeField] private UnityEngine.UI.Image _slotBackgroundImage = null;
        [SerializeField] private UnityEngine.UI.Image _itemImage = null;

        [SerializeField] private Sprite _emptySlotSprite = null;
        [SerializeField] private Sprite _takenSlotSprite = null;

        public Item.Item Item { get; private set; }

        public void SetItem(Item.Item item)
        {
            Item = item;

            _slotBackgroundImage.sprite = Item == null ? _emptySlotSprite : _takenSlotSprite;

            if (Item == null)
            {
                _itemImage.enabled = false;
                return;
            }

            _itemImage.sprite = Database.ItemDatabase.GetItemSprite(Item);
        }
    }
}