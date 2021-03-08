namespace RSLib.Framework.InputSystem
{
    using System.Xml;
    using System.Xml.Linq;
    using UnityEngine;

    public partial class InputManager : Singleton<InputManager>
    {
        [SerializeField] private InputMapDatas _defaultMapDatas = null;
        [SerializeField] private bool _disableMapLoading = false;

        public delegate void KeyAssignedEventHandler(InputAction action, KeyCode btn, bool alt);

        private static InputMap s_inputMap;

        private static System.Collections.IEnumerator s_assignKeyCoroutine;
        private static KeyCode[] s_allKeyCodes;
        private static InputAction[] s_allActions;

        public static bool IsAssigningKey => s_assignKeyCoroutine != null;

        public static void RestoreDefaultMap()
        {
            s_inputMap = new InputMap(Instance._defaultMapDatas);
        }

        public static void AssignKey(InputAction action, bool alt, KeyAssignedEventHandler callback = null)
        {
            if (IsAssigningKey)
                return;

            s_assignKeyCoroutine = AssignKeyCoroutine(action, alt, callback);
            Instance.StartCoroutine(s_assignKeyCoroutine);
        }

        public static bool GetInput(InputAction action)
        {
            (KeyCode btn, KeyCode altBtn) = s_inputMap.GetActionKeyCodes(action);
            return Input.GetKey(btn) || Input.GetKey(altBtn);
        }

        public static bool GetInputDown(InputAction action)
        {
            (KeyCode btn, KeyCode altBtn) = s_inputMap.GetActionKeyCodes(action);
            return Input.GetKeyDown(btn) || Input.GetKeyDown(altBtn);
        }

        public static bool GetInputUp(InputAction action)
        {
            (KeyCode btn, KeyCode altBtn) = s_inputMap.GetActionKeyCodes(action);
            return Input.GetKeyUp(btn) || Input.GetKeyUp(altBtn);
        }

        public static System.Collections.Generic.Dictionary<InputAction, (KeyCode btn, KeyCode altBtn)> GetMapCopy()
        {
            return s_inputMap.MapCopy;
        }

        private static System.Collections.IEnumerator AssignKeyCoroutine(InputAction action, bool alt, KeyAssignedEventHandler callback = null)
        {
            Instance.Log($"Assigning key for action {action.ToString()}...");

            for (int i = 0; i < 2; ++i)
                yield return RSLib.Yield.SharedYields.WaitForEndOfFrame;

            yield return new WaitUntil(() => Input.anyKeyDown);

            KeyCode keyDown = KeyCode.None;

            for (int i = s_allKeyCodes.Length - 1; i >= 0; --i)
                if (Input.GetKeyDown(s_allKeyCodes[i]))
                    keyDown = s_allKeyCodes[i];

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Instance.Log($"Cancelling key assignment for action {action.ToString()}.");
                callback?.Invoke(action, alt ? s_inputMap.GetActionKeyCodes(action).altBtn : s_inputMap.GetActionKeyCodes(action).btn, alt);
                s_assignKeyCoroutine = null;
                yield break;
            }

            Instance.Log($"Assigning key {keyDown.ToString()} to {(alt ? "alternate " : "")}button for action {action.ToString()}.");
            s_inputMap.SetActionButton(action, keyDown, alt);
            callback?.Invoke(action, keyDown, alt);
            s_assignKeyCoroutine = null;
        }

        protected override void Awake()
        {
            base.Awake();

            if (_disableMapLoading || !TryLoadMap())
                RestoreDefaultMap();

            s_allKeyCodes = RSLib.Helpers.GetEnumValues<KeyCode>();
            s_allActions = RSLib.Helpers.GetEnumValues<InputAction>();
        }

        //private void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.S))
        //        SaveCurrentMap();
        //}
    }

    public partial class InputManager : Singleton<InputManager>
    {
        private static string SavePath => $"{Application.persistentDataPath}/InputMap.xml";

        public static void SaveCurrentMap()
        {
            Instance.Log("Saving input map...");

            XContainer container = new XElement("InputMap");

            // XContainer content generation.
            for (int i = s_allActions.Length - 1; i >= 0; --i)
            {
                if (!s_inputMap.HasAction(s_allActions[i]))
                    continue;

                XElement actionElement = new XElement(s_allActions[i].ToString());
                (KeyCode btn, KeyCode altBtn) = s_inputMap.GetActionKeyCodes(s_allActions[i]);
                actionElement.Add(new XElement("Btn", btn.ToString()));
                actionElement.Add(new XElement("AltBtn", altBtn.ToString()));

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
    }
}