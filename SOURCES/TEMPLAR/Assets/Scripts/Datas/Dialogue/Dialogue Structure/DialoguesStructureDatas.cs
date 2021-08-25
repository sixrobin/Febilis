namespace Templar.Datas.Dialogue.DialogueStructure
{
    using RSLib.Extensions;
    using System.Xml.Linq;
    
    public class DialoguesStructureDatas : Datas
    {
        public DialoguesStructureDatas(XContainer container) : base(container)
        {
        }

        public string Id { get; private set; }

        public DialoguesStructureDialogueDatas[] Dialogues { get; private set; }

        public override void Deserialize(XContainer container)
        {
            XElement dialogueStructureElement = container as XElement;

            XAttribute idAttribute = dialogueStructureElement.Attribute("Id");
            UnityEngine.Assertions.Assert.IsFalse(idAttribute.IsNullOrEmpty(), "DialogueStructure Id attribute is null or empty.");
            Id = idAttribute.Value;

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