namespace Templar.UI.Dialogue
{
    using RSLib.Extensions;
    using UnityEngine;

    public class DialogueSellItemView : UIPanel
    {
        [SerializeField] private TMPro.TextMeshProUGUI _sellItemText = null;
        [SerializeField] private RSLib.Framework.GUI.EnhancedButton _confirmBtn = null;
        [SerializeField] private RSLib.Framework.GUI.EnhancedButton _cancelBtn = null;

        private bool _initialized;
        private Datas.Dialogue.DialogueSellItemDatas _sellItemData;
        private DialogueManager.ItemSellingInfo _itemSellingInfo;
        private Color _confirmTextBaseColor;
        
        public bool IsItemAffordable => Manager.GameManager.InventoryCtrl.GetItemQuantity(Item.InventoryController.ITEM_ID_COIN) >= _sellItemData.Price;
        
        public override GameObject FirstSelected => IsItemAffordable ? _confirmBtn.gameObject : _cancelBtn.gameObject;
        
        private void Init()
        {
            if (_initialized)
                return;
            
            _confirmBtn.onClick.AddListener(OnConfirmButtonClicked);
            _cancelBtn.onClick.AddListener(OnCancelButtonClicked);
            
            _initialized = true;
        }
        
        private void OnConfirmButtonClicked()
        {
            CProLogger.Log(this, $"Buying {_sellItemData.Quantity} {_sellItemData.ItemId} for {_sellItemData.Price} gold coins.", gameObject);
            
            UnityEngine.Assertions.Assert.IsTrue(
                IsItemAffordable, 
                $"Buying {_sellItemData.ItemId} for {_sellItemData.Price} with only {Manager.GameManager.InventoryCtrl.GetItemQuantity(Item.InventoryController.ITEM_ID_COIN)} owned coins.");

            Manager.GameManager.InventoryCtrl.RemoveItem(Item.InventoryController.ITEM_ID_COIN, _sellItemData.Price);
            Manager.GameManager.InventoryCtrl.AddItem(_sellItemData.ItemId, _sellItemData.Quantity);

            _itemSellingInfo.ItemSold = true;
            
            Display(false);
            UI.Navigation.UINavigationManager.NullifySelected();
        }
        
        private void OnCancelButtonClicked()
        {
            CProLogger.Log(this, $"Not buying {_sellItemData.Quantity} {_sellItemData.ItemId} for {_sellItemData.Price} gold coins.", gameObject);
            
            Display(false);
            UI.Navigation.UINavigationManager.NullifySelected();
        }

        private void InitNavigation()
        {
            _confirmBtn.SetMode(UnityEngine.UI.Navigation.Mode.Explicit);
            _cancelBtn.SetMode(UnityEngine.UI.Navigation.Mode.Explicit);

            bool isItemAffordable = IsItemAffordable;

            _confirmBtn.Interactable = IsItemAffordable;
            
            _confirmBtn.SetSelectOnUp(isItemAffordable ? _cancelBtn : null);
            _confirmBtn.SetSelectOnDown(isItemAffordable ? _cancelBtn : null);
            _cancelBtn.SetSelectOnUp(isItemAffordable ? _confirmBtn : null);
            _cancelBtn.SetSelectOnDown(isItemAffordable ? _confirmBtn : null);
        }
        
        public System.Collections.IEnumerator Open(Datas.Dialogue.DialogueSellItemDatas sellItemData, DialogueManager.ItemSellingInfo itemSellingInfo)
        {
            _sellItemData = sellItemData;
            _itemSellingInfo = itemSellingInfo;
            
            InitNavigation();
            Localize();

            UI.Navigation.UINavigationManager.OpenAndSelect(this);
            
            Display(true);
            yield return new WaitUntil(() => !Displayed);
        }

        private void Localize()
        {
            string textFormat = Localizer.Get(string.IsNullOrEmpty(_sellItemData.CustomLocalizationId)
                                              ? Localization.Dialogue.SELL_ITEM_ASK_FORMAT_DEFAULT
                                              : $"{Localization.Dialogue.SELL_ITEM_ASK_FORMAT}{_sellItemData.CustomLocalizationId}");

            string localizedItemName = Localizer.Get($"{Localization.Item.NAME_PREFIX}{_sellItemData.ItemId}");
            
            _sellItemText.text = string.Format(textFormat, _sellItemData.Quantity, localizedItemName, _sellItemData.Price);
            
            _confirmBtn.SetText(Localizer.Get(Localization.Dialogue.SELL_ITEM_CONFIRM));
            _cancelBtn.SetText(Localizer.Get(Localization.Dialogue.SELL_ITEM_CANCEL));
        }
        
        protected override void Awake()
        {
            base.Awake();
            Init();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            _confirmBtn.onClick.RemoveListener(OnConfirmButtonClicked);
            _cancelBtn.onClick.RemoveListener(OnCancelButtonClicked);
        }
    }
}