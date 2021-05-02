namespace Templar.Datas.Dialogue
{
    using System.Xml.Linq;
    using UnityEngine;

    public partial class DialogueDatabase : RSLib.Framework.ConsoleProSingleton<DialogueDatabase>
    {
        public const string PORTRAIT_PREFIX = "Dialogue-Portrait_";

        [SerializeField] private TextAsset[] _dialoguesDatas = null;
        [SerializeField] private TextAsset _speakersDisplayDatas = null;
        [SerializeField] private Sprite[] _portraits = null;
        [SerializeField] private Sprite _defaultPortrait = null;

#if UNITY_EDITOR
        [Header("DEBUG")]
        [SerializeField] private string portraitsAssetsRootPath = "Assets/Textures/UI/Dialogue/Portraits";
#endif

        public static System.Collections.Generic.Dictionary<string, DialogueDatas> DialoguesDatas { get; private set; }
        public static System.Collections.Generic.Dictionary<string, SentenceDatas> SentencesDatas { get; private set; }

        public static System.Collections.Generic.Dictionary<string, SpeakerDisplayDatas> SpeakersDisplayDatas { get; private set; }

        public static System.Collections.Generic.Dictionary<string, Sprite> Portraits { get; private set; }
        public static Sprite DefaultPortrait => Instance._defaultPortrait;

        public static Sprite GetPortraitOrUseDefault(SentenceDatas sentenceDatas)
        {
            string portraitId = sentenceDatas.OverridePortraitId ?? sentenceDatas.SpeakerId;

            if (!Portraits.TryGetValue($"{PORTRAIT_PREFIX}{portraitId}", out Sprite portrait))
            {
                Instance.LogWarning($"Portrait not found in {Instance.GetType().Name} for Id {portraitId}, using default one.");
                return DefaultPortrait;
            }

            return portrait;
        }

        public static string GetSpeakerDisplayName(SentenceDatas sentenceDatas)
        {
            if (!string.IsNullOrEmpty(sentenceDatas.OverrideDisplayName))
                return sentenceDatas.OverrideDisplayName;

            if (!SpeakersDisplayDatas.TryGetValue(sentenceDatas.SpeakerId, out SpeakerDisplayDatas speakerDisplayDatas))
            {
                Instance.LogError($"Speaker Id {sentenceDatas.SpeakerId} was not found in {Instance.GetType().Name} speakers display datas, returning Id.");
                return sentenceDatas.SpeakerId;
            }

            return speakerDisplayDatas.DisplayName;
        }

        public static PortraitAnchor GetSpeakerPortraitAnchor(SentenceDatas sentenceDatas)
        {
            if (sentenceDatas.OverridePortraitAnchor != PortraitAnchor.NONE)
                return sentenceDatas.OverridePortraitAnchor;

            if (!SpeakersDisplayDatas.TryGetValue(sentenceDatas.SpeakerId, out SpeakerDisplayDatas speakerDisplayDatas))
            {
                Instance.LogError($"Speaker Id {sentenceDatas.SpeakerId} was not found in {Instance.GetType().Name} speakers display datas, returning PortraitAnchor.TOP_RIGHT.");
                return PortraitAnchor.TOP_RIGHT;
            }

            return speakerDisplayDatas.PortraitAnchor;
        }

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
                foreach (XElement sentenceElement in allFilesElements[i].Elements("SentenceDatas"))
                {
                    SentenceDatas sentenceDatas = new SentenceDatas(sentenceElement);
                    SentencesDatas.Add(sentenceDatas.Id, sentenceDatas);
                }
            }

            for (int i = allFilesElements.Count - 1; i >= 0; --i)
            {
                foreach (XElement dialogueElement in allFilesElements[i].Elements("DialogueDatas"))
                {
                    DialogueDatas dialogueDatas = new DialogueDatas(dialogueElement);
                    DialoguesDatas.Add(dialogueDatas.Id, dialogueDatas);
                }
            }

            Log($"Deserialized {SentencesDatas.Count} sentences datas and {DialoguesDatas.Count} dialogues datas.");

            DeserializeSpeakersDatas();
        }

        private void DeserializeSpeakersDatas()
        {
            SpeakersDisplayDatas = new System.Collections.Generic.Dictionary<string, SpeakerDisplayDatas>();

            XDocument speakersDisplayDatasDoc = XDocument.Parse(_speakersDisplayDatas.text, LoadOptions.SetBaseUri);

            XElement speakersDisplayElement = speakersDisplayDatasDoc.Element("SpeakersDisplayDatas");
            foreach (XElement speakerDisplayElement in speakersDisplayElement.Elements("SpeakerDisplayDatas"))
            {
                SpeakerDisplayDatas speakerDisplayDatas = new SpeakerDisplayDatas(speakerDisplayElement);
                SpeakersDisplayDatas.Add(speakerDisplayDatas.Id, speakerDisplayDatas);
            }
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
        [ContextMenu("Get Portraits from Assets")]
        private void GetPortraitsFromAssets()
        {
            Instance.Log($"Getting portraits in folder {portraitsAssetsRootPath}...");

            System.Collections.Generic.List<Sprite> portraits = new System.Collections.Generic.List<Sprite>();

            foreach (Sprite portrait in RSLib.EditorUtilities.AssetDatabaseUtilities.GetAllAssetsAtFolderPath<Sprite>(portraitsAssetsRootPath))
                portraits.Add(portrait);

            Instance.Log($"Found {portraits.Count} portraits.");
            _portraits = portraits.ToArray();

            RSLib.EditorUtilities.SceneManagerUtilities.SetCurrentSceneDirty();
        }
#endif

        protected override void Awake()
        {
            base.Awake();
            Deserialize();
            GeneratePortraitsDictionary();
        }
    }
}