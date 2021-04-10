namespace Templar.Manager
{
    using RSLib.Extensions;
    using System.Xml;
    using System.Xml.Linq;
    using UnityEngine;

    public partial class SaveManager : RSLib.Framework.Singleton<SaveManager>
    {
        [SerializeField] private bool _disableLoading = false;

        private static string SavePath => $"{Application.persistentDataPath}/Save/Game.xml";

        public static void Save()
        {
            Instance.Log("Saving game progression...");

            XContainer container = new XElement("GameSave");

            XElement checkpointIdElement = new XElement("CheckpointId", Interaction.CheckpointController.CurrCheckpointId);
            container.Add(checkpointIdElement);

            XElement paletteIndexElement = new XElement("PaletteIndex", FindObjectOfType<PaletteSelector>().CurrRampIndex); // [TMP] Find.
            container.Add(paletteIndexElement);

            try
            {
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(SavePath);
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
                    using (System.IO.FileStream diskStream = System.IO.File.Open(SavePath, System.IO.FileMode.Create, System.IO.FileAccess.Write))
                    {
                        ms.CopyTo(diskStream);
                    }
                }
            }
            catch (System.Exception e)
            {
                Instance.LogError($"Could not save game ! Exception message:\n{e.ToString()}");
                return;
            }

            Instance.Log("Game saved successfully !");
        }

        public static bool TryLoad()
        {
            if (!System.IO.File.Exists(SavePath))
                return false;

            Instance.Log("Loading game progression...");

            try
            {
                XContainer container = XDocument.Parse(System.IO.File.ReadAllText(SavePath));

                XElement gameSaveElement = container.Element("GameSave");

                XElement checkpointIdElement = gameSaveElement.Element("CheckpointId");
                Interaction.CheckpointController.LoadCurrentCheckpointId(GameManager.OverrideCheckpoint?.Id ?? checkpointIdElement.Value);

                XElement paletteIndexElement = gameSaveElement.Element("PaletteIndex");
                if (paletteIndexElement != null)
                    FindObjectOfType<PaletteSelector>().LoadPalette(paletteIndexElement.ValueToInt()); // [TMP] Find.
            }
            catch (System.Exception e)
            {
                Instance.LogError($"Could not load game ! Exception message:\n{e.ToString()}");
                return false;
            }

            Instance.Log("Game loaded successfully !");
            return true;
        }

        public static bool EraseSave()
        {
            if (!System.IO.File.Exists(SavePath))
                return false;

            try
            {
                System.IO.File.Delete(SavePath);
            }
            catch (System.Exception e)
            {
                Instance.LogError($"Could not delete game save ! Exception message:\n{e.ToString()}");
                return false;
            }

            Instance.Log("Game save erased successfully !");
            return true;
        }

        protected override void Awake()
        {
            base.Awake();

            if (!_disableLoading)
                TryLoad();
        }
    }

    public partial class SaveManager : RSLib.Framework.Singleton<SaveManager>
    {
        public override void Log(string msg)
        {
            CProLogger.Log(this, msg, gameObject);
        }

        public override void Log(string msg, Object context)
        {
            CProLogger.Log(this, msg, gameObject);
        }

        public override void LogError(string msg)
        {
            CProLogger.LogError(this, msg, gameObject);
        }

        public override void LogError(string msg, Object context)
        {
            CProLogger.LogError(this, msg, gameObject);
        }
    }
}