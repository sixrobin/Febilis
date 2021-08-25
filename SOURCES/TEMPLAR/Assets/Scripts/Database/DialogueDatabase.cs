namespace Templar.Database
{
    using System.Xml.Linq;
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    public class DialogueDatabase : RSLib.Framework.ConsoleProSingleton<DialogueDatabase>, IDatabase
    {
        public const string PORTRAIT_PREFIX = "Dialogue-Portrait_";

        [SerializeField] private TextAsset[] _dialoguesDatas = null;
        [SerializeField] private TextAsset[] _dialoguesStructuresDatas = null;
        [SerializeField] private TextAsset _speakersDisplayDatas = null;
        [SerializeField] private Sprite[] _portraits = null;
        [SerializeField] private Sprite _defaultPortrait = null;

#if UNITY_EDITOR
        [SerializeField] private string portraitsAssetsRootPath = "Assets/Textures/UI/Dialogue/Portraits";
#endif

        public static System.Collections.Generic.Dictionary<string, Datas.Dialogue.DialogueStructure.DialoguesStructureDatas> DialoguesStructuresDatas { get; private set; }
        public static System.Collections.Generic.Dictionary<string, Datas.Dialogue.DialogueDatas> DialoguesDatas { get; private set; }
        public static System.Collections.Generic.Dictionary<string, Datas.Dialogue.SentenceDatas> SentencesDatas { get; private set; }
        
        public static System.Collections.Generic.Dictionary<string, Datas.Dialogue.SpeakerDisplayDatas> SpeakersDisplayDatas { get; private set; }

        public static System.Collections.Generic.Dictionary<string, Sprite> Portraits { get; private set; }
        public static Sprite DefaultPortrait => Instance._defaultPortrait;

        void IDatabase.Load()
        {
            DeserializeDialoguesStructuresDatas();
            DeserializeDialoguesDatas();
            DeserializeSpeakersDisplayDatas();
            GeneratePortraitsDictionary();
        }

        public static Sprite GetPortraitOrUseDefault(Datas.Dialogue.SentenceDatas sentenceDatas)
        {
            string portraitId = sentenceDatas.OverridePortraitId ?? sentenceDatas.SpeakerId;

            if (!Portraits.TryGetValue($"{PORTRAIT_PREFIX}{portraitId}", out Sprite portrait))
            {
                Instance.LogWarning($"Portrait not found in {Instance.GetType().Name} for Id {portraitId}, using default one.");
                return DefaultPortrait;
            }

            return portrait;
        }

        public static string GetSpeakerDisplayName(Datas.Dialogue.SentenceDatas sentenceDatas)
        {
            if (!string.IsNullOrEmpty(sentenceDatas.OverrideDisplayName))
                return sentenceDatas.OverrideDisplayName;

            if (!SpeakersDisplayDatas.TryGetValue(sentenceDatas.SpeakerId, out Datas.Dialogue.SpeakerDisplayDatas speakerDisplayDatas))
            {
                Instance.LogError($"Speaker Id {sentenceDatas.SpeakerId} was not found in {Instance.GetType().Name} speakers display datas, returning Id.");
                return sentenceDatas.SpeakerId;
            }

            return speakerDisplayDatas.DisplayName;
        }

        public static Datas.Dialogue.PortraitAnchor GetSpeakerPortraitAnchor(Datas.Dialogue.SentenceDatas sentenceDatas)
        {
            if (sentenceDatas.OverridePortraitAnchor != Datas.Dialogue.PortraitAnchor.NONE)
                return sentenceDatas.OverridePortraitAnchor;

            if (!SpeakersDisplayDatas.TryGetValue(sentenceDatas.SpeakerId, out Datas.Dialogue.SpeakerDisplayDatas speakerDisplayDatas))
            {
                Instance.LogError($"Speaker Id {sentenceDatas.SpeakerId} was not found in {Instance.GetType().Name} speakers display datas, returning PortraitAnchor.TOP_RIGHT.");
                return Datas.Dialogue.PortraitAnchor.TOP_RIGHT;
            }

            return speakerDisplayDatas.PortraitAnchor;
        }

        private void DeserializeDialoguesDatas()
        {
            SentencesDatas = new System.Collections.Generic.Dictionary<string, Datas.Dialogue.SentenceDatas>();
            DialoguesDatas = new System.Collections.Generic.Dictionary<string, Datas.Dialogue.DialogueDatas>();

            System.Collections.Generic.List<XElement> allFilesElements = new System.Collections.Generic.List<XElement>();

            // Gather all documents main element in a list.
            for (int i = _dialoguesDatas.Length - 1; i >= 0; --i)
            {
                XDocument dialoguesDatasDoc = XDocument.Parse(_dialoguesDatas[i].text, LoadOptions.SetBaseUri);
                allFilesElements.Add(dialoguesDatasDoc.Element("DialoguesDatas"));
            }

            for (int i = allFilesElements.Count - 1; i >= 0; --i)
            {
                foreach (XElement sentenceElement in allFilesElements[i].Elements("SentenceDatas"))
                {
                    Datas.Dialogue.SentenceDatas sentenceDatas = new Datas.Dialogue.SentenceDatas(sentenceElement);
                    SentencesDatas.Add(sentenceDatas.Id, sentenceDatas);
                }
            }

            for (int i = allFilesElements.Count - 1; i >= 0; --i)
            {
                foreach (XElement dialogueElement in allFilesElements[i].Elements("DialogueDatas"))
                {
                    Datas.Dialogue.DialogueDatas dialogueDatas = new Datas.Dialogue.DialogueDatas(dialogueElement);
                    DialoguesDatas.Add(dialogueDatas.Id, dialogueDatas);
                }
            }

            Log($"Deserialized {SentencesDatas.Count} sentences datas and {DialoguesDatas.Count} dialogues datas.");
        }

        private void DeserializeDialoguesStructuresDatas()
        {
            DialoguesStructuresDatas = new System.Collections.Generic.Dictionary<string, Datas.Dialogue.DialogueStructure.DialoguesStructureDatas>();

            System.Collections.Generic.List<XElement> allFilesElements = new System.Collections.Generic.List<XElement>();

            // Gather all documents main element in a list.
            for (int i = _dialoguesStructuresDatas.Length - 1; i >= 0; --i)
            {
                XDocument dialoguesStructuresDatasDoc = XDocument.Parse(_dialoguesStructuresDatas[i].text, LoadOptions.SetBaseUri);
                allFilesElements.Add(dialoguesStructuresDatasDoc.Element("DialoguesStructuresDatas"));
            }

            for (int i = allFilesElements.Count - 1; i >= 0; --i)
            {
                foreach (XElement dialoguesStructureElement in allFilesElements[i].Elements("DialoguesStructureDatas"))
                {
                    Datas.Dialogue.DialogueStructure.DialoguesStructureDatas dialoguesStructureDatas = new Datas.Dialogue.DialogueStructure.DialoguesStructureDatas(dialoguesStructureElement);
                    DialoguesStructuresDatas.Add(dialoguesStructureDatas.Id, dialoguesStructureDatas);
                }
            }

            Log($"Deserialized {DialoguesStructuresDatas.Count} dialogues structures datas.");
        }

        private void DeserializeSpeakersDisplayDatas()
        {
            SpeakersDisplayDatas = new System.Collections.Generic.Dictionary<string, Datas.Dialogue.SpeakerDisplayDatas>();

            XDocument speakersDisplayDatasDoc = XDocument.Parse(_speakersDisplayDatas.text, LoadOptions.SetBaseUri);

            XElement speakersDisplayElement = speakersDisplayDatasDoc.Element("SpeakersDisplayDatas");
            foreach (XElement speakerDisplayElement in speakersDisplayElement.Elements("SpeakerDisplayDatas"))
            {
                Datas.Dialogue.SpeakerDisplayDatas speakerDisplayDatas = new Datas.Dialogue.SpeakerDisplayDatas(speakerDisplayElement);
                SpeakersDisplayDatas.Add(speakerDisplayDatas.Id, speakerDisplayDatas);
            }

            Log($"Deserialized {SpeakersDisplayDatas.Count} speakers display datas.");
        }

        private void GeneratePortraitsDictionary()
        {
            Portraits = new System.Collections.Generic.Dictionary<string, Sprite>();

            for (int i = _portraits.Length - 1; i >= 0; --i)
            {
                UnityEngine.Assertions.Assert.IsFalse(
                    Portraits.ContainsKey(_portraits[i].name),
                    $"Portrait \"{_portraits[i].name}\" has already been registered in {GetType().Name}.");

                Portraits.Add(_portraits[i].name, _portraits[i]);
            }

            Log($"Generated {Portraits.Count} dialogue portraits.");
        }

#if UNITY_EDITOR
        public void GetPortraitsFromAssets()
        {
            Instance.Log($"Getting portraits in folder {portraitsAssetsRootPath}...", true);

            System.Collections.Generic.List<Sprite> portraits = new System.Collections.Generic.List<Sprite>();

            foreach (Sprite portrait in RSLib.EditorUtilities.AssetDatabaseUtilities.GetAllAssetsAtFolderPath<Sprite>(portraitsAssetsRootPath))
                portraits.Add(portrait);

            Instance.Log($"Found {portraits.Count} portraits.", true);
            _portraits = portraits.ToArray();

            RSLib.EditorUtilities.SceneManagerUtilities.SetCurrentSceneDirty();
            RSLib.EditorUtilities.PrefabEditorUtilities.SetCurrentPrefabStageDirty();
        }
#endif
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(DialogueDatabase))]
    public class DialogueDatabaseEditor : RSLib.EditorUtilities.ButtonProviderEditor<DialogueDatabase>
    {
        protected override void DrawButtons()
        {
            DrawButton("Get Portraits from Assets", Obj.GetPortraitsFromAssets);
        }
    }
#endif
}