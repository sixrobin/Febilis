namespace RSLib.Framework.InputSystem
{
    using System.Xml;
    using System.Xml.Linq;
    using UnityEngine;

    public partial class InputManager : Singleton<InputManager>
    {
        [SerializeField] private InputMapDatas _defaultMapDatas = null;
        [SerializeField] private bool _disableMapLoading = false;

        public delegate void KeyAssignedEventHandler(string actionId, KeyCode btn, bool alt);

        private static InputMap s_inputMap;

        private static System.Collections.IEnumerator s_assignKeyCoroutine;
        private static KeyCode[] s_allKeyCodes;

        public static bool IsAssigningKey => s_assignKeyCoroutine != null;

        public static void RestoreDefaultMap()
        {
            s_inputMap = new InputMap(Instance._defaultMapDatas);
        }

        public static void AssignKey(string actionId, bool alt, KeyAssignedEventHandler callback = null)
        {
            if (IsAssigningKey)
                return;

            s_assignKeyCoroutine = AssignKeyCoroutine(actionId, alt, callback);
            Instance.StartCoroutine(s_assignKeyCoroutine);
        }

        public static bool GetInput(string actionId)
        {
            (KeyCode btn, KeyCode altBtn) = s_inputMap.GetActionKeyCodes(actionId);
            return Input.GetKey(btn) || Input.GetKey(altBtn);
        }

        public static bool GetInputDown(string actionId)
        {
            (KeyCode btn, KeyCode altBtn) = s_inputMap.GetActionKeyCodes(actionId);
            return Input.GetKeyDown(btn) || Input.GetKeyDown(altBtn);
        }
        
        public static bool GetInputUp(string actionId)
        {
            (KeyCode btn, KeyCode altBtn) = s_inputMap.GetActionKeyCodes(actionId);
            return Input.GetKeyUp(btn) || Input.GetKeyUp(altBtn);
        }

        public static System.Collections.Generic.Dictionary<string, (KeyCode btn, KeyCode altBtn)> GetMapCopy()
        {
            return s_inputMap.MapCopy;
        }

        private static System.Collections.IEnumerator AssignKeyCoroutine(string actionId, bool alt, KeyAssignedEventHandler callback = null)
        {
            Instance.Log($"Assigning key for action Id {actionId}...");

            for (int i = 0; i < 2; ++i)
                yield return Yield.SharedYields.WaitForEndOfFrame;

            yield return new WaitUntil(() => Input.anyKeyDown);

            KeyCode keyDown = KeyCode.None;

            for (int i = s_allKeyCodes.Length - 1; i >= 0; --i)
                if (Input.GetKeyDown(s_allKeyCodes[i]))
                    keyDown = s_allKeyCodes[i];

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Instance.Log($"Cancelling key assignment for action Id {actionId}.");
                callback?.Invoke(actionId, alt ? s_inputMap.GetActionKeyCodes(actionId).altBtn : s_inputMap.GetActionKeyCodes(actionId).btn, alt);
                s_assignKeyCoroutine = null;
                yield break;
            }

            Instance.Log($"Assigning key {keyDown.ToString()} to {(alt ? "alternate " : "")}button for action Id {actionId}.");
            s_inputMap.SetActionButton(actionId, keyDown, alt);
            callback?.Invoke(actionId, keyDown, alt);
            s_assignKeyCoroutine = null;
        }

        protected override void Awake()
        {
            base.Awake();

            if (_disableMapLoading || !TryLoadMap())
                RestoreDefaultMap();

            s_allKeyCodes = Helpers.GetEnumValues<KeyCode>();
        }
    }

    public partial class InputManager : Singleton<InputManager>
    {
        private static string SavePath => $"{Application.persistentDataPath}/InputMap.xml";

        public static void SaveCurrentMap()
        {
            Instance.Log("Saving input map...");

            XContainer container = new XElement("InputMap");

            foreach (System.Collections.Generic.KeyValuePair<string, (KeyCode btn, KeyCode altBtn)> binding in s_inputMap.MapCopy)
            {
                if (!s_inputMap.HasAction(binding.Key))
                    continue;

                XElement actionElement = new XElement(binding.Key);
                (KeyCode btn, KeyCode altBtn) = s_inputMap.GetActionKeyCodes(binding.Key);
                actionElement.Add(new XAttribute("Btn", btn.ToString()));
                actionElement.Add(new XAttribute("AltBtn", altBtn.ToString()));

                container.Add(actionElement);
            }

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
                Instance.LogError($"Could not save Input map ! Exception message:\n{e.ToString()}");
            }
        }

        public static bool TryLoadMap()
        {
            if (!System.IO.File.Exists(SavePath))
                return false;

            Instance.Log("Loading input map...");

            XContainer container = XDocument.Parse(System.IO.File.ReadAllText(SavePath));
            s_inputMap = new InputMap();
            return s_inputMap.Deserialize(container);
        }

        [ContextMenu("Save Current Map")]
        public void DebugSaveCurrentMap()
        {
            SaveCurrentMap();
        }
    }
}