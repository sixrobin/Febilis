﻿namespace Templar.Datas.Dialogue.DialogueStructure
{
    using RSLib.Extensions;
    using System.Xml.Linq;

    public abstract class DialoguesStructureDialogueConditionDatas : Datas
    {
        public DialoguesStructureDialogueConditionDatas(XContainer container) : base(container)
        {
        }

        public override void Deserialize(XContainer container)
        {
        }
    }


    public class DialogueNeverDoneDialogueConditionDatas : DialoguesStructureDialogueConditionDatas
    {
        public DialogueNeverDoneDialogueConditionDatas(XContainer container) : base(container)
        {
        }

        public string DialogueId { get; private set; }

        public override void Deserialize(XContainer container)
        {
            base.Deserialize(container);

            XElement dialogueNeverDoneElement = container as XElement;
            DialogueId = dialogueNeverDoneElement.Value;
        }
    }
    
    
    public class ItemUnsoldDialogueConditionDatas : DialoguesStructureDialogueConditionDatas
    {
        public ItemUnsoldDialogueConditionDatas(XContainer container) : base(container)
        {
        }

        public string ItemId { get; private set; }

        public override void Deserialize(XContainer container)
        {
            base.Deserialize(container);

            XElement itemUnsoldElement = container as XElement;
            ItemId = itemUnsoldElement.Value;
        }
    }


    public class PlayerHasItemDialogueConditionDatas : DialoguesStructureDialogueConditionDatas
    {
        public PlayerHasItemDialogueConditionDatas(XContainer container) : base(container)
        {
        }

        public string ItemId { get; private set; }
        public int MinQuantity { get; private set; }

        public override void Deserialize(XContainer container)
        {
            base.Deserialize(container);

            XElement playerHasItemElement = container as XElement;

            XAttribute itemIdAttribute = playerHasItemElement.Attribute("Id");
            UnityEngine.Assertions.Assert.IsNotNull(itemIdAttribute, "Dialogue Condition PlayerHasItem element needs an Id attribute.");
            ItemId = itemIdAttribute.Value;

            XAttribute quantityAttribute = playerHasItemElement.Attribute("MinQuantity");
            MinQuantity = quantityAttribute?.ValueToInt() ?? 1;
        }
    }


    public class PlayerDoesntHaveItemDialogueConditionDatas : DialoguesStructureDialogueConditionDatas
    {
        public PlayerDoesntHaveItemDialogueConditionDatas(XContainer container) : base(container)
        {
        }

        public string ItemId { get; private set; }

        public override void Deserialize(XContainer container)
        {
            base.Deserialize(container);

            XElement playerDoesntHaveItemElement = container as XElement;

            XAttribute itemIdAttribute = playerDoesntHaveItemElement.Attribute("Id");
            UnityEngine.Assertions.Assert.IsNotNull(itemIdAttribute, "Dialogue Condition PlayerDoesntHaveItem element needs an Id attribute.");
            ItemId = itemIdAttribute.Value;
        }
    }


    public class BoardDiscoveredDialogueConditionDatas : DialoguesStructureDialogueConditionDatas
    {
        public BoardDiscoveredDialogueConditionDatas(XContainer container) : base(container)
        {
        }

        public string BoardId { get; private set; }

        public override void Deserialize(XContainer container)
        {
            base.Deserialize(container);

            XElement boardDiscoveredElement = container as XElement;
            BoardId = boardDiscoveredElement.Value;
        }
    }


    public class ZoneDiscoveredDialogueConditionDatas : DialoguesStructureDialogueConditionDatas
    {
        public ZoneDiscoveredDialogueConditionDatas(XContainer container) : base(container)
        {
        }

        public string ZoneId { get; private set; }

        public override void Deserialize(XContainer container)
        {
            base.Deserialize(container);

            XElement zoneDiscoveredElement = container as XElement;
            ZoneId = zoneDiscoveredElement.Value;
        }
    }
}