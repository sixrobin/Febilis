namespace Templar.Datas.Dialogue
{
    using RSLib.Extensions;
    using System.Linq;
    using System.Xml.Linq;

    public class DialogueDatas : Datas
    {
        public DialogueDatas(XContainer container) : base(container)
        {
        }

        public string Id { get; private set; }

        public IDialogueSequenceElementDatas[] SequenceElementsDatas { get; private set; }

        public override void Deserialize(XContainer container)
        {
            XElement dialogueElement = container as XElement;

            XAttribute idAttribute = dialogueElement.Attribute("Id");
            UnityEngine.Assertions.Assert.IsFalse(idAttribute.IsNullOrEmpty(), "Dialogue Id attribute is null or empty.");
            Id = idAttribute.Value;

            System.Collections.Generic.IEnumerable<XElement> dialogueSequenceElements = dialogueElement.Elements();
            SequenceElementsDatas = new IDialogueSequenceElementDatas[dialogueSequenceElements.Count()];

            int i = 0;
            foreach (XElement sentenceElement in dialogueSequenceElements)
            {
                if (sentenceElement.Name.LocalName == "Sentence")
                {
                    // We may want to create a custom class containing SentenceDatas, like DialogueSentenceDatas
                    // to override datas from the base sentence.

                    XAttribute sentenceIdAttribute = sentenceElement.Attribute("Id");
                    UnityEngine.Assertions.Assert.IsNotNull(sentenceIdAttribute, "Dialogue Sentence element needs an Id attribute.");
                    SequenceElementsDatas[i] = Database.DialogueDatabase.SentencesDatas[sentenceIdAttribute.Value];
                }
                else if (sentenceElement.Name.LocalName == "Pause")
                {
                    SequenceElementsDatas[i] = new DialoguePauseDatas(sentenceElement);
                }

                i++;
            }
        }
    }
}