﻿namespace Templar.Manager
{
    using System.Xml;
    using System.Xml.Linq;

    public class SettingsManager : RSLib.Framework.ConsoleProSingleton<SettingsManager>
    {
        public static Settings.AxisDeadZone AxisDeadZone { get; private set; }
        public static Settings.ConstrainCursor ConstrainCursor { get; private set; }
        public static Settings.FullscreenMode FullscreenMode { get; private set; }
        public static Settings.MonitorIndex MonitorIndex { get; private set; }
        public static Settings.PixelPerfect PixelPerfect { get; private set; }
        public static Settings.RunInBackground RunInBackground { get; private set; }
        public static Settings.ScreenSize ScreenSize { get; private set; }
        public static Settings.ShakeAmount ShakeAmount { get; private set; }
        public static Settings.TargetFrameRate TargetFrameRate { get; private set; }

        private static string SettingsSavePath => System.IO.Path.Combine(UnityEngine.Application.persistentDataPath, "Save", "Settings.xml");
        private static string InputsSavePath => System.IO.Path.Combine(UnityEngine.Application.persistentDataPath, "Save", "Inputs.xml");

        public static void Save()
        {
            Instance.Log("Saving settings...", Instance.gameObject, true);

            try
            {
                XContainer container = new XElement("SettingsSave");

                container.Add(AxisDeadZone.Save());
                container.Add(ConstrainCursor.Save());
                container.Add(FullscreenMode.Save());
                container.Add(MonitorIndex.Save());
                container.Add(PixelPerfect.Save());
                container.Add(RunInBackground.Save());
                container.Add(ScreenSize.Save());
                container.Add(ShakeAmount.Save());
                container.Add(TargetFrameRate.Save());

                // Audio.
                if (RSLib.Audio.AudioManager.TryGetFloatParameterValue("MasterVolume", out float masterVolume))
                    container.Add(new XElement("MasterVolume", RSLib.Audio.AudioManager.DecibelsToLinear(masterVolume)));
                if (RSLib.Audio.AudioManager.TryGetFloatParameterValue("MusicVolume", out float musicVolume))
                {
                    UnityEngine.Debug.LogError(musicVolume);
                    UnityEngine.Debug.LogError(RSLib.Audio.AudioManager.DecibelsToLinear(musicVolume));
                    container.Add(new XElement("MusicVolume", RSLib.Audio.AudioManager.DecibelsToLinear(musicVolume)));
                }
                if (RSLib.Audio.AudioManager.TryGetFloatParameterValue("SFXVolume", out float sfxVolume))
                    container.Add(new XElement("SFXVolume", RSLib.Audio.AudioManager.DecibelsToLinear(sfxVolume)));
                if (RSLib.Audio.AudioManager.TryGetFloatParameterValue("FootstepsVolume", out float footstepsVolume))
                    container.Add(new XElement("FootstepsVolume", RSLib.Audio.AudioManager.DecibelsToLinear(footstepsVolume)));
                if (RSLib.Audio.AudioManager.TryGetFloatParameterValue("UIVolume", out float uiVolume))
                    container.Add(new XElement("UIVolume", RSLib.Audio.AudioManager.DecibelsToLinear(uiVolume)));
                
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

                XElement fullscreenModeElement = settingsSaveElement.Element(Settings.FullscreenMode.SAVE_ELEMENT_NAME);
                FullscreenMode = fullscreenModeElement != null ? new Settings.FullscreenMode(fullscreenModeElement) : new Settings.FullscreenMode();

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
                
                // Audio.
                XElement masterVolumeElement = settingsSaveElement.Element("MasterVolume");
                if (masterVolumeElement != null && float.TryParse(masterVolumeElement.Value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float masterVolume))
                    RSLib.Audio.AudioManager.SetVolumePercentage("MasterVolume", masterVolume);
                
                XElement musicVolumeElement = settingsSaveElement.Element("MusicVolume");
                if (musicVolumeElement != null && float.TryParse(musicVolumeElement.Value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float musicVolume))
                    RSLib.Audio.AudioManager.SetVolumePercentage("MusicVolume", musicVolume);
                
                XElement sfxVolumeElement = settingsSaveElement.Element("SFXVolume");
                if (sfxVolumeElement != null && float.TryParse(sfxVolumeElement.Value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float sfxVolume))
                    RSLib.Audio.AudioManager.SetVolumePercentage("SFXVolume", sfxVolume);
                
                XElement footstepsVolumeElement = settingsSaveElement.Element("FootstepsVolume");
                if (footstepsVolumeElement != null && float.TryParse(footstepsVolumeElement.Value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float footstepsVolume))
                    RSLib.Audio.AudioManager.SetVolumePercentage("FootstepsVolume", footstepsVolume);
                
                XElement uiVolumeElement = settingsSaveElement.Element("UIVolume");
                if (uiVolumeElement != null && float.TryParse(uiVolumeElement.Value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float uiVolume))
                    RSLib.Audio.AudioManager.SetVolumePercentage("UIVolume", uiVolume);
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
            FullscreenMode = new Settings.FullscreenMode();
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