namespace Templar.Datas.Dialogue
{
    using RSLib.Extensions;
    using System.Linq;
    using System.Xml.Linq;

    public class DialogueDatas : Datas
    {
        private const string ELEMENT_NAME_SENTENCE = "Sentence";
        private const string ELEMENT_NAME_PAUSE = "Pause";
        private const string ELEMENT_NAME_ADD_ITEM = "AddItem";
        private const string ELEMENT_NAME_REMOVE_ITEM = "RemoveItem";

        public DialogueDatas(XContainer container) : base(container)
        {
        }

        public string Id { get; private set; }

        public IDialogueSequenceElementDatas[] SequenceElementsDatas { get; private set; }

        public bool InvertPortraitsAnchors { get; private set; }

        public override void Deserialize(XContainer container)
        {
            XElement dialogueElement = container as XElement;

            XAttribute idAttribute = dialogueElement.Attribute("Id");
            UnityEngine.Assertions.Assert.IsFalse(idAttribute.IsNullOrEmpty(), "Dialogue Id attribute is null or empty.");
            Id = idAttribute.Value;

            InvertPortraitsAnchors = dialogueElement.Element("InvertPortraitsAnchors") != null;

            XElement sequenceElement = dialogueElement.Element("Sequence");
            UnityEngine.Assertions.Assert.IsNotNull(sequenceElement, "Dialogue needs a Sequence element.");

            System.Collections.Generic.IEnumerable<XElement> dialogueSequenceElements = sequenceElement.Elements();
            SequenceElementsDatas = new IDialogueSequenceElementDatas[dialogueSequenceElements.Count()];

            int i = 0;
            foreach (XElement sentenceElement in dialogueSequenceElements)
            {
                if (sentenceElement.Name.LocalName == ELEMENT_NAME_SENTENCE)
                {
                    // We may want to create a custom class containing SentenceDatas, like DialogueSentenceDatas
                    // to override datas from the base sentence.

                    XAttribute sentenceIdAttribute = sentenceElement.Attribute("Id");
                    UnityEngine.Assertions.Assert.IsNotNull(sentenceIdAttribute, "Dialogue Sentence element needs an Id attribute.");
                    SequenceElementsDatas[i] = Database.DialogueDatabase.SentencesDatas[sentenceIdAttribute.Value];
                }
                else if (sentenceElement.Name.LocalName == ELEMENT_NAME_PAUSE)
                {
                    SequenceElementsDatas[i] = new DialoguePauseDatas(sentenceElement);
                }
                else if (sentenceElement.Name.LocalName == ELEMENT_NAME_ADD_ITEM)
                {
                    SequenceElementsDatas[i] = new DialogueAddItemDatas(sentenceElement);
                }
                else if (sentenceElement.Name.LocalName == ELEMENT_NAME_REMOVE_ITEM)
                {
                    SequenceElementsDatas[i] = new DialogueRemoveItemDatas(sentenceElement);
                }

                i++;
            }
        }
    }
}