namespace Templar.Manager
{
    using RSLib.Extensions;
    using System.Xml;
    using System.Xml.Linq;
    using UnityEngine;

    public partial class SaveManager : RSLib.Framework.ConsoleProSingleton<SaveManager>
    {
        [SerializeField] private bool _disableLoading = false;

        private static string SavePath => $"{Application.persistentDataPath}/Save/Game.xml";

        public static void Save()
        {
            Instance.Log("Saving game progression...");

            try
            {
                XContainer container = new XElement("GameSave");

                XElement checkpointIdElement = new XElement("CheckpointId", Interaction.Checkpoint.CheckpointController.CurrCheckpointId);
                container.Add(checkpointIdElement);

                XElement paletteIndexElement = new XElement("PaletteIndex", PaletteSelector.CurrRampIndex);
                container.Add(paletteIndexElement);

                XElement currencyElement = new XElement("Currency", CurrencyManager.Currency);
                container.Add(currencyElement);

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
                Interaction.Checkpoint.CheckpointController.LoadCurrentCheckpointId(GameManager.OverrideCheckpoint?.Id ?? checkpointIdElement.Value);

                XElement paletteIndexElement = gameSaveElement.Element("PaletteIndex");
                if (paletteIndexElement != null)
                    PaletteSelector.SetPalette(paletteIndexElement.ValueToInt());

                XElement currencyElement = gameSaveElement.Element("Currency");
                if (currencyElement != null)
                    CurrencyManager.LoadCurrency(currencyElement.ValueToLong());
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

                Interaction.Checkpoint.CheckpointController.ForceRemoveCurrentCheckpoint();
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

            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command("Save", "Saves game progression.", Save));
            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command("EraseSave", "Erases save file if it exists.", () => EraseSave()));
        }
    }
}