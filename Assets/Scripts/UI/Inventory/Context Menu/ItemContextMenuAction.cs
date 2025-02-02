﻿namespace Templar.UI.Inventory.ContextMenu
{
    using UnityEngine;

    [DisallowMultipleComponent]
    public abstract class ItemContextMenuAction : MonoBehaviour
    {
        private const string SELECTED_TEXT_PREFIX = ">";

        [SerializeField] private RSLib.Framework.GUI.EnhancedButton _btn = null;
        
        protected ItemContextMenu _contextMenu;
        private string _initBtnText;

        public RSLib.Framework.GUI.EnhancedButton Button => _btn;

        public InventorySlot Slot { get; private set; }

        public bool ActionAllowed { get; private set; }

        protected abstract bool IsActionAllowed();
        protected abstract void TriggerAction();

        public abstract void Localize();
        
        public void SetContextMenu(ItemContextMenu contextMenu)
        {
            _contextMenu = contextMenu;
        }

        private void OnButtonPointerEnter(RSLib.Framework.GUI.EnhancedButton source)
        {
            Button.SetText($"{SELECTED_TEXT_PREFIX}{_initBtnText}");
        }

        private void OnButtonPointerExit(RSLib.Framework.GUI.EnhancedButton source)
        {
            Button.SetText(_initBtnText);
        }

        public void Init(InventorySlot slot)
        {
            Slot = slot;

            ActionAllowed = IsActionAllowed();
            Button.Interactable = ActionAllowed;

            Localize();
            _initBtnText = Button.GetText().text;

            // Set text according to state ? Like equip/unequip ?
            // Need some virtual method if so.
        }
        
        private void Awake()
        {
            Button.onClick.AddListener(TriggerAction);
            Button.PointerEnter += OnButtonPointerEnter;
            Button.PointerExit += OnButtonPointerExit;
        }

        private void OnDestroy()
        {
            Button.onClick.RemoveListener(TriggerAction);
            Button.PointerEnter -= OnButtonPointerEnter;
            Button.PointerExit -= OnButtonPointerExit;
        }

        protected virtual void Reset()
        {
            if (_btn == null)
                _btn = GetComponent<RSLib.Framework.GUI.EnhancedButton>();
        }
    }
}