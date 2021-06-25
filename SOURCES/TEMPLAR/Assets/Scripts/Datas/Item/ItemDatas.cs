namespace Templar.Datas.Item
{
    using RSLib.Extensions;
    using System.Xml.Linq;

    public class ItemDatas : Datas
    {
        public ItemDatas(XContainer container) : base(container)
        {
        }

        public string Id { get; private set; }

        public string Description { get; private set; }
        public string Type { get; private set; } // [TODO] Enum, with flag for QUEST ?
        public bool AlwaysShowQuantity { get; private set; }
        public bool AlwaysInInventory { get; private set; }

        public override void Deserialize(XContainer container)
        {
            XElement itemElement = container as XElement;

            XAttribute idAttribute = itemElement.Attribute("Id");
            UnityEngine.Assertions.Assert.IsFalse(idAttribute.IsNullOrEmpty(), "Item Id attribute is null or empty.");
            Id = idAttribute.Value;

            XElement typeElement = itemElement.Element("Type");
            UnityEngine.Assertions.Assert.IsFalse(typeElement.IsNullOrEmpty(), "Item Type element is null or empty.");
            Type = typeElement.Value;

            XElement descriptionElement = itemElement.Element("Description");
            UnityEngine.Assertions.Assert.IsFalse(descriptionElement.IsNullOrEmpty(), "Item Description element is null or empty.");
            Description = descriptionElement.Value;

            AlwaysShowQuantity = itemElement.Element("AlwaysShowQuantity") != null;
            AlwaysInInventory = itemElement.Element("AlwaysInInventory") != null;
        }
    }
}