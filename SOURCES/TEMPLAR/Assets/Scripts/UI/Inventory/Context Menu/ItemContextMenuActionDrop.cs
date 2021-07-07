﻿namespace Templar.UI.Inventory.ContextMenu
{
    using RSLib.Extensions;
    using UnityEngine;

    public class ItemContextMenuActionDrop : ItemContextMenuAction
    {
        private const float DROP_X_OFFSET = 1.5f;
        private const float DROP_Y_OFFSET = 0.5f;

        protected override bool IsActionAllowed()
        {
            return !Slot.Item.Datas.AlwaysInInventory
                && Slot.Item.DropConditionsChecker.CheckConditions();
                // && !"IsQuestItem"
        }

        protected override void TriggerAction()
        {
            _contextMenu.Close(); // Closes all inventory panel.

            Manager.LootManager.SpawnItem(
                Slot.Item.Id,
                Manager.GameManager.PlayerCtrl.transform.position.AddX(Manager.GameManager.PlayerCtrl.CurrDir * DROP_X_OFFSET).AddY(DROP_Y_OFFSET));

            Manager.GameManager.InventoryCtrl.RemoveItem(Slot.Item.Id);
        }
    }
}