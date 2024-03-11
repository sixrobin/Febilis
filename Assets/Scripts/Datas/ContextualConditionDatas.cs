namespace Templar.Datas.ContextualConditions
{
    using RSLib.Extensions;
    using System.Xml.Linq;

    public abstract class ContextualConditionDatas : Datas
    {
        public ContextualConditionDatas(XContainer container) : base(container)
        {
        }

        public override void Deserialize(XContainer container)
        {
        }
    }


    public class HasItemContextualConditionDatas : ContextualConditionDatas
    {
        public HasItemContextualConditionDatas(XContainer container) : base(container)
        {
        }

        public string ItemId { get; private set; }
        public int MinQuantity { get; private set; }

        public override void Deserialize(XContainer container)
        {
            XElement hasItemElement = container as XElement;

            XAttribute itemIdAttribute = hasItemElement.Attribute("Id");
            UnityEngine.Assertions.Assert.IsNotNull(itemIdAttribute, "Contextual Condition HasItem element needs an Id attribute.");
            UnityEngine.Assertions.Assert.IsTrue(Database.ItemDatabase.ItemsDatas.ContainsKey(itemIdAttribute.Value), $"Unknown ItemId {itemIdAttribute.Value} in {typeof(Database.ItemDatabase).Name}.");
            ItemId = itemIdAttribute.Value;

            XAttribute quantityAttribute = hasItemElement.Attribute("MinQuantity");
            MinQuantity = quantityAttribute?.ValueToInt() ?? 1;
        }
    }


    public class DoesntHaveItemContextualConditionDatas : ContextualConditionDatas
    {
        public DoesntHaveItemContextualConditionDatas(XContainer container) : base(container)
        {
        }

        public string ItemId { get; private set; }

        public override void Deserialize(XContainer container)
        {
            XElement hasItemElement = container as XElement;

            XAttribute itemIdAttribute = hasItemElement.Attribute("Id");
            UnityEngine.Assertions.Assert.IsNotNull(itemIdAttribute, "Contextual Condition HasItem element needs an Id attribute.");
            UnityEngine.Assertions.Assert.IsTrue(Database.ItemDatabase.ItemsDatas.ContainsKey(itemIdAttribute.Value), $"Unknown ItemId {itemIdAttribute.Value} in {typeof(Database.ItemDatabase).Name}.");
            ItemId = itemIdAttribute.Value;
        }
    }
}