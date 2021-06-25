namespace Templar.UI.Inventory
{
    using System.Linq;
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [DisallowMultipleComponent]
    public class InventoryView : UIPanel
    {
        private const string EMPTY_SLOT_NAME = "???";
        private const string EMPTY_SLOT_DESC = "???";

        [SerializeField] private Item.InventoryController _inventoryCtrl = null;
        [SerializeField] private InventorySlot[] _slotsViews = null;
        [SerializeField] private GameObject _firstSelected = null;
        [SerializeField] private TMPro.TextMeshProUGUI _itemName = null;
        [SerializeField] private TMPro.TextMeshProUGUI _itemDesc = null;

        public static bool DebugForceShowItemsQuantity { get; private set; }

        public override GameObject FirstSelected => _firstSelected;

        private void OnInventoryContentChanged(Item.InventoryController.InventoryContentChangedEventArgs args)
        {
            InventorySlot slot = GetItemSlot(args.Item) ?? GetFirstEmptySlot();
            UnityEngine.Assertions.Assert.IsNotNull(slot, $"No valid slot had been found to add or modify item {args.Item.Id}.");

            slot.SetItem(args.Item, args.NewQuantity);
        }

        private void OnInventoryCleared()
        {
            for (int i = _slotsViews.Length - 1; i >= 0; --i)
                _slotsViews[i].Clear();
        }

        private void OnInventorySlotHovered(InventorySlot slot)
        {
            if (slot.Item == null)
            {
                _itemName.text = EMPTY_SLOT_NAME;
                _itemDesc.text = EMPTY_SLOT_DESC;
                return;
            }

            _itemName.text = $"{slot.Item.Id} {(slot.Quantity > 1 ? $"({slot.Quantity})" : string.Empty)}";
            _itemDesc.text = slot.Item.Datas.Description;
        }

        public static bool CanToggleInventory()
        {
            return !RSLib.Framework.InputSystem.InputManager.IsAssigningKey
                && !Dialogue.DialogueManager.DialogueRunning
                && !Manager.OptionsManager.AnyPanelOpen();
        }

        private InventorySlot GetItemSlot(Item.Item item)
        {
            return _slotsViews.Where(o => o.Item == item).FirstOrDefault();
        }

        private InventorySlot GetFirstEmptySlot()
        {
            return _slotsViews.Where(o => o.IsEmpty).FirstOrDefault();
        }

        private void Awake()
        {
            _inventoryCtrl.InventoryContentChanged += OnInventoryContentChanged;
            _inventoryCtrl.InventoryCleared += OnInventoryCleared;
            InventorySlot.InventorySlotHovered += OnInventorySlotHovered;

            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command<bool>("ForceShowItemsQuantity", "Forces inventory to display items quantity.",
                (show) =>
                {
                    DebugForceShowItemsQuantity = show;
                    for (int i = _slotsViews.Length - 1; i >= 0; --i)
                        _slotsViews[i].Refresh();
                }));
        }

        private void Update()
        {
            if (!CanToggleInventory())
                return;

            if (Input.GetButtonDown("Inventory")) // [TODO] Constant.
            {
                if (!Displayed)
                {
                    UI.Navigation.UINavigationManager.OpenAndSelect(this);
                    Display(true);

                    return;
                }

                Navigation.UINavigationManager.CloseCurrentPanel();
            }
        }

        private void OnDestroy()
        {
            _inventoryCtrl.InventoryContentChanged -= OnInventoryContentChanged;
            _inventoryCtrl.InventoryCleared -= OnInventoryCleared;
            InventorySlot.InventorySlotHovered -= OnInventorySlotHovered;
        }

        public void LocateSlotsInParentChildren()
        {
            _slotsViews = transform.parent.GetComponentsInChildren<InventorySlot>();

#if UNITY_EDITOR
            RSLib.EditorUtilities.SceneManagerUtilities.SetCurrentSceneDirty();
            RSLib.EditorUtilities.PrefabEditorUtilities.SetCurrentPrefabStageDirty();
#endif
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(InventoryView))]
    public class InventoryViewEditor : RSLib.EditorUtilities.ButtonProviderEditor<InventoryView>
    {
        protected override void DrawButtons()
        {
            DrawButton("Locate Slots in Parent Children", Obj.LocateSlotsInParentChildren);
        }
    }
#endif
}