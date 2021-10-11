namespace Templar.Manager
{
    using System.Xml;
    using System.Xml.Linq;

    public class SettingsManager : RSLib.Framework.ConsoleProSingleton<SettingsManager>
    {
        public static Settings.AxisDeadZone AxisDeadZone { get; private set; }
        public static Settings.ConstrainCursor ConstrainCursor { get; private set; }
        public static Settings.Fullscreen Fullscreen { get; private set; }
        public static Settings.MonitorIndex MonitorIndex { get; private set; }
        public static Settings.PixelPerfect PixelPerfect { get; private set; }
        public static Settings.RunInBackground RunInBackground { get; private set; }
        public static Settings.ScreenSize ScreenSize { get; private set; }
        public static Settings.ShakeAmount ShakeAmount { get; private set; }
        public static Settings.TargetFrameRate TargetFrameRate { get; private set; }

        private static string SettingsSavePath => $"{UnityEngine.Application.persistentDataPath}/Save/Settings.xml";
        private static string InputsSavePath => $"{UnityEngine.Application.persistentDataPath}/Save/Inputs.xml";

        public static void Save()
        {
            Instance.Log("Saving settings...", Instance.gameObject, true);

            try
            {
                XContainer container = new XElement("SettingsSave");

                container.Add(AxisDeadZone.Save());
                container.Add(ConstrainCursor.Save());
                container.Add(Fullscreen.Save());
                container.Add(MonitorIndex.Save());
                container.Add(PixelPerfect.Save());
                container.Add(RunInBackground.Save());
                container.Add(ScreenSize.Save());
                container.Add(ShakeAmount.Save());
                container.Add(TargetFrameRate.Save());

                System.IO.FileInfo fileInfo = new System.IO.FileInfo(SettingsSavePath);
                if (!fileInfo.Directory.Exists)
                    System.IO.Directory.CreateDirectory(fileInfo.DirectoryName);

                byte[] buffer;
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    using (XmlWriter xmlWriter = XmlWriter.Create(ms, new XmlWriterSettings() { Indent = true, Encoding = System.Text.Encoding.UTF8 }))
                    {
                        XDocument saveDocument = new XDocument();
                        saveDocument.Add(container);
                        saveDocument.Save(xmlWriter);
                    }

                    buffer = ms.ToArray();
                }

                using (System.IO.MemoryStream ms = new System.IO.MemoryStream(buffer))
                {
                    using (System.IO.FileStream diskStream = System.IO.File.Open(SettingsSavePath, System.IO.FileMode.Create, System.IO.FileAccess.Write))
                    {
                        ms.CopyTo(diskStream);
                    }
                }
            }
            catch (System.Exception e)
            {
                Instance.LogError($"Could not save settings ! Exception message:\n{e}", Instance.gameObject);
                return;
            }

            Instance.Log("Settings saved successfully !", Instance.gameObject, true);
        }

        public static bool TryLoad()
        {
            if (!System.IO.File.Exists(SettingsSavePath))
                return false;

            Instance.Log("Loading settings...", Instance.gameObject, true);

            try
            {
                XContainer container = XDocument.Parse(System.IO.File.ReadAllText(SettingsSavePath));
                XElement settingsSaveElement = container.Element("SettingsSave");

                XElement axisDeadZoneElement = settingsSaveElement.Element(Settings.AxisDeadZone.SAVE_ELEMENT_NAME);
                AxisDeadZone = axisDeadZoneElement != null ? new Settings.AxisDeadZone(axisDeadZoneElement) : new Settings.AxisDeadZone();

                XElement constrainCursorElement = settingsSaveElement.Element(Settings.ConstrainCursor.SAVE_ELEMENT_NAME);
                ConstrainCursor = constrainCursorElement != null ? new Settings.ConstrainCursor(constrainCursorElement) : new Settings.ConstrainCursor();

                XElement fullscreenElement = settingsSaveElement.Element(Settings.Fullscreen.SAVE_ELEMENT_NAME);
                Fullscreen = fullscreenElement != null ? new Settings.Fullscreen(fullscreenElement) : new Settings.Fullscreen();

                XElement monitorIndexElement = settingsSaveElement.Element(Settings.MonitorIndex.SAVE_ELEMENT_NAME);
                MonitorIndex = monitorIndexElement != null ? new Settings.MonitorIndex(monitorIndexElement) : new Settings.MonitorIndex();

                XElement pixelPerfectElement = settingsSaveElement.Element(Settings.PixelPerfect.SAVE_ELEMENT_NAME);
                PixelPerfect = pixelPerfectElement != null ? new Settings.PixelPerfect(pixelPerfectElement) : new Settings.PixelPerfect();

                XElement runInBackgroundElement = settingsSaveElement.Element(Settings.RunInBackground.SAVE_ELEMENT_NAME);
                RunInBackground = runInBackgroundElement != null ? new Settings.RunInBackground(runInBackgroundElement) : new Settings.RunInBackground();

                XElement screenSizeElement = settingsSaveElement.Element(Settings.ScreenSize.SAVE_ELEMENT_NAME);
                ScreenSize = screenSizeElement != null ? new Settings.ScreenSize(screenSizeElement) : new Settings.ScreenSize();

                XElement shakeAmountElement = settingsSaveElement.Element(Settings.ShakeAmount.SAVE_ELEMENT_NAME);
                ShakeAmount = shakeAmountElement != null ? new Settings.ShakeAmount(shakeAmountElement) : new Settings.ShakeAmount();

                XElement targetFrameRateElement = settingsSaveElement.Element(Settings.TargetFrameRate.SAVE_ELEMENT_NAME);
                TargetFrameRate = targetFrameRateElement != null ? new Settings.TargetFrameRate(targetFrameRateElement) : new Settings.TargetFrameRate();
            }
            catch (System.Exception e)
            {
                Instance.LogError($"Could not load settings ! Exception message:\n{e}");
                return false;
            }

            Instance.Log("Settings loaded successfully !", Instance.gameObject, true);
            return true;
        }

        public static void Init()
        {
            AxisDeadZone = new Settings.AxisDeadZone();
            ConstrainCursor = new Settings.ConstrainCursor();
            Fullscreen = new Settings.Fullscreen();
            MonitorIndex = new Settings.MonitorIndex();
            PixelPerfect = new Settings.PixelPerfect();
            RunInBackground = new Settings.RunInBackground();
            ScreenSize = new Settings.ScreenSize();
            ShakeAmount = new Settings.ShakeAmount();
            TargetFrameRate = new Settings.TargetFrameRate();
        }

        public static void LoadInputs()
        {
            RSLib.Framework.InputSystem.InputManager.SavePath = InputsSavePath;

            if (!RSLib.Framework.InputSystem.InputManager.TryLoadMap())
                RSLib.Framework.InputSystem.InputManager.SetMap(RSLib.Framework.InputSystem.InputManager.GetDefaultMapCopy());
            else
                RSLib.Framework.InputSystem.InputManager.GenerateMissingInputsFromSave();
        }

        protected override void Awake()
        {
            base.Awake();
            if (!IsValid)
                return;

            if (!TryLoad())
                Init();

            LoadInputs();

            Save();
            RSLib.Framework.InputSystem.InputManager.SaveCurrentMap();

            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command("SaveSettings", "Saves settings.", Save));
            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command("LoadSettings", "Tries to load settings.", () => TryLoad()));
        }
    }
}