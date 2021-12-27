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

        private static RSLib.Encryption.Rijndael s_rijndael;

        [Header("ENCRYPTION")]
        [SerializeField] private bool _encryptSave = true;

        [Header("VERSIONS")]
        [SerializeField] private int _saveVersion = 0;
        [SerializeField] private int _saveMinimumVersion = 0;

        [Header("DEBUG")]
        [SerializeField] private bool _disableLoading = false;

        public static int SaveVersion => Instance._saveVersion;
        public static int SaveMinimumVersion => Instance._saveMinimumVersion;

        public static bool DisableLoading => Instance._disableLoading;

        public static bool GameSaveExist => System.IO.File.Exists(GameSavePath);

        private static string GameSavePath => System.IO.Path.Combine(Application.persistentDataPath, "Save", "Game.dat");

        public static bool TrySave()
        {
            Instance.Log("Saving game progression...", Instance.gameObject, true);

            try
            {
                XContainer container = new XElement("GameSave");

                // XContainer initialization.
                {
                    XAttribute versionAttribute = new XAttribute("Version", SaveVersion);
                    container.Add(versionAttribute);

                    XElement checkpointIdElement = new XElement("CheckpointId", Interaction.Checkpoint.CheckpointController.CurrCheckpointId);
                    container.Add(checkpointIdElement);

                    container.Add(GameManager.InventoryCtrl.Save());
                    container.Add(DialoguesStructuresManager.Save());
                    container.Add(FindObjectOfType<UI.Inventory.InventoryView>().Save()); // [TMP] Find.
                }

                System.IO.FileInfo fileInfo = new System.IO.FileInfo(GameSavePath);
                if (!fileInfo.Directory.Exists)
                    System.IO.Directory.CreateDirectory(fileInfo.DirectoryName);

                XDeclaration xDeclaration = new XDeclaration("1.0", "utf-8", "yes");
                XDocument saveDocument = new XDocument(xDeclaration, container);
                string decryptedSave = saveDocument.ToString();

                if (Instance._encryptSave)
                {
                    byte[] encryptedSave = s_rijndael.Encrypt(decryptedSave);
                    System.IO.File.WriteAllBytes(GameSavePath, encryptedSave);
                }
                else
                {
                    System.IO.File.WriteAllText(GameSavePath, decryptedSave);
                }
            }
            catch (System.Exception e)
            {
                Instance.LogError($"Could not save game ! Exception message:\n{e}", Instance.gameObject);
                return false;
            }

            Instance.Log("Game saved successfully !", Instance.gameObject, true);
            return true;
        }

        public static bool TryLoad()
        {
            if (!GameSaveExist)
                return false;

            Instance.Log("Loading game progression...", Instance.gameObject, true);

            try
            {
                byte[] saveBytes = System.IO.File.ReadAllBytes(GameSavePath);
                string saveText = System.IO.File.ReadAllText(GameSavePath);

                XContainer container = null;

                try
                {
                    container = XDocument.Parse(Instance._encryptSave ? s_rijndael.Decrypt(saveBytes) : saveText);
                catch (System.Exception) // Loading encrypted save without encryption toggled on.
                {
                    Instance.LogWarning($"Could not parse save to XDocument. This may be due to save being encrypted but loading it without decryption, trying to decrypt it and parse again."
                        + $"\nThat should happen in editor only. If it happened to a player, something strange has happened !");

                    try
                    {
                        container = XDocument.Parse(s_rijndael.Decrypt(saveBytes));
                    }
                    catch (System.Exception) // Loading normal save with encryption toggled on.
                    {
                        Instance.LogWarning($"Could not parse save to XDocument. This may be due to save not being encrypted but loading it with decryption, trying to read it as it is and parse again."
                            + $"\nThat should happen in editor only. If it happened to a player, something strange has happened !");
                        
                        container = XDocument.Parse(saveText);
                    }
                }

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

            // Cryptography initialization.
            s_rijndael = new RSLib.Encryption.Rijndael
            (
                new byte[] { 0x53, 0x12, 0x22, 0xe1, 0x08, 0xab, 0xda, 0xe1, 0x0b, 0x03, 0xe5, 0x96, 0xd9, 0x23, 0xbd, 0x1a, 0xb8, 0x67, 0x4c, 0x3b, 0xee, 0x3e, 0x61, 0x46, 0x66, 0xcd, 0xea, 0x9c, 0x24, 0x20, 0x8c, 0x2c },
                new byte[] { 0x5e, 0x67, 0x6c, 0xe3, 0xbf, 0x54, 0xb2, 0x45, 0xbc, 0xcc, 0x9e, 0x2d, 0xc8, 0xa0, 0xab, 0xca }
            );

            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command("SaveGame", "Tries to save game progression.", () => TrySave()));
            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command("LoadGame", "Tries to load game progression.", () => TryLoad()));
            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command("EraseGameSave", "Erases game save file if it exists.", () => EraseSave()));
            
            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command<bool>("SetSaveEncryption", "Sets save encryption state.", (value) => _encryptSave = value));
            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command("ToggleSaveEncryption", "Toggles save encryption state.", () => _encryptSave = !_encryptSave));
        }

        private void OnValidate()
        {
            _saveMinimumVersion = Mathf.Min(_saveMinimumVersion, _saveVersion);
        }
    }
}