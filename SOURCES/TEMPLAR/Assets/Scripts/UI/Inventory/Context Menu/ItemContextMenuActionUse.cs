﻿namespace Templar.UI.Inventory.ContextMenu
{
    using UnityEngine;

    [DisallowMultipleComponent]
    public class ItemContextMenuActionUse : ItemContextMenuAction
    {
        protected override bool IsActionAllowed()
        {
            return Slot.Item.UseConditionsChecker.CheckConditions();
        }

        protected override void TriggerAction()
        {
            CProLogger.Log(this, $"Using {Slot.Item.Id}.");

            // [TODO] This is hardcoded, we might want xml tags like <Heal/> or something, probably.
            if (Slot.Item.Id == Item.InventoryController.ITEM_ID_POTION)
            {
                _contextMenu.Close(); // Closes all inventory panel.
                Manager.GameManager.PlayerCtrl.TriggerHeal();
            }
        }
    }
}