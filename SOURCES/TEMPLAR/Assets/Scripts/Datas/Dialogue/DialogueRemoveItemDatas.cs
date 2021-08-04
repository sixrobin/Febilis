namespace Templar.Datas.Dialogue
{
    using RSLib.Extensions;
    using System.Xml.Linq;

    public class DialogueRemoveItemDatas : Datas, IDialogueSequenceElementDatas
    {
        public DialogueRemoveItemDatas(XContainer container) : base(container)
        {
        }

        public string ItemId { get; private set; }
        public int Quantity { get; private set; }

        public override void Deserialize(XContainer container)
        {
            XElement removeItemElement = container as XElement;

            XAttribute itemIdAttribute = removeItemElement.Attribute("Id");
            UnityEngine.Assertions.Assert.IsNotNull(itemIdAttribute, "Dialogue RemoveItem element needs an Id attribute.");
            ItemId = itemIdAttribute.Value;

            XAttribute quantityAttribute = removeItemElement.Attribute("Quantity");
            Quantity = quantityAttribute?.ValueToInt() ?? 1;
        }
    }
}