namespace RSLib.Framework.GameSettings
{
    using UnityEngine;

    public class GameSettingsManager : RSLib.Framework.Singleton<GameSettingsManager>
    {
        private const string XML_CONTAINER_NAME = "GameSettings";

        [SerializeField] protected bool _loadOnAwake = false;
        [SerializeField] protected bool _saveOnAwake = false;

        #region SHARED SETTINGS PROPERTIES
        public ConstrainCursor ConstrainCursor { get; protected set; }
        public MonitorIndex MonitorIndex { get; protected set; }
        public PixelPerfect PixelPerfect { get; protected set; }
        public RunInBackground RunInBackground { get; protected set; }
        public ScreenMode ScreenMode { get; protected set; }
        public ShakeAmount ShakeAmount { get; protected set; }
        public ShowTutorials ShowTutorials { get; protected set; }
        public StickDeadZone StickDeadZone { get; protected set; }
        public TargetFrameRate TargetFrameRate { get; protected set; }
        #endregion // SHARED SETTINGS PROPERTIES

        protected virtual string SavePath => System.IO.Path.Combine(UnityEngine.Application.persistentDataPath, "Save", "Settings.xml");

        public bool SaveXML()
        {
            Log("Saving game settings...", gameObject, true);

            try
            {
                System.Xml.Linq.XContainer container = new System.Xml.Linq.XElement(XML_CONTAINER_NAME);

                container.Add(ConstrainCursor.Save());
                container.Add(MonitorIndex.Save());
                container.Add(PixelPerfect.Save());
                container.Add(RunInBackground.Save());
                container.Add(ScreenMode.Save());
                container.Add(ShakeAmount.Save());
                container.Add(ShowTutorials.Save());
                container.Add(StickDeadZone.Save());
                container.Add(TargetFrameRate.Save());

                SaveCustomSettings(container);
                
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(SavePath);
                if (fileInfo.Directory != null && !fileInfo.Directory.Exists)
                    System.IO.Directory.CreateDirectory(fileInfo.DirectoryName);

                byte[] buffer;
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    using (System.Xml.XmlWriter xmlWriter = System.Xml.XmlWriter.Create(ms, new System.Xml.XmlWriterSettings { Indent = true, Encoding = System.Text.Encoding.UTF8 }))
                    {
                        System.Xml.Linq.XDocument saveDocument = new System.Xml.Linq.XDocument();
                        saveDocument.Add(container);
                        saveDocument.Save(xmlWriter);
                    }

                    buffer = ms.ToArray();
                }

                using (System.IO.MemoryStream ms = new System.IO.MemoryStream(buffer))
                {
                    using (System.IO.FileStream diskStream = System.IO.File.Open(SavePath, System.IO.FileMode.Create, System.IO.FileAccess.Write))
                    {
                        ms.CopyTo(diskStream);
                    }
                }
            }
            catch (System.Exception e)
            {
                LogError($"Could not save settings! Exception message:\n{e}", gameObject);
                return false;
            }

            Log("Settings saved successfully!", gameObject, true);
            return true;
        }

        public bool TryLoadXML()
        {
            Log("Loading game settings...", gameObject, true);

            if (!System.IO.File.Exists(SavePath))
            {
                Log($"Game settings save file does not exist (path:{SavePath}), aborting loading.", gameObject, true);
                return false;
            }

            try
            {
                System.Xml.Linq.XContainer container = System.Xml.Linq.XDocument.Parse(System.IO.File.ReadAllText(SavePath));
                System.Xml.Linq.XElement gameSettingsElement = container.Element(XML_CONTAINER_NAME);

                if (gameSettingsElement != null)
                {
                    ConstrainCursor = new ConstrainCursor(gameSettingsElement.Element(ConstrainCursor.SERIALIZATION_NAME));
                    MonitorIndex = new MonitorIndex(gameSettingsElement.Element(MonitorIndex.SERIALIZATION_NAME));
                    PixelPerfect = new PixelPerfect(gameSettingsElement.Element(PixelPerfect.SERIALIZATION_NAME));
                    RunInBackground = new RunInBackground(gameSettingsElement.Element(RunInBackground.SERIALIZATION_NAME));
                    ScreenMode = new ScreenMode(gameSettingsElement.Element(ScreenMode.SERIALIZATION_NAME));
                    ShakeAmount = new ShakeAmount(gameSettingsElement.Element(ShakeAmount.SERIALIZATION_NAME));
                    ShowTutorials = new ShowTutorials(gameSettingsElement.Element(ShowTutorials.SERIALIZATION_NAME));
                    StickDeadZone = new StickDeadZone(gameSettingsElement.Element(StickDeadZone.SERIALIZATION_NAME));
                    TargetFrameRate = new TargetFrameRate(gameSettingsElement.Element(TargetFrameRate.SERIALIZATION_NAME));

                    LoadCustomSettings(gameSettingsElement);
                }
            }
            catch (System.Exception e)
            {
                LogError($"Could not load settings! Exception message:\n{e}");
                return false;
            }

            Log("Settings loaded successfully!", gameObject, true);
            return true;
        }

        /// <summary>
        /// Can be overridden to save data that are specific to a project.
        /// </summary>
        /// <param name="container">Game settings save container.</param>
        protected virtual void SaveCustomSettings(System.Xml.Linq.XContainer container)
        {
        }
        
        /// <summary>
        /// Can be overridden to load data that are specific to a project.
        /// </summary>
        /// <param name="gameSettingsElement">Game settings save element.</param>
        protected virtual void LoadCustomSettings(System.Xml.Linq.XElement gameSettingsElement)
        {
            
        }

        /// <summary>
        /// Initializes game settings.
        /// Can be overridden to initialize data that are specific to a project, but do not forget to call base when doing so.
        /// </summary>
        public virtual void Init()
        {
            ConstrainCursor = new ConstrainCursor();
            MonitorIndex = new MonitorIndex();
            PixelPerfect = new PixelPerfect();
            RunInBackground = new RunInBackground();
            ScreenMode = new ScreenMode();
            ShakeAmount = new ShakeAmount();
            ShowTutorials = new ShowTutorials();
            StickDeadZone = new StickDeadZone();
            TargetFrameRate = new TargetFrameRate();
        }

        protected override void Awake()
        {
            base.Awake();
            if (!IsValid)
                return;

            if (_loadOnAwake)
                if (!TryLoadXML())
                    Init();   

            if (_saveOnAwake)
                SaveXML();

            RSLib.Debug.Console.DebugConsole.OverrideCommand("SettingsSave", "Saves settings.", () => SaveXML());
            RSLib.Debug.Console.DebugConsole.OverrideCommand("SettingsLoad", "Tries to load settings.", () => TryLoadXML());
            RSLib.Debug.Console.DebugConsole.OverrideCommand("SettingsLog", "Log settings values.", DebugLogSettings);
        }

        [ContextMenu("Save")]
        private void DebugSave()
        {
            SaveXML();
        }

        [ContextMenu("Load")]
        private void DebugLoad()
        {
            TryLoadXML();
        }
        
        [ContextMenu("Log Settings")]
        private void DebugLogSettings()
        {
            string log = DebugGetSettingsLog();
            
            Log(log, gameObject, true);
            
            if (RSLib.Debug.Console.DebugConsole.Instance.IsOpen)
                RSLib.Debug.Console.DebugConsole.LogExternal(log);
        }

        protected virtual string DebugGetSettingsLog()
        {
            const string format = "\n\r{0}: {1}";
            string log = "SETTINGS";

            log += string.Format(format, ConstrainCursor.SerializationName, ConstrainCursor.Value);
            log += string.Format(format, MonitorIndex.SerializationName, MonitorIndex.Value);
            log += string.Format(format, PixelPerfect.SerializationName, PixelPerfect.Value);
            log += string.Format(format, RunInBackground.SerializationName, RunInBackground.Value);
            log += string.Format(format, ScreenMode.SerializationName, ScreenMode.Value);
            log += string.Format(format, ShakeAmount.SerializationName, $"{ShakeAmount.Value * 100f}%");
            log += string.Format(format, ShowTutorials.SerializationName, ShowTutorials.Value);
            log += string.Format(format, StickDeadZone.SerializationName, StickDeadZone.Value);
            log += string.Format(format, TargetFrameRate.SerializationName, TargetFrameRate.Value == -1 ? "MAX" : $"{TargetFrameRate.Value}fps");

            return log;
        }
    }
}
