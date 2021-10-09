namespace Templar.Manager
{
    using System.Xml;
    using System.Xml.Linq;

    public class SettingsManager : RSLib.Framework.ConsoleProSingleton<SettingsManager>
    {
        private Settings.AxisDeadZone _axisDeadZone;
        private Settings.ShakeAmount _shakeAmount;
        private Settings.PixelPerfect _pixelPerfect;
        private Settings.MonitorIndex _monitorIndex;

        private static string SettingsSavePath => $"{UnityEngine.Application.persistentDataPath}/Save/Settings.xml";

        public static void Save()
        {
            Instance.Log("Saving settings...", Instance.gameObject, true);

            try
            {
                XContainer container = new XElement("SettingsSave");

                container.Add(Instance._shakeAmount.Save());
                container.Add(Instance._axisDeadZone.Save());
                container.Add(Instance._pixelPerfect.Save());
                container.Add(Instance._monitorIndex.Save());

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
            {
                Init();
                return false;
            }

            Instance.Log("Loading settings...", Instance.gameObject, true);

            try
            {
                XContainer container = XDocument.Parse(System.IO.File.ReadAllText(SettingsSavePath));
                XElement settingsSaveElement = container.Element("SettingsSave");

                XElement axisDeadZoneElement = settingsSaveElement.Element(Settings.AxisDeadZone.SAVE_ELEMENT_NAME);
                Instance._axisDeadZone = axisDeadZoneElement != null ? new Settings.AxisDeadZone(axisDeadZoneElement) : new Settings.AxisDeadZone();

                XElement shakeAmountElement = settingsSaveElement.Element(Settings.ShakeAmount.SAVE_ELEMENT_NAME);
                Instance._shakeAmount = shakeAmountElement != null ? new Settings.ShakeAmount(shakeAmountElement) : new Settings.ShakeAmount();

                XElement pixelPerfectElement = settingsSaveElement.Element(Settings.PixelPerfect.SAVE_ELEMENT_NAME);
                Instance._pixelPerfect = pixelPerfectElement != null ? new Settings.PixelPerfect(pixelPerfectElement) : new Settings.PixelPerfect();

                XElement monitorIndexElement = settingsSaveElement.Element(Settings.MonitorIndex.SAVE_ELEMENT_NAME);
                Instance._monitorIndex = monitorIndexElement != null ? new Settings.MonitorIndex(monitorIndexElement) : new Settings.MonitorIndex();
            }
            catch (System.Exception e)
            {
                Instance.LogError($"Could not load settings ! Exception message:\n{e}");
                return false;
            }

            Instance.Log("Settings loaded successfully !", Instance.gameObject, true);
            return true;
        }

        private static void Init()
        {
            Instance._shakeAmount = new Settings.ShakeAmount();
            Instance._axisDeadZone = new Settings.AxisDeadZone();
            Instance._pixelPerfect = new Settings.PixelPerfect();
            Instance._monitorIndex = new Settings.MonitorIndex();
        }

        protected override void Awake()
        {
            base.Awake();
            if (!IsValid)
                return;

            TryLoad();

            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command("SaveSettings", "Saves settings.", Save));
            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command("LoadSettings", "Tries to load settings.", () => TryLoad()));
        }
    }
}