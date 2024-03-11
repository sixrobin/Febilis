namespace RSLib.Framework.InputSystem
{
    using System.Xml;
    using System.Xml.Linq;
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    public partial class InputManager : Singleton<InputManager>
    {
        [SerializeField] private InputMapDatas _defaultMapDatas = null;
        [SerializeField] private bool _disableMapLoading = false;
        [SerializeField] private bool _autoLoadOnStart = true;
        [SerializeField] private KeyCode[] _cancelAssignKeys = null;
        [SerializeField] private KeyCode[] _unbindableKeys = null;

        public delegate void KeyAssignedEventHandler(string actionId, KeyCode btn, bool alt);

        private static InputMap s_inputMap;

        private static System.Collections.IEnumerator s_assignKeyCoroutine;
        private static KeyCode[] s_allKeyCodes;

        public static bool IsAssigningKey => s_assignKeyCoroutine != null;

        public static InputMap GetDefaultMapCopy()
        {
            return new InputMap(Instance._defaultMapDatas);
        }

        public static void AssignKey(string actionId, bool alt, KeyAssignedEventHandler callback = null)
        {
            AssignKey(s_inputMap, actionId, alt, callback);
        }

        public static void AssignKey(InputMap map, string actionId, bool alt, KeyAssignedEventHandler callback = null)
        {
            if (IsAssigningKey)
                return;

            Instance.StartCoroutine(s_assignKeyCoroutine = AssignKeyCoroutine(map, actionId, alt, callback));
        }

        public static bool GetInput(string actionId)
        {
            InputMapDatas.KeyBinding keyBinding = s_inputMap.GetActionBinding(actionId);
            return Input.GetKey(keyBinding.KeyCodes.btn) || (s_inputMap.UseAltButtons && Input.GetKey(keyBinding.KeyCodes.altBtn));
        }

        public static bool GetAnyInput(params string[] actionsIds)
        {
            for (int i = actionsIds.Length - 1; i >= 0; --i)
                if (GetInput(actionsIds[i]))
                    return true;

            return false;
        }

        public static bool GetInputDown(string actionId)
        {
            InputMapDatas.KeyBinding keyBinding = s_inputMap.GetActionBinding(actionId);
            return Input.GetKeyDown(keyBinding.KeyCodes.btn) || (s_inputMap.UseAltButtons && Input.GetKeyDown(keyBinding.KeyCodes.altBtn));
        }

        public static bool GetAnyInputDown(params string[] actionsIds)
        {
            for (int i = actionsIds.Length - 1; i >= 0; --i)
                if (GetInputDown(actionsIds[i]))
                    return true;

            return false;
        }

        public static bool GetInputUp(string actionId)
        {
            InputMapDatas.KeyBinding keyBinding = s_inputMap.GetActionBinding(actionId);
            return Input.GetKeyUp(keyBinding.KeyCodes.btn) || (s_inputMap.UseAltButtons && Input.GetKeyUp(keyBinding.KeyCodes.altBtn));
        }

        public static bool GetAnyInputUp(params string[] actionsIds)
        {
            for (int i = actionsIds.Length - 1; i >= 0; --i)
                if (GetInputUp(actionsIds[i]))
                    return true;

            return false;
        }

        public static InputMap GetMap()
        {
            return s_inputMap;
        }
        
        public static System.Collections.Generic.Dictionary<string, InputMapDatas.KeyBinding> GetMapCopy()
        {
            return GetMap().MapCopy;
        }

        public static void SetBindings(System.Collections.Generic.Dictionary<string, InputMapDatas.KeyBinding> bindings, bool useAltButtons)
        {
            s_inputMap = new InputMap(bindings, useAltButtons);
        }

        public static void SetMap(InputMap map)
        {
            s_inputMap = new InputMap(map);
        }

        public static void GenerateMissingInputsFromSave()
        {
            for (int i = Instance._defaultMapDatas.Bindings.Length - 1; i >= 0; --i)
            {
                if (s_inputMap.HasAction(Instance._defaultMapDatas.Bindings[i].ActionId))
                    continue;

                Instance.Log($"Loading missing binding from InputSave for action Id {Instance._defaultMapDatas.Bindings[i].ActionId}", Instance.gameObject);
                s_inputMap.CreateAction(Instance._defaultMapDatas.Bindings[i].ActionId, Instance._defaultMapDatas.Bindings[i]);
            }

            s_inputMap.SortActionsBasedOnData(Instance._defaultMapDatas);
            SaveCurrentMap();
        }

        private static System.Collections.IEnumerator AssignKeyCoroutine(InputMap map, string actionId, bool alt, KeyAssignedEventHandler callback = null)
        {
            Instance.Log($"Assigning key for action Id {actionId}...", Instance.gameObject);

            KeyCode key = KeyCode.None;

            for (int i = 0; i < 2; ++i)
                yield return Yield.SharedYields.WaitForEndOfFrame;

            while (key == KeyCode.None)
            {
                yield return new WaitUntil(() => Input.anyKeyDown);

                for (int i = Instance._cancelAssignKeys.Length - 1; i >= 0; --i)
                {
                    if (!Input.GetKeyDown(Instance._cancelAssignKeys[i]))
                        continue;
                    
                    Instance.Log($"Cancelling key assignment for action Id {actionId}.", Instance.gameObject);
                    callback?.Invoke(actionId, alt ? map.GetActionBinding(actionId).KeyCodes.altBtn : map.GetActionBinding(actionId).KeyCodes.btn, alt);
                    s_assignKeyCoroutine = null;
                    yield break;
                }

                for (int i = s_allKeyCodes.Length - 1; i >= 0; --i)
                {
                    if (!Input.GetKeyDown(s_allKeyCodes[i]))
                        continue;
                    
                    key = s_allKeyCodes[i];
                    break;
                }

                for (int i = Instance._unbindableKeys.Length - 1; i >= 0; --i)
                {
                    if (Instance._unbindableKeys[i] != key)
                        continue;
                    
                    key = KeyCode.None;
                    break;
                }
            }

            Instance.Log($"Assigning key {key} to {(alt ? "alternate " : "")}button for action Id {actionId}.", Instance.gameObject);
            map.SetActionButton(actionId, key, alt);
            callback?.Invoke(actionId, key, alt);

            s_assignKeyCoroutine = null;
        }

        private void Start()
        {
            s_allKeyCodes = Helpers.GetEnumValues<KeyCode>();

            // Used to trigger loading manually from anywhere else, depending on the project this API is used in.
            if (!_autoLoadOnStart)
                return;

            if (_disableMapLoading || !TryLoadMap())
                s_inputMap = GetDefaultMapCopy();
            else
                GenerateMissingInputsFromSave();

            SaveCurrentMap();
        }
    }

    public partial class InputManager : Singleton<InputManager>
    {
        public class SaveDoneEventArgs : System.EventArgs
        {
            public SaveDoneEventArgs(bool success)
            {
                Success = success;
            }
            
            public bool Success { get; }
        }
        
        public const string INPUT_MAP_ELEMENT_NAME = "InputMap";
        public const string BTN_ATTRIBUTE_NAME = "Btn";
        public const string ALT_ATTRIBUTE_NAME = "Alt";

        private static string s_savePath;

        public delegate void SaveDoneEventHandler(SaveDoneEventArgs args);
        public static event SaveDoneEventHandler SaveDone;
        
        public static string SavePath
        {
            get => string.IsNullOrEmpty(s_savePath) ? System.IO.Path.Combine(Application.persistentDataPath, "Inputs.xml") : s_savePath;
            set => s_savePath = value;
        }

        public static void SaveCurrentMap()
        {
            Instance.Log($"Saving input map to {SavePath}...", Instance.gameObject);

            XContainer container = new XElement(INPUT_MAP_ELEMENT_NAME);

            container.Add(new XAttribute("UseAltButtons", s_inputMap.UseAltButtons));
            
            foreach (System.Collections.Generic.KeyValuePair<string, InputMapDatas.KeyBinding> binding in s_inputMap.MapCopy)
            {
                if (!s_inputMap.HasAction(binding.Key) || !binding.Value.UserAssignable)
                    continue;

                XElement actionElement = new XElement(binding.Key);
                (KeyCode btn, KeyCode altBtn) = s_inputMap.GetActionBinding(binding.Key).KeyCodes;
                actionElement.Add(new XAttribute(BTN_ATTRIBUTE_NAME, btn.ToString()));
                actionElement.Add(new XAttribute(ALT_ATTRIBUTE_NAME, altBtn.ToString()));

                container.Add(actionElement);
            }
            
            try
            {
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(SavePath);
                if (fileInfo.Directory != null && fileInfo.DirectoryName != null && !fileInfo.Directory.Exists)
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
                Instance.LogError($"Could not save Input map at {SavePath} ! Exception message:\n{e}", Instance.gameObject);
                SaveDone?.Invoke(new SaveDoneEventArgs(false));
            }
            
            SaveDone?.Invoke(new SaveDoneEventArgs(true));
        }

        public static bool TryLoadMap()
        {
            if (!System.IO.File.Exists(SavePath))
                return false;

            try
            {
                Instance.Log($"Loading input map from {SavePath}...", Instance.gameObject);

                XContainer container = XDocument.Parse(System.IO.File.ReadAllText(SavePath));
                s_inputMap = new InputMap(container);
            }
            catch (System.Exception e)
            {
                Instance.LogError($"Could not load Input map from {SavePath} ! Exception message:\n{e}", Instance.gameObject);
                return false;
            }

            return true;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(InputManager))]
    public class InputManagerEditor : EditorUtilities.ButtonProviderEditor<InputManager>
    {
        protected override void DrawButtons()
        {
            DrawButton("Save Current Map", InputManager.SaveCurrentMap);
            DrawButton("Try Load Map", () => InputManager.TryLoadMap());
        }
    }
#endif
}