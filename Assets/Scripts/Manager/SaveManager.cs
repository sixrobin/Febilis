﻿namespace Templar.Manager
{
    using RSLib.Extensions;
    using System.Xml;
    using System.Xml.Linq;
    using UnityEngine;

    public class SaveManager : RSLib.Framework.SingletonConsolePro<SaveManager>
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

        public class SaveDoneEventArgs : System.EventArgs
        {
            public SaveDoneEventArgs(bool success, bool onLoad)
            {
                Success = success;
                OnLoad = onLoad;
            }
            
            public bool Success { get; }
            public bool OnLoad { get; }
        }
        
        [Header("ENCRYPTION")]
        [SerializeField] private bool _encryptSave = true;

        [Header("VERSIONS")]
        [SerializeField] private int _saveVersion = 0;
        [SerializeField] private int _saveMinimumVersion = 0;

        [Header("DEBUG")]
        [SerializeField] private bool _disableLoading = false;

        private static RSLib.Encryption.Rijndael s_rijndael;

        public delegate void SaveDoneEventHandler(SaveDoneEventArgs args);
        public static event SaveDoneEventHandler SaveDone;
        
        public static int SaveVersion => Instance._saveVersion;
        public static int SaveMinimumVersion => Instance._saveMinimumVersion;

        public static bool DisableLoading => Instance._disableLoading;

        public static bool GameSaveExist => System.IO.File.Exists(GameSaveFilePath);

        private static string GameSaveFolderPath => System.IO.Path.Combine(Application.persistentDataPath, "Save");
        private static string GameSaveFilePath => System.IO.Path.Combine(GameSaveFolderPath, "Game.dat");

        public static bool TrySave(bool? overrideEncryptSave = null, bool onLoad = false)
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

                    container.Add(FlagsManager.Save());
                    container.Add(GameManager.InventoryCtrl.Save());
                    container.Add(GameManager.InventoryView.Save());
                    container.Add(DialoguesStructuresManager.Save());
                    container.Add(LootManager.Save());
                    container.Add(InputTutorialDisplay.Save());
                }

                System.IO.FileInfo fileInfo = new System.IO.FileInfo(GameSaveFilePath);
                if (!fileInfo.Directory.Exists)
                    System.IO.Directory.CreateDirectory(fileInfo.DirectoryName);

                XDocument saveDocument = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), container);
                string decryptedSave = saveDocument.ToString();

                if (Instance._encryptSave && (!overrideEncryptSave.HasValue || overrideEncryptSave.Value))
                    System.IO.File.WriteAllBytes(GameSaveFilePath, s_rijndael.Encrypt(decryptedSave));
                else
                    System.IO.File.WriteAllText(GameSaveFilePath, decryptedSave);
            }
            catch (System.Exception e)
            {
                Instance.LogError($"Could not save game ! Exception message:\n{e}", Instance.gameObject);
                SaveDone?.Invoke(new SaveDoneEventArgs(false, onLoad));
                return false;
            }

            Instance.Log("Game saved successfully !", Instance.gameObject, true);
            SaveDone?.Invoke(new SaveDoneEventArgs(true, onLoad));
            return true;
        }

        public static bool TryLoad()
        {
            if (!GameSaveExist)
                return false;

            Instance.Log("Loading game progression...", Instance.gameObject, true);

            try
            {
                byte[] saveBytes = System.IO.File.ReadAllBytes(GameSaveFilePath);
                string saveText = System.IO.File.ReadAllText(GameSaveFilePath);

                XContainer container = null;

                try
                {
                    container = XDocument.Parse(Instance._encryptSave ? s_rijndael.Decrypt(saveBytes) : saveText);
                }
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
                GameManager.InventoryView.Load(gameSaveElement.Element("InventoryView"));
                DialoguesStructuresManager.Load(gameSaveElement.Element("DialoguesStructures"));
                LootManager.Load(gameSaveElement.Element("ItemsLoot"));
                InputTutorialDisplay.Load(gameSaveElement.Element("ValidatedInputs"));
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
            if (!System.IO.File.Exists(GameSaveFilePath))
                return false;

            try
            {
                System.IO.File.Delete(GameSaveFilePath);

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

            Instance.Log($"Save version: {_saveVersion} (minimum: {_saveMinimumVersion}).");
            
            RSLib.Debug.Console.DebugConsole.OverrideCommand("SaveGame", "Tries to save game progression.", () => TrySave());
            RSLib.Debug.Console.DebugConsole.OverrideCommand<bool>("SaveGame", "Tries to save game progression, specifying encryption state.", encryptSave => TrySave(encryptSave));
            RSLib.Debug.Console.DebugConsole.OverrideCommand("LoadGame", "Tries to load game progression.", () => TryLoad());
            RSLib.Debug.Console.DebugConsole.OverrideCommand("EraseGameSave", "Erases game save file if it exists.", () => EraseSave());
            RSLib.Debug.Console.DebugConsole.OverrideCommand("OpenSaveFolder", "Opens game save file.", OpenSaveFolder);
            RSLib.Debug.Console.DebugConsole.OverrideCommand<bool>("SetSaveEncryption", "Sets save encryption state.", value => _encryptSave = value);
            RSLib.Debug.Console.DebugConsole.OverrideCommand("ToggleSaveEncryption", "Toggles save encryption state.", () => _encryptSave = !_encryptSave);
        }

        private void OnValidate()
        {
            _saveMinimumVersion = Mathf.Min(_saveMinimumVersion, _saveVersion);
        }
        
        [ContextMenu("Save")]
        public void DebugSave()
        {
            TrySave();
        }
        
        [ContextMenu("Save (no encryption)")]
        public void DebugSaveNoEncryption()
        {
            TrySave(false);
        }
        
        [ContextMenu("Open Save Folder")]
        public void OpenSaveFolder()
        {
            System.Diagnostics.Process.Start($@"{GameSaveFolderPath}");
        }
        
        [ContextMenu("Erase Save File")]
        public void DebugEraseSaveFile()
        {
            if (!System.IO.File.Exists(GameSaveFilePath))
                return;
            
            System.IO.File.Delete(GameSaveFilePath);
            Instance.Log("Game save erased successfully !", true);
        }
    }
    
#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(SaveManager))]
    public class SaveManagerEditor : RSLib.EditorUtilities.ButtonProviderEditor<SaveManager>
    {
        protected override void DrawButtons()
        {
            if (UnityEditor.EditorApplication.isPlaying)
            {
                DrawButton("Save", Obj.DebugSave);
                DrawButton("Save (no encryption)", Obj.DebugSaveNoEncryption);
            }
            
            DrawButton("Open Save Folder", Obj.OpenSaveFolder);
            DrawButton("Erase Save File", Obj.DebugEraseSaveFile);
        }
    }
#endif
}