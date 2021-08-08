namespace Templar.Datas.Dialogue
{
    using RSLib.Extensions;
    using System.Xml.Linq;

    public class DialogueAddItemDatas : Datas, IDialogueSequenceElementDatas
    {
        public DialogueAddItemDatas(XContainer container) : base(container)
        {
        }

        public string ItemId { get; private set; }
        public int Quantity { get; private set; }

        public override void Deserialize(XContainer container)
        {
            XElement addItemElement = container as XElement;

            XAttribute itemIdAttribute = addItemElement.Attribute("Id");
            UnityEngine.Assertions.Assert.IsNotNull(itemIdAttribute, "Dialogue AddItem element needs an Id attribute.");
            ItemId = itemIdAttribute.Value;

            XAttribute quantityAttribute = addItemElement.Attribute("Quantity");
            Quantity = quantityAttribute?.ValueToInt() ?? 1;
        }
    }
}