namespace Templar.Manager
{
    using RSLib.Extensions;
    using System.Xml;
    using System.Xml.Linq;
    using UnityEngine;

    public class SaveManager : RSLib.Framework.ConsoleProSingleton<SaveManager>
    {
        public class SaveVersionUnknownException : System.Exception
        {
            public SaveVersionUnknownException() { }
            public SaveVersionUnknownException(string message) : base(message) { }
        }

        public class IncompatibleSaveVersionException : System.Exception
        {
            public IncompatibleSaveVersionException() { }
            public IncompatibleSaveVersionException(string message) : base(message) { }
        }

        [Header("VERSIONS")]
        [SerializeField] private int _saveVersion = 0;
        [SerializeField] private int _saveMinimumVersion = 0;

        [Header("DEBUG")]
        [SerializeField] private bool _disableLoading = false;

        public static int SaveVersion => Instance._saveVersion;
        public static int SaveMinimumVersion => Instance._saveMinimumVersion;

        public static bool DisableLoading => Instance._disableLoading;

        private static string GameSavePath => $"{Application.persistentDataPath}/Save/Game.xml";

        public static void Save()
        {
            Instance.Log("Saving game progression...", Instance.gameObject, true);

            try
            {
                XContainer container = new XElement("GameSave");

                XAttribute versionAttribute = new XAttribute("Version", SaveVersion);
                container.Add(versionAttribute);

                XElement checkpointIdElement = new XElement("CheckpointId", Interaction.Checkpoint.CheckpointController.CurrCheckpointId);
                container.Add(checkpointIdElement);

                container.Add(FlagsManager.Save());
                container.Add(GameManager.InventoryCtrl.Save());
                container.Add(DialoguesStructuresManager.Save());
                container.Add(FindObjectOfType<UI.Inventory.InventoryView>().Save()); // [TMP] Find.

                System.IO.FileInfo fileInfo = new System.IO.FileInfo(GameSavePath);
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
                    using (System.IO.FileStream diskStream = System.IO.File.Open(GameSavePath, System.IO.FileMode.Create, System.IO.FileAccess.Write))
                    {
                        ms.CopyTo(diskStream);
                    }
                }
            }
            catch (System.Exception e)
            {
                Instance.LogError($"Could not save game ! Exception message:\n{e}", Instance.gameObject);
                return;
            }

            Instance.Log("Game saved successfully !", Instance.gameObject, true);
        }

        public static bool TryLoad()
        {
            if (!System.IO.File.Exists(GameSavePath))
                return false;

            Instance.Log("Loading game progression...", Instance.gameObject, true);

            try
            {
                XContainer container = XDocument.Parse(System.IO.File.ReadAllText(GameSavePath));
                XElement gameSaveElement = container.Element("GameSave");

                XAttribute versionAttribute = gameSaveElement.Attribute("Version");
                int version = versionAttribute?.ValueToInt() ?? -1;

                if (version == -1)
                    throw new SaveVersionUnknownException($"Save version isn't specified in the save file.");

                if (version < SaveMinimumVersion)
                    throw new IncompatibleSaveVersionException($"Save file version is {version} while minimum handled is {SaveMinimumVersion}.");

                XElement checkpointIdElement = gameSaveElement.Element("CheckpointId");
                Interaction.Checkpoint.CheckpointController.LoadCurrentCheckpointId(
                    GameManager.OptionalCheckpoint.Enabled ? GameManager.OptionalCheckpoint.Value.Identifier.Id : checkpointIdElement.Value);

                FlagsManager.Load(gameSaveElement.Element("Flags"));
                GameManager.InventoryCtrl.Load(gameSaveElement.Element("Inventory"));
                DialoguesStructuresManager.Load(gameSaveElement.Element("DialoguesStructures"));
                FindObjectOfType<UI.Inventory.InventoryView>().Load(gameSaveElement.Element("InventoryView")); // [TMP] Find.
            }
            catch (SaveVersionUnknownException e)
            {
                Instance.LogError($"Could not load game ! Exception message:\n{e}");
                return false;
            }
            catch (IncompatibleSaveVersionException e)
            {
                Instance.LogError($"Could not load game ! Exception message:\n{e}");
                return false;
            }
            catch (System.Exception e)
            {
                Instance.LogError($"Could not load game ! Exception message:\n{e}");
                return false;
            }

            Instance.Log("Game loaded successfully !", Instance.gameObject, true);
            return true;
        }

        public static void LoadNewGame()
        {
            Instance.Log("Loading new game.", true);

            if (GameManager.InventoryCtrl == null)
                Instance.LogWarning($"{nameof(GameManager.InventoryCtrl)} could not be found, cannot load it.");
            else
               GameManager.InventoryCtrl.Load();
        
            FindObjectOfType<UI.Inventory.InventoryView>().Load(); // [TMP] Find.
        }

        public static bool EraseSave()
        {
            if (!System.IO.File.Exists(GameSavePath))
                return false;

            try
            {
                System.IO.File.Delete(GameSavePath);

                FlagsManager.Clear();
                DialoguesStructuresManager.Clear();
                Interaction.Checkpoint.CheckpointController.ClearCurrentCheckpoint();
            }
            catch (System.Exception e)
            {
                Instance.LogError($"Could not delete game save ! Exception message:\n{e}");
                return false;
            }

            Instance.Log("Game save erased successfully !", true);
            return true;
        }

        protected override void Awake()
        {
            base.Awake();
            if (!IsValid)
                return;

            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command("SaveGame", "Saves game progression.", Save));
            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command("LoadGame", "Tries to load game progression.", () => TryLoad()));
            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command("EraseGameSave", "Erases game save file if it exists.", () => EraseSave()));
        }

        private void OnValidate()
        {
            _saveMinimumVersion = Mathf.Min(_saveMinimumVersion, _saveVersion);
        }
    }
}