namespace Templar
{
    using System.Collections.Generic;
    using System.Linq;

    public class Localizer : RSLib.Framework.ConsoleProSingleton<Localizer>
    {
        [UnityEngine.SerializeField] private UnityEngine.TextAsset _localizationCsv = null;

        private Dictionary<string, Dictionary<string, string>> _entries;
        
        /// <summary>
        /// All languages handled in loaded CSV file.
        /// </summary>
        public string[] Languages { get; private set; }
        
        /// <summary>
        /// Currently selected language.
        /// </summary>
        public string Language { get; private set; }

        private static Dictionary<string, string> LanguageEntries => Instance._entries[Instance.Language];

        /// <summary>
        /// Gets the localized key for current language, and returns the key itself if it has not been found.
        /// </summary>
        /// <param name="key">Key to localize.</param>
        /// <returns>Localized key if it exists, else the key itself.</returns>
        public static string Get(string key)
        {
            if (LanguageEntries.TryGetValue(key, out string entry))
                return entry;
            
            Instance.LogWarning($"Key {key} is not in language {Instance.Language}!");
            return key;
        }
        
        /// <summary>
        /// Checks if the key exists for current language, and localizes it if so.
        /// </summary>
        /// <param name="key">Key to localize.</param>
        /// <param name="entry">Localized entry if key exists.</param>
        /// <returns>True if the key exists, else false.</returns>
        public static bool TryGet(string key, out string entry)
        {
            return LanguageEntries.TryGetValue(key, out entry);
        }
        
        /// <summary>
        /// Sets the current language, based on its index in the handled languages list.
        /// </summary>
        /// <param name="languageIndex">Selected language index.</param>
        public static void SetCurrentLanguage(int languageIndex)
        {
            if (languageIndex > Instance._entries.Count - 1)
            {
                Instance.LogWarning($"Tried to set language index to {languageIndex} but only {Instance._entries.Count} languages are known!");
                return;
            }
            
            SetCurrentLanguage(Instance._entries.ElementAt(languageIndex).Key);
        }
        
        /// <summary>
        /// Sets the current language, based on its name.
        /// </summary>
        /// <param name="languageName">Selected language name.</param>
        public static void SetCurrentLanguage(string languageName)
        {
            if (!Instance._entries.ContainsKey(languageName))
            {
                Instance.LogWarning($"Tried to set language to {languageName} but it has not been found!");
                return;
            }

            Instance.Log($"Setting language to {languageName}.");
            Instance.Language = languageName;
        }
        
        /// <summary>
        /// Loads a CSV file and parses its data to generate localization dictionaries.
        /// </summary>
        /// <param name="csvFile">CSV localization file.</param>
        private static void LoadCSV(UnityEngine.TextAsset csvFile)
        {
            string[,] grid = CSVReader.SplitCSVGrid(csvFile.text);
            
            // Initialize languages.
            Instance._entries = new Dictionary<string, Dictionary<string, string>>();
            List<string> languages = new List<string>();
            for (int x = 1; x < grid.GetLength(0); ++x) // Start at 1 to avoid keys column.
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
        }
    }
}
