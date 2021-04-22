namespace Templar.Datas.Dialogue
{
    using System.Xml.Linq;
    using UnityEngine;

    public partial class DialogueDatabase : RSLib.Framework.ConsoleProSingleton<DialogueDatabase>
    {
        public const string PORTRAIT_PREFIX = "Dialogue-Portrait_";

        [SerializeField] private TextAsset[] _dialoguesDatas = null;
        [SerializeField] private Sprite[] _portraits = null;
        [SerializeField] private Sprite _defaultPortrait = null;

#if UNITY_EDITOR
        [Header("DEBUG")]
        [SerializeField] private string portraitsAssetsRootPath = "Assets/Textures/UI/Dialogue/Portraits";
#endif

        public static System.Collections.Generic.Dictionary<string, DialogueDatas> DialoguesDatas { get; private set; }
        public static System.Collections.Generic.Dictionary<string, SentenceDatas> SentencesDatas { get; private set; }

        public static System.Collections.Generic.Dictionary<string, Sprite> Portraits { get; private set; }
        public static Sprite DefaultPortrait => Instance._defaultPortrait;

        public static Sprite GetPortrait(string speakerId)
        {
            if (!Portraits.TryGetValue($"{PORTRAIT_PREFIX}{speakerId}", out Sprite portrait))
            {
                Instance.LogError($"Portrait not found in {Instance.GetType().Name} for Speaker Id {speakerId}.");
                return null;
            }

            return portrait;
        }

        public static Sprite GetPortraitOrUseDefault(string speakerId)
        {
            if (!Portraits.TryGetValue($"{PORTRAIT_PREFIX}{speakerId}", out Sprite portrait))
            {
                Instance.LogWarning($"Portrait not found in {Instance.GetType().Name} for Speaker Id {speakerId}, using default one.");
                return DefaultPortrait;
            }

            return portrait;
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