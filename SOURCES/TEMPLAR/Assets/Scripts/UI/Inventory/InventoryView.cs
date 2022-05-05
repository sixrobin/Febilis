namespace Templar.UI.Inventory
{
    using RSLib.Extensions;
    using System.Linq;
    using System.Xml.Linq;
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [DisallowMultipleComponent]
    public partial class InventoryView : UIPanel, IScrollViewClosestItemGetter
    {
        private const string INVENTORY_INPUT = "Inventory";
        private const string UI_SCROLL_INPUT = "UIScroll";
        
        private const string EMPTY_SLOT_NAME = "???";
        private const string EMPTY_SLOT_TYPE = "";
        private const string EMPTY_SLOT_DESC = "???";
        private const float SCROLL_BAR_AUTO_REFRESH_VALUE = 0.02f;
        private const float SCROLL_BAR_AUTO_REFRESH_MARGIN = 0.05f;

        [Header("REFS")]
        [SerializeField] private Item.InventoryController _inventoryCtrl = null;
        [SerializeField] private InventorySlot[] _slotsViews = null;
        [SerializeField] private GameObject _firstSelected = null;

        [Header("ITEM DETAILS")]
        [SerializeField] private TMPro.TextMeshProUGUI _itemName = null;
        [SerializeField] private TMPro.TextMeshProUGUI _itemType = null;
        [SerializeField] private UnityEngine.UI.Image _itemTypeIcon = null;
        [SerializeField] private TMPro.TextMeshProUGUI _itemDesc = null;

        [Header("ITEM DESCRIPTION SCROLL")]
        [SerializeField] private UnityEngine.UI.Scrollbar _itemDescScrollbar = null;
        [SerializeField] private RectTransform _itemDescSlidingArea = null;
        [SerializeField] private RectTransform _itemDescHandle = null;
        [SerializeField] private Vector2 _scrollHandleSizePercentageMinMax = new Vector2(0.3f, 0.8f);
        [SerializeField] private Vector2 _scrollSpeedByHandleSizePercentage = new Vector2(0.1f, 4f);
        [SerializeField, Min(0.33f)] private float _scrollSpeedMin = 0.33f;

        [Header("ITEM CONTEXT MENU")]
        [SerializeField] private ContextMenu.ItemContextMenu _contextMenu = null;

        [Header("UI NAVIGATION")]
        [SerializeField, Min(1)] private int _slotsRowLength = 3;
        [SerializeField] private RectTransform _slotsViewport = null;
        [SerializeField] private UnityEngine.UI.Scrollbar _scrollbar = null;
        [SerializeField] private RectTransform _scrollHandle = null;
        [SerializeField] private ScrollbarToScrollViewNavigationHandler _scrollbarToScrollViewNavigationHandler = null;

        public bool ClosedThisFrame { get; private set; }

        public InventorySlot MovedSlotSource { get; private set; }
        public bool IsMovingSlot => MovedSlotSource != null;

        public InventorySlot CurrentlyHoveredSlot { get; private set; }

        public static bool DebugForceShowItemsQuantity { get; private set; }

        public override GameObject FirstSelected => _firstSelected;

        public bool IsContextMenuDisplayed => _contextMenu.Displayed;

        public ScrollbarToScrollViewNavigationHandler ScrollbarToScrollViewNavigationHandler => _scrollbarToScrollViewNavigationHandler;

        public static bool CanToggleInventory()
        {
            return !RSLib.Framework.InputSystem.InputManager.IsAssigningKey
                && !Dialogue.DialogueManager.DialogueRunning
                && !Manager.BoardsTransitionManager.IsInBoardTransition
                && !Manager.OptionsManager.AnyPanelOpen();
        }

        public override void Open()
        {
            base.Open();

            _scrollbar.value = 1f;
            ResetDescriptionScrollbar();
         
            // Item button hover will be played at the same time, do not play this.
            // RSLib.Audio.UI.UIAudioManager.PlayGenericNavigationClip();
        }

        public override void Close()
        {
            if (Displayed && !ClosedThisFrame)
                StartCoroutine(CloseAtEndOfFrame());
        }

        public override void OnBackButtonPressed()
        {
            if (!Displayed)
                return;

            if (IsMovingSlot)
            {
                StopMoveSlot();
                return;
            }

            if (!ClosedThisFrame)
                StartCoroutine(CloseAtEndOfFrame());
        }

        public GameObject GetClosestItemToScrollbar()
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

        public void BeginMoveSlot(InventorySlot slot)
        {
            CProLogger.Log(this, $"Moving slot of item {slot.Item.Datas.Id}.", slot.gameObject);
            MovedSlotSource = slot;
        }

        public void StopMoveSlot()
        {
            CurrentlyHoveredSlot?.SetSelectorMovingAnimation(false);

            MovedSlotSource?.DisplayMovedItemBackground(false);
            MovedSlotSource = null;
        }

        private void OnInventoryContentChanged(Item.InventoryController.InventoryContentChangedEventArgs args)
        {
            if (args.OnInit)
                return; // Full content refresh is done on Start.

            InventorySlot slot = GetItemSlot(args.Item) ?? GetFirstEmptySlot();
            UnityEngine.Assertions.Assert.IsNotNull(slot, $"No valid slot had been found to add or modify item {args.Item.Datas.Id}.");

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
            CurrentlyHoveredSlot = slot;

            if (slot.Item == null)
                return;

            _itemName.text = $"{Localizer.Get($"{Localization.Item.NAME_PREFIX}{slot.Item.Datas.Id}")} {(slot.Quantity > 1 ? $"({slot.Quantity})" : string.Empty)}";
            _itemDesc.text = Localizer.Get($"{Localization.Item.DESCRIPTION_PREFIX}{slot.Item.Datas.Id}");
            _itemType.text = Localizer.Get($"{Localization.Item.TYPE_PREFIX}{slot.Item.Datas.Type}");
            
            _itemTypeIcon.enabled = true;
            _itemTypeIcon.sprite = Database.ItemDatabase.GetItemTypeSprite(slot.Item);

            ResetDescriptionScrollbar();
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
            if (IsMovingSlot)
            {
                UnityEngine.Assertions.Assert.IsNotNull(MovedSlotSource, $"Moving item slot but source slot is null.");

                if (slot == MovedSlotSource)
                {
                    StopMoveSlot();
                    return;
                }


                if (slot.IsEmpty)
                {
                    CProLogger.Log(this, $"Moving item to empty slot {slot.transform.name}.", slot.gameObject);
                    slot.Copy(MovedSlotSource);
                    MovedSlotSource.Clear();
                }
                else
                {
                    CProLogger.Log(this, $"Moving item to empty slot {slot.transform.name}, swapping with {slot.Item.Datas.Id}.", slot.gameObject);
                    slot.Swap(MovedSlotSource);
                }

                StopMoveSlot();
                OnInventorySlotHovered(slot);

                return;
            }

            if (slot.IsEmpty)
                return;

            _contextMenu.SetSlotContext(slot);
            Navigation.UINavigationManager.OpenAndSelect(_contextMenu);
        }

        private void OnSlotViewPointerEnter(RSLib.Framework.GUI.EnhancedButton source)
        {
            RSLib.Helpers.AdjustScrollViewToFocusedItem(source.GetComponent<RectTransform>(),
                                                        _slotsViewport,
                                                        _scrollbar,
                                                        SCROLL_BAR_AUTO_REFRESH_VALUE,
                                                        SCROLL_BAR_AUTO_REFRESH_MARGIN);
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

            _scrollbar.SetMode(UnityEngine.UI.Navigation.Mode.Explicit);
            _scrollbar.SetSelectOnLeft(ScrollbarToScrollViewNavigationHandler);
            ScrollbarToScrollViewNavigationHandler.SetClosestItemGetter(this);
        }

        private void UpdateContent()
        {
            Clear();

            foreach (System.Collections.Generic.KeyValuePair<Item.Item, int> item in _inventoryCtrl.Items)
            {
                InventorySlot slot = _itemsSlotIndexesSave != null ? GetSlotAtIndex(_itemsSlotIndexesSave[item.Key.Datas.Id]) : GetFirstEmptySlot();
                UnityEngine.Assertions.Assert.IsNotNull(slot, $"No valid slot had been found to add item {item.Key}.");
                UnityEngine.Assertions.Assert.IsTrue(slot.IsEmpty, $"Slot found to add item {item.Key} is not empty and holds item {slot.Item?.Datas.Id}.");
            
                slot.SetItem(item.Key, item.Value);
            }

            _itemsSlotIndexesSave = null; // "Consume" saved slot indexes when updating, then clear because it should be done only once.
        }

        private void Clear()
        {
            for (int i = _slotsViews.Length - 1; i >= 0; --i)
                _slotsViews[i].Clear();
        }

        private InventorySlot GetItemSlot(Item.Item item)
        {
            return _slotsViews.Where(o => o.Item == item).FirstOrDefault();
        }

        private InventorySlot GetFirstEmptySlot()
        {
            return _slotsViews.Where(o => o.IsEmpty).FirstOrDefault();
        }

        private InventorySlot GetSlotAtIndex(int index)
        {
            return _slotsViews[index];
        }
        
        private void ResetDescriptionScrollbar()
        {
            _itemDescScrollbar.value = 1f;
        }

        private void ScrollThroughDescription()
        {
            float scrollInput = Input.GetAxisRaw(UI_SCROLL_INPUT);
            if (Mathf.Abs(scrollInput) > 0.05f)
            {
                float scrollSpeed = RSLib.Maths.Maths.NormalizeClamped(
                    _itemDescHandle.rect.height / _itemDescSlidingArea.rect.height, // Handle height percentage.
                    _scrollHandleSizePercentageMinMax.x,
                    _scrollHandleSizePercentageMinMax.y,
                    _scrollSpeedByHandleSizePercentage.x,
                    _scrollSpeedByHandleSizePercentage.y);

                if (scrollSpeed < _scrollSpeedMin)
                    scrollSpeed = _scrollSpeedMin;

                _itemDescScrollbar.value = Mathf.Clamp01(_itemDescScrollbar.value + scrollSpeed * Mathf.Sign(scrollInput) * Time.deltaTime);
            }
        }

        private System.Collections.IEnumerator CloseAtEndOfFrame()
        {
            ClosedThisFrame = true;

            Display(false);

            yield return RSLib.Yield.SharedYields.WaitForEndOfFrame;

            CurrentlyHoveredSlot = null;
            UI.Navigation.UINavigationManager.CloseCurrentPanel();
            UI.Navigation.UINavigationManager.NullifySelected();

            ClosedThisFrame = false;
            
            RSLib.Audio.UI.UIAudioManager.PlayGenericNavigationClip();
        }

        protected override void Awake()
        {
            _inventoryCtrl.InventoryContentChanged += OnInventoryContentChanged;
            _inventoryCtrl.InventoryCleared += OnInventoryCleared;
            _inventoryCtrl.InventoryInitialized += OnInventoryInitialized;

            InventorySlot.InventorySlotHovered += OnInventorySlotHovered;
            InventorySlot.InventorySlotExit += OnInventorySlotExit;
        }

        private void Start()
        {
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
            if (CanToggleInventory() && RSLib.Framework.InputSystem.InputManager.GetInputDown(INVENTORY_INPUT))
            {
                if (!Displayed)
                {
                    Navigation.UINavigationManager.OpenAndSelect(this);
                    Open();
                }
                else
                {
                    Navigation.UINavigationManager.CloseCurrentPanel();
                    Navigation.UINavigationManager.NullifySelected();
                }
            }

            if (Displayed)
                ScrollThroughDescription();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

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

#if UNITY_EDITOR
        private void OnValidate()
        {
            _scrollHandleSizePercentageMinMax.y = Mathf.Clamp01(_scrollHandleSizePercentageMinMax.y);
            _scrollHandleSizePercentageMinMax.x = Mathf.Clamp(_scrollHandleSizePercentageMinMax.x, 0f, _scrollHandleSizePercentageMinMax.y);

            _scrollSpeedByHandleSizePercentage.x = Mathf.Max(_scrollSpeedMin, _scrollSpeedByHandleSizePercentage.x);
            _scrollSpeedByHandleSizePercentage.y = Mathf.Max(_scrollSpeedByHandleSizePercentage.x, _scrollSpeedByHandleSizePercentage.y);
        }
#endif
    }

    public partial class InventoryView : UIPanel
    {
        private System.Collections.Generic.Dictionary<string, int> _itemsSlotIndexesSave;

        public void Load(XElement inventoryViewElement = null)
        {
            if (inventoryViewElement == null)
                return;

            _itemsSlotIndexesSave = new System.Collections.Generic.Dictionary<string, int>();

            foreach (XElement slotElement in inventoryViewElement.Elements("Slot"))
            {
                XAttribute itemIdAttribute = slotElement.Attribute("ItemId");

                XAttribute indexAttribute = slotElement.Attribute("Index");
                int index = indexAttribute.ValueToInt();

                _itemsSlotIndexesSave.Add(itemIdAttribute.Value, index);
            }
        }

        public XElement Save()
        {
            XElement inventoryViewElement = new XElement("InventoryView");

            foreach (InventorySlot slotView in _slotsViews)
            {
                if (slotView.IsEmpty)
                    continue;

                XElement slotElement = new XElement("Slot");

                slotElement.Add(new XAttribute("Index", slotView.transform.GetSiblingIndex()));
                slotElement.Add(new XAttribute("ItemId", slotView.Item.Datas.Id));

                inventoryViewElement.Add(slotElement);
            }

            return inventoryViewElement;
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