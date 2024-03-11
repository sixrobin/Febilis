namespace Templar.Datas.Dialogue.DialogueStructure
{
    using System.Linq;
    using RSLib.Extensions;
    using System.Xml.Linq;
    
    public class DialoguesStructureDatas : Datas
    {
        public DialoguesStructureDatas(XContainer container) : base(container)
        {
        }

        public string Id { get; private set; }

        public System.Collections.Generic.Dictionary<string, int> SoldItems { get; private set; }
        public bool HasItemsToSoldLeft => SoldItems != null && SoldItems.Any(o => o.Value > 0);
        
        public DialoguesStructureDialogueDatas[] Dialogues { get; private set; }

        public override void Deserialize(XContainer container)
        {
            XElement dialogueStructureElement = container as XElement;

            XAttribute idAttribute = dialogueStructureElement.Attribute("Id");
            UnityEngine.Assertions.Assert.IsFalse(idAttribute.IsNullOrEmpty(), "DialogueStructure Id attribute is null or empty.");
            Id = idAttribute.Value;

            XElement soldItemsElement = dialogueStructureElement.Element("SoldItems");
            if (soldItemsElement != null)
            {
                SoldItems = new System.Collections.Generic.Dictionary<string, int>();
                foreach (XElement soldItemIdElement in soldItemsElement.Elements("ItemId"))
                {
                    string itemId = soldItemIdElement.Value;
                    
                    UnityEngine.Assertions.Assert.IsTrue(
                        Database.ItemDatabase.ItemsDatas.ContainsKey(itemId),
                        $"DialogueStructure {Id} contains an item to sold with Id {itemId} that is not known in {nameof(Database.ItemDatabase)}!");
                    
                    if (SoldItems.ContainsKey(itemId))
                        SoldItems[itemId]++;
                    else
                        SoldItems.Add(itemId, 1);
                }
            }
            
            System.Collections.Generic.List<DialoguesStructureDialogueDatas> dialogues = new System.Collections.Generic.List<DialoguesStructureDialogueDatas>();
            foreach (XElement dialogueElement in dialogueStructureElement.Elements("Dialogue"))
            {
                DialoguesStructureDialogueDatas dialogue = new DialoguesStructureDialogueDatas(dialogueElement);
                dialogues.Add(dialogue);
            }

            Dialogues = dialogues.ToArray();
        }
    }
}