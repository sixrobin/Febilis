﻿namespace Templar.Datas.Item
{
    using RSLib.Extensions;
    using System.Xml.Linq;

    public class ItemDatas : Datas
    {
        public ItemDatas(XContainer container) : base(container)
        {
        }

        public string Id { get; private set; }

        public Templar.Item.ItemType Type { get; private set; }

        public bool AlwaysShowQuantity { get; private set; }
        public bool AlwaysInInventory { get; private set; }
        public bool SkipPickupNotification { get; private set; }

        public ItemActionConditionsCheckerDatas UseConditionsCheckerDatas { get; private set; }
        public ItemActionConditionsCheckerDatas EquipConditionsCheckerDatas { get; private set; }
        public ItemActionConditionsCheckerDatas DropConditionsCheckerDatas { get; private set; }
        public ItemActionConditionsCheckerDatas MoveConditionsCheckerDatas { get; private set; }

        public override void Deserialize(XContainer container)
        {
            XElement itemElement = container as XElement;

            XAttribute idAttribute = itemElement.Attribute("Id");
            UnityEngine.Assertions.Assert.IsFalse(idAttribute.IsNullOrEmpty(), "Item Id attribute is null or empty.");
            Id = idAttribute.Value;

            XElement typeElement = itemElement.Element("Type");
            UnityEngine.Assertions.Assert.IsFalse(typeElement.IsNullOrEmpty(), $"Item {Id} Type element is null or empty.");
            Type = typeElement.ValueToEnum<Templar.Item.ItemType>();

            AlwaysShowQuantity = itemElement.Element("AlwaysShowQuantity") != null;
            AlwaysInInventory = itemElement.Element("AlwaysInInventory") != null;
            SkipPickupNotification = itemElement.Element("SkipPickupNotification") != null;

            XElement actionsConditionsElement = itemElement.Element("Actions");
            if (actionsConditionsElement != null)
            {
                XElement useConditionsElement = actionsConditionsElement.Element("Use");
                XElement equipConditionsElement = actionsConditionsElement.Element("Equip");
                XElement dropConditionsElement = actionsConditionsElement.Element("Drop");
                XElement moveConditionsElement = actionsConditionsElement.Element("Move");

                if (useConditionsElement != null)
                    UseConditionsCheckerDatas = new ItemActionConditionsCheckerDatas(useConditionsElement);

                if (equipConditionsElement != null)
                    EquipConditionsCheckerDatas = new ItemActionConditionsCheckerDatas(equipConditionsElement);

                if (dropConditionsElement != null)
                    DropConditionsCheckerDatas = new ItemActionConditionsCheckerDatas(dropConditionsElement);

                if (moveConditionsElement != null)
                    MoveConditionsCheckerDatas = new ItemActionConditionsCheckerDatas(moveConditionsElement);
            }
        }
    }
}