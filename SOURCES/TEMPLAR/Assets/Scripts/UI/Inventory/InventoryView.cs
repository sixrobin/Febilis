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
        [SerializeField] private UnityEngine.UI.Image _itemTypeIcon = null;
        [SerializeField] private TMPro.TextMeshProUGUI _itemDesc = null;

        [Header("ITEM CONTEXT MENU")]
        [SerializeField] private ContextMenu.ItemContextMenu _contextMenu = null;

        [Header("UI NAVIGATION")]
        [SerializeField, Min(1)] private int _slotsRowLength = 3;
        [SerializeField] private RectTransform _slotsViewport = null;
        [SerializeField] private UnityEngine.UI.Scrollbar _scrollbar = null;
        [SerializeField] private RectTransform _scrollHandle = null;

        private Vector3[] _slotsViewportWorldCorners = new Vector3[4];

        public static bool DebugForceShowItemsQuantity { get; private set; }

        public override GameObject FirstSelected => _firstSelected;

        public bool IsContextMenuDisplayed => _contextMenu.Displayed;

        public static bool CanToggleInventory()
        {
            return !RSLib.Framework.InputSystem.InputManager.IsAssigningKey
                && !Dialogue.DialogueManager.DialogueRunning
                && !Manager.OptionsManager.AnyPanelOpen();
        }

        public override void OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Navigation.UINavigationManager.NullifySelected();
        }

        public GameObject GetClosestSlotToScrollHandle()
        {
            RectTransform closestSlot = null;
            float sqrClosestDist = Mathf.Infinity;
            
            Vector3[] scrollHandleWorldCorners = new Vector3[4];
            _scrollHandle.GetWorldCorners(scrollHandleWorldCorners);
            Vector3 scrollHandleCenterWorld = RSLib.Maths.Maths.ComputeAverageVector(scrollHandleWorldCorners);

            foreach (RectTransform target in _slotsViews.Select(o => o.GetComponent<RectTransform>()))
            {
                Vector3[] slotWorldCorners = new Vector3[4];
                target.GetWorldCorners(slotWorldCorners);
                Vector3 slotCenterWorld = RSLib.Maths.Maths.ComputeAverageVector(slotWorldCorners);

                float sqrTargetDist = (slotCenterWorld - scrollHandleCenterWorld).sqrMagnitude;
                if (sqrTargetDist > sqrClosestDist)
                    continue;

                sqrClosestDist = sqrTargetDist;
                closestSlot = target;
            }

            return closestSlot.gameObject;
        }

        private void UpdateContent()
        {
            Clear();

            foreach (System.Collections.Generic.KeyValuePair<Item.Item, int> item in _inventoryCtrl.Items)
            {
                InventorySlot slot = GetFirstEmptySlot();
                UnityEngine.Assertions.Assert.IsNotNull(slot, $"No valid slot had been found to add item {item.Key}.");
            
                slot.SetItem(item.Key, item.Value);
            }
        }

        private void Clear()
        {
            for (int i = _slotsViews.Length - 1; i >= 0; --i)
                _slotsViews[i].Clear();
        }

        private void OnInventoryContentChanged(Item.InventoryController.InventoryContentChangedEventArgs args)
        {
            if (args.OnInit)
                return; // Full content refresh is done on Start.

            InventorySlot slot = GetItemSlot(args.Item) ?? GetFirstEmptySlot();
            UnityEngine.Assertions.Assert.IsNotNull(slot, $"No valid slot had been found to add or modify item {args.Item.Id}.");

            slot.SetItem(args.Item, args.NewQuantity);
        }

        private void OnInventoryCleared()
        {
            Clear();
        }

        private void OnInventoryInitialized()
        {
            UpdateContent();
        }

        private void OnInventorySlotHovered(InventorySlot slot)
        {
            if (slot.Item == null)
                return;

            _itemName.text = $"{slot.Item.Id} {(slot.Quantity > 1 ? $"({slot.Quantity})" : string.Empty)}";
            _itemDesc.text = slot.Item.Datas.Description;
            _itemType.text = slot.Item.Datas.Type.ToString().ToLower().UpperFirst();
            _itemTypeIcon.enabled = true;
            _itemTypeIcon.sprite = Database.ItemDatabase.GetItemTypeSprite(slot.Item);
        }

        private void OnInventorySlotExit(InventorySlot slot)
        {
            if (IsContextMenuDisplayed)
                return;

            _itemName.text = EMPTY_SLOT_NAME;
            _itemDesc.text = EMPTY_SLOT_DESC;
            _itemType.text = EMPTY_SLOT_TYPE;
            _itemTypeIcon.enabled = false;
        }

        private void OnInventorySlotButtonClicked(InventorySlot slot)
        {
            if (slot.IsEmpty)
                return;

            _contextMenu.SetSlotContext(slot);
            Navigation.UINavigationManager.OpenAndSelect(_contextMenu);
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
                _scrollbar.value += SCROLL_BAR_AUTO_REFRESH_VALUE;
                sourceRectTransform.GetWorldCorners(sourceCorners);
            }

            while (sourceCorners[0].y < _slotsViewportWorldCorners[0].y)
            {
                _scrollbar.value -= SCROLL_BAR_AUTO_REFRESH_VALUE;
                sourceRectTransform.GetWorldCorners(sourceCorners);
            }

            if (_scrollbar.value - SCROLL_BAR_AUTO_REFRESH_MARGIN < 0f)
                _scrollbar.value = 0f;
            else if (_scrollbar.value + SCROLL_BAR_AUTO_REFRESH_MARGIN > 1f)
                _scrollbar.value = 1f;
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
                    _slotsViews[i].Button.SetSelectOnRight(_scrollbar);
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

            //// [TODO] Scrollbar selects top left corner slot. We may want to select a better one depending on the current viewport state.
            //_scrollbar.SetMode(UnityEngine.UI.Navigation.Mode.Explicit);
            //_scrollbar.SetSelectOnLeft(_slotsViews[_slotsRowLength - 1].Button);
        }

        private InventorySlot GetItemSlot(Item.Item item)
        {
            return _slotsViews.Where(o => o.Item == item).FirstOrDefault();
        }

        private InventorySlot GetFirstEmptySlot()
        {
            return _slotsViews.Where(o => o.IsEmpty).FirstOrDefault();
        }

        private void Start()
        {
            _inventoryCtrl.InventoryContentChanged += OnInventoryContentChanged;
            _inventoryCtrl.InventoryCleared += OnInventoryCleared;
            _inventoryCtrl.InventoryInitialized += OnInventoryInitialized;

            InventorySlot.InventorySlotHovered += OnInventorySlotHovered;
            InventorySlot.InventorySlotExit += OnInventorySlotExit;

            for (int i = _slotsViews.Length - 1; i >= 0; --i)
            {
                InventorySlot slotView = _slotsViews[i];
                slotView.Button.onClick.AddListener(() => OnInventorySlotButtonClicked(slotView));
                slotView.SetInventoryView(this);
            }

            InitNavigation();
            UpdateContent();

            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command("ClearInventoryView", "Instantly clears the inventory view.", Clear));
            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command("UpdateInventoryView", "Instantly refreshes the inventory view based on inventory content.", UpdateContent));
            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command<bool>(
                "ForceShowItemsQuantity",
                "Forces inventory to display items quantity.",
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
                    _scrollbar.value = 1f;
                    Open();

                    return;
                }

                Navigation.UINavigationManager.CloseCurrentPanel();
                Navigation.UINavigationManager.NullifySelected();
            }
        }

        private void OnDestroy()
        {
            _inventoryCtrl.InventoryContentChanged -= OnInventoryContentChanged;
            _inventoryCtrl.InventoryCleared -= OnInventoryCleared;
            _inventoryCtrl.InventoryInitialized -= OnInventoryInitialized;

            InventorySlot.InventorySlotHovered -= OnInventorySlotHovered;
            InventorySlot.InventorySlotExit -= OnInventorySlotExit;

            for (int i = _slotsViews.Length - 1; i >= 0; --i)
            {
                InventorySlot slotView = _slotsViews[i];
                slotView.Button.onClick.RemoveListener(() => OnInventorySlotButtonClicked(slotView));
            }
        }

        public void LocateSlotsInParentChildren()
        {
            _slotsViews = transform.parent.GetComponentsInChildren<InventorySlot>();
            RSLib.EditorUtilities.SceneManagerUtilities.SetCurrentSceneDirty();
            RSLib.EditorUtilities.PrefabEditorUtilities.SetCurrentPrefabStageDirty();
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