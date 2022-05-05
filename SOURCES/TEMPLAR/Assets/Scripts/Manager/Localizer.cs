namespace Templar
{
    using System.Collections.Generic;
    using System.Linq;

    public class Localizer : RSLib.Framework.ConsoleProSingleton<Localizer>
    {
        [UnityEngine.SerializeField] private UnityEngine.TextAsset _localizationCsv = null;

        private Dictionary<string, Dictionary<string, string>> _entries;
        
        public string[] Languages { get; private set; }
        
        public string Language { get; private set; }

        private static Dictionary<string, string> LanguageEntries => Instance._entries[Instance.Language];

        public static string Get(string key)
        {
            if (LanguageEntries.TryGetValue(key, out string entry))
                return entry;
            
            Instance.LogWarning($"Key {key} is not in language {Instance.Language}!");
            return key;
        }
        
        public static bool TryGet(string key, out string entry)
        {
            return LanguageEntries.TryGetValue(key, out entry);
        }
        
        public static void SetCurrentLanguage(int languageIndex)
        {
            if (languageIndex > Instance._entries.Count - 1)
            {
                Instance.LogWarning($"Tried to set language index to {languageIndex} but only {Instance._entries.Count} languages are known!");
                return;
            }
            
            SetCurrentLanguage(Instance._entries.ElementAt(languageIndex).Key);
        }
        
        public static void SetCurrentLanguage(string language)
        {
            if (!Instance._entries.ContainsKey(language))
            {
                Instance.LogWarning($"Tried to set language to {language} but it has not been found!");
                return;
            }

            Instance.Log($"Setting language to {language}.");
            Instance.Language = language;
        }
        
        private static void LoadCSV(UnityEngine.TextAsset csvFile)
        {
            string[,] grid = CSVReader.SplitCSVGrid(csvFile.text);
            
            // Initialize languages.
            Instance._entries = new Dictionary<string, Dictionary<string, string>>();
            List<string> languages = new List<string>();
            for (int x = 1; x < grid.GetLength(0); ++x)
            {
                string language = grid[x, 0];
                if (string.IsNullOrEmpty(language))
                    continue;
                
                Instance._entries.Add(language, new Dictionary<string, string>());
                languages.Add(language);
            }

            Instance.Languages = languages.ToArray();

            // Initialize entries.
            for (int y = 1; y < grid.GetLength(1); ++y)
            {
                string key = grid[0, y];
                if (string.IsNullOrEmpty(key))
                    continue;

                for (int x = 1; x < grid.GetLength(0); ++x)
                {
                    string language = grid[x, 0];
                    if (string.IsNullOrEmpty(language))
                        continue;
                    
                    string entry = grid[x, y];
                    Instance._entries[language].Add(key, entry);
                }
            }
        }
        
        protected override void Awake()
        {
            base.Awake();
            if (!IsValid)
                return;
            
            LoadCSV(_localizationCsv);
            
            if (Instance._entries.Count > 0)
                Instance.Language = Instance._entries.ElementAt(0).Key;
            
            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command<string>("localizeKey", "Localizes a given key.", (key) => Log(Get(key), forceVerbose: true)));
            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command<string>("setLanguage", "Set language.", SetCurrentLanguage));
            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command<int>("setLanguageIndex", "Set language index.", SetCurrentLanguage));
        }
    }
}
