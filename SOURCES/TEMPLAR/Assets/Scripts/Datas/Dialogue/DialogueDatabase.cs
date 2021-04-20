namespace Templar.Datas.Dialogue
{
    using System.Xml.Linq;
    using UnityEngine;

    public partial class DialogueDatabase : RSLib.Framework.ConsoleProSingleton<DialogueDatabase>
    {
        [SerializeField] private TextAsset[] _dialoguesDatas = null;

        public static System.Collections.Generic.Dictionary<string, DialogueDatas> DialoguesDatas { get; private set; }
        public static System.Collections.Generic.Dictionary<string, SentenceDatas> SentencesDatas { get; private set; }

        private void Deserialize()
        {
            SentencesDatas = new System.Collections.Generic.Dictionary<string, SentenceDatas>();
            DialoguesDatas = new System.Collections.Generic.Dictionary<string, DialogueDatas>();

            System.Collections.Generic.List<XElement> allFilesElements = new System.Collections.Generic.List<XElement>();

            // Gather all documents main element in a list.
            for (int i = _dialoguesDatas.Length - 1; i >= 0; --i)
            {
                XDocument dialoguesDatasDoc = XDocument.Parse(_dialoguesDatas[i].text, LoadOptions.SetBaseUri);
                allFilesElements.Add(dialoguesDatasDoc.Element("DialoguesDatas"));
            }

            for (int i = allFilesElements.Count - 1; i >= 0; --i)
            {
                foreach (XElement sentenceElement in allFilesElements[i].Elements("Sentence"))
                {
                    SentenceDatas sentenceDatas = new SentenceDatas(sentenceElement);
                    SentencesDatas.Add(sentenceDatas.Id, sentenceDatas);
                }
            }

            for (int i = allFilesElements.Count - 1; i >= 0; --i)
            {
                foreach (XElement dialogueElement in allFilesElements[i].Elements("Dialogue"))
                {
                    DialogueDatas dialogueDatas = new DialogueDatas(dialogueElement);
                    DialoguesDatas.Add(dialogueDatas.Id, dialogueDatas);
                }
            }

            Log($"Deserialized {SentencesDatas.Count} sentences datas and {DialoguesDatas.Count} dialogues datas.");
        }

        protected override void Awake()
        {
            base.Awake();
            Deserialize();
        }
    }
}