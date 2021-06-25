namespace Templar.UI.Inventory
{
    using RSLib.Extensions;
    using System.Linq;
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [DisallowMultipleComponent]
    public class InventoryView : UIPanel
    {
        private const string EMPTY_SLOT_NAME = "???";
        private const string EMPTY_SLOT_TYPE = "";
        private const string EMPTY_SLOT_DESC = "???";
        private const float SCROLL_BAR_AUTO_REFRESH_VALUE = 0.02f;
        private const float SCROLL_BAR_AUTO_REFRESH_MARGIN = 0.05f;

        [SerializeField] private Item.InventoryController _inventoryCtrl = null;
        [SerializeField] private InventorySlot[] _slotsViews = null;
        [SerializeField] private GameObject _firstSelected = null;
        [SerializeField] private TMPro.TextMeshProUGUI _itemName = null;
        [SerializeField] private TMPro.TextMeshProUGUI _itemType = null;
        [SerializeField] private TMPro.TextMeshProUGUI _itemDesc = null;

        [Header("UI NAVIGATION")]
        [SerializeField, Min(1)] private int _slotsRowLength = 3;
        [SerializeField] private RectTransform _slotsViewport = null;
        [SerializeField] private RectTransform _slotsContent = null;
        [SerializeField] private UnityEngine.UI.Scrollbar _scrollBar = null;

        private Vector3[] _slotsViewportWorldCorners = new Vector3[4];

        public static bool DebugForceShowItemsQuantity { get; private set; }

        public override GameObject FirstSelected => _firstSelected;

        public static bool CanToggleInventory()
        {
            return !RSLib.Framework.InputSystem.InputManager.IsAssigningKey
                && !Dialogue.DialogueManager.DialogueRunning
                && !Manager.OptionsManager.AnyPanelOpen();
        }

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
                _itemType.text = EMPTY_SLOT_TYPE;
                return;
            }

            _itemName.text = $"{slot.Item.Id} {(slot.Quantity > 1 ? $"({slot.Quantity})" : string.Empty)}";
            _itemDesc.text = slot.Item.Datas.Description;
            _itemType.text = slot.Item.Datas.Type;
            // [TODO] Type icon. Get it from database base on Enum value.
        }

        private void OnSlotViewPointerEnter(RSLib.Framework.GUI.EnhancedButton source)
        {
            // Automatically adjust the scroll view content position so that navigating through the slots with a controller
            // works without having to move the scroll bar manually.
            // This is also handling mouse hovering for now.

            RectTransform sourceRectTransform = source.GetComponent<RectTransform>();

            Vector3[] sourceCorners = new Vector3[4];
            sourceRectTransform.GetWorldCorners(sourceCorners);
            _slotsViewport.GetWorldCorners(_slotsViewportWorldCorners);

            while (sourceCorners[1].y > _slotsViewportWorldCorners[1].y)
            {
                _scrollBar.value += SCROLL_BAR_AUTO_REFRESH_VALUE;
                sourceRectTransform.GetWorldCorners(sourceCorners);
            }

            while (sourceCorners[0].y < _slotsViewportWorldCorners[0].y)
            {
                _scrollBar.value -= SCROLL_BAR_AUTO_REFRESH_VALUE;
                sourceRectTransform.GetWorldCorners(sourceCorners);
            }

            if (_scrollBar.value - SCROLL_BAR_AUTO_REFRESH_MARGIN < 0f)
                _scrollBar.value = 0f;
            else if (_scrollBar.value + SCROLL_BAR_AUTO_REFRESH_MARGIN > 1f)
                _scrollBar.value = 1f;
        }

        private void InitNavigation()
        {
            UnityEngine.Assertions.Assert.IsTrue(_slotsViews.Length % _slotsRowLength == 0, $"Slots grid layout doesn't seem to be perfectly filled.");
            int columnLength = _slotsViews.Length / _slotsRowLength;

            for (int i = 0; i < _slotsViews.Length; ++i)
            {
                _slotsViews[i].Button.PointerEnter += OnSlotViewPointerEnter;

                _slotsViews[i].Button.SetMode(UnityEngine.UI.Navigation.Mode.Explicit);

                if (i % _slotsRowLength == 0) // Left side.
                {
                    _slotsViews[i].Button.SetSelectOnRight(_slotsViews[i + 1].Button);
                }
                else if ((i - (_slotsRowLength - 1)) % _slotsRowLength == 0) // Right side.
                {
                    _slotsViews[i].Button.SetSelectOnLeft(_slotsViews[i - 1].Button);
                    _slotsViews[i].Button.SetSelectOnRight(_scrollBar);
                }
                else
                {
                    _slotsViews[i].Button.SetSelectOnRight(_slotsViews[i + 1].Button);
                    _slotsViews[i].Button.SetSelectOnLeft(_slotsViews[i - 1].Button);
                }

                if (i < _slotsRowLength) // Up side.
                {
                    _slotsViews[i].Button.SetSelectOnDown(_slotsViews[i + _slotsRowLength].Button);
                }
                else if (i > _slotsViews.Length - _slotsRowLength - 1) // Down side.
                {
                    _slotsViews[i].Button.SetSelectOnUp(_slotsViews[i - _slotsRowLength].Button);
                }
                else
                {
                    _slotsViews[i].Button.SetSelectOnDown(_slotsViews[i + _slotsRowLength].Button);
                    _slotsViews[i].Button.SetSelectOnUp(_slotsViews[i - _slotsRowLength].Button);
                }
            }

            // [TODO] Scrollbar selects top left corner slot. We may want to select a better one depending on the current viewport state.
            _scrollBar.SetMode(UnityEngine.UI.Navigation.Mode.Explicit);
            _scrollBar.SetSelectOnLeft(_slotsViews[_slotsRowLength - 1].Button);
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

            InitNavigation();

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
                    Navigation.UINavigationManager.OpenAndSelect(this);
                    _scrollBar.value = 1f;
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