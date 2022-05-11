namespace Templar.Datas.Dialogue
{
    using RSLib.Extensions;
    using System.Xml.Linq;

    public class DialogueSellItemDatas : Datas, IDialogueSequenceElementDatas
    {
        public DialogueSellItemDatas(XContainer container) : base(container)
        {
        }

        public string ItemId { get; private set; }
        public int Price { get; private set; }
        public int Quantity { get; private set; }

        public string CustomLocalizationId { get; private set; }
        
        public override void Deserialize(XContainer container)
        {
            XElement sellItemElement = container as XElement;

            XAttribute itemIdAttribute = sellItemElement.Attribute("Id");
            UnityEngine.Assertions.Assert.IsNotNull(itemIdAttribute, "Dialogue SellItem element needs an Id attribute.");
            UnityEngine.Assertions.Assert.IsTrue(Database.ItemDatabase.ItemsDatas.ContainsKey(itemIdAttribute.Value), $"Unknown ItemId {itemIdAttribute.Value} in {typeof(Database.ItemDatabase).Name}.");
            ItemId = itemIdAttribute.Value;

            XAttribute priceAttribute = sellItemElement.Attribute("Price");
            UnityEngine.Assertions.Assert.IsNotNull(priceAttribute, "Dialogue SellItem element needs a Price attribute.");
            Price = priceAttribute.ValueToInt();
            UnityEngine.Assertions.Assert.IsTrue(Price >= 0, $"Dialogue SellItem Price value cannot be negative ({Price}) for Item {ItemId}.");

            XAttribute quantityAttribute = sellItemElement.Attribute("Quantity");
            Quantity = quantityAttribute?.ValueToInt() ?? 1;

            XElement customLocalizationIdElement = sellItemElement.Element("CustomLocalizationId");
            CustomLocalizationId = customLocalizationIdElement?.Value;
        }
    }
}