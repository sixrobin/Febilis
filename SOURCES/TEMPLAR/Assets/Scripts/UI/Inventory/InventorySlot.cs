namespace Templar.UI.Inventory
{
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [DisallowMultipleComponent]
    public class InventorySlot : MonoBehaviour
    {
        [SerializeField] private RSLib.Framework.GUI.EnhancedButton _button = null;
        [SerializeField] private UnityEngine.UI.Image _slotBackgroundImage = null;
        [SerializeField] private UnityEngine.UI.Image _itemImage = null;
        [SerializeField] private TMPro.TextMeshProUGUI _quantityText = null;
        [SerializeField] private GameObject _selector = null;

        [SerializeField] private Sprite _emptySlotSprite = null;
        [SerializeField] private Sprite _takenSlotSprite = null;

        public delegate void InventorySlotHoveredEventHandler(InventorySlot slot);
        public static event InventorySlotHoveredEventHandler InventorySlotHovered;
        public static event InventorySlotHoveredEventHandler InventorySlotExit;

        public InventoryView InventoryView { get; private set; }
        public Item.Item Item { get; private set; }
        public int Quantity { get; private set; }

        public bool IsEmpty => Item == null;

        public RSLib.Framework.GUI.EnhancedButton Button => _button;

        public RectTransform RectTransform { get; private set; }

        public void SetInventoryView(InventoryView inventoryView)
        {
            InventoryView = inventoryView;
        }

        public void DisplaySelector(bool show)
        {
            _selector.SetActive(show);
        }

        private void OnPointerEnter(RSLib.Framework.GUI.EnhancedButton source)
        {
            DisplaySelector(true);
            InventorySlotHovered?.Invoke(this);
        }

        private void OnPointerExit(RSLib.Framework.GUI.EnhancedButton source)
        {
            if (InventoryView.IsContextMenuDisplayed) // Current slot context menu is open, don't hide selector.
                return;

            DisplaySelector(false);
            InventorySlotExit?.Invoke(this);
        }

        public void Copy(InventorySlot source)
        {
            SetItem(source.Item, source.Quantity);
        }

        public void Clear()
        {
            Item = null;
            Quantity = 0;

            Refresh();
        }

        public void SetItem(Item.Item item, int quantity)
        {
            Item = item;
            Quantity = quantity;

            if (Quantity == 0 && !Item.Datas.AlwaysInInventory)
            {
                Clear();
                return;
            }

            Refresh();
        }

        public void Refresh()
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
            _itemImage.sprite = Database.ItemDatabase.GetItemSprite(Item);

            _quantityText.enabled = ShouldShowQuantity();
            if (_quantityText.enabled)
                _quantityText.text = Quantity.ToString();
        }

        private bool ShouldShowQuantity()
        {
            return InventoryView.DebugForceShowItemsQuantity || Quantity > 1 || Item.Datas.AlwaysShowQuantity;
        }

        private void Awake()
        {
            _button.PointerEnter += OnPointerEnter;
            _button.PointerExit += OnPointerExit;

            RectTransform = GetComponent<RectTransform>();
        }
    }
}