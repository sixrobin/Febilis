namespace RSLib.Debug.Console
{
    using RSLib.Extensions;
    using System.Collections.Generic;
    using UnityEngine;
    #if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
    #endif
    
    /// <summary>
    /// This console system is used to access code at runtime and in build by referencing methods.
    /// It is very simple and has been made for "quick & dirty" implementation but useful results.
    /// The console does support :
    /// - Methods with no parameter,
    /// - Methods with one parameter of type : bool, float, integer or string,
    /// - Methods with two parameters of type : bool, float, integer or string (any order).
    /// 
    /// Methods can NOT have the same name AND the same count of parameters (the system checks the command ID and the parameters count before their types).
    /// This class is a MonoBehaviour Singleton that should have its instance created before anything else in the game.
    /// To use the console and use custom methods :
    /// - The AddCommand method adds a new command if possible, and has an extra parameter to specify if it should override an existing command of the same type (which might be useful for same scenes/classes reloading),
    /// - The OverrideCommand method is a shortcut to call the AddCommand method with the override system enabled,
    /// - The RemoveCommand method does remove a command by its ID and the possibility to specify the parameters count.
    /// To create a command, simply use **new SB.Debug.Console.DebugCommand<paramsTypes>(params)** constructor.
    /// 
    /// N.B.: This console system is NOT a good implementation of such a feature, simply a basic working one.
    /// </summary>
    [DisallowMultipleComponent]
    public class DebugConsole : RSLib.Framework.Singleton<DebugConsole>
    {
        public static class Constants
        {
            // Used to display the parameters types in a more readable way that something like **System.Int32**.
            public static readonly Dictionary<System.Type, string> TypesFormats = new Dictionary<System.Type, string>()
            {
                { typeof(bool), "bool" },
                { typeof(float), "float" },
                { typeof(int), "int" },
                { typeof(string), "string" }
            };

            // Set as an array of strings to handle lines spacing.
            public static readonly string[] HotkeyHelps = new string[]
            {
                ">>> Use <b>UP</b> and <b>DOWN</b> arrows to navigate through history.",
                ">>> Use <b>ESCAPE</b> to erase your current command <b>or</b> to cancel autocompletion.",
                ">>> Use <b>TAB</b> and <b>SHIFT+TAB</b> to navigate through autocompletion.",
                ">>> Use <b>RETURN</b> to validate autocompletion and <b>BACKSPACE</b> to cancel it."
            };

            public static readonly Dictionary<HistoryLine.Validity, string> ValidityFormats = new Dictionary<HistoryLine.Validity, string>(new RSLib.Framework.Comparers.EnumComparer<HistoryLine.Validity>())
            {
                { HistoryLine.Validity.VALID, "<b>> {0}</b>" },
                { HistoryLine.Validity.INVALID, "<b>> {0}  -  Invalid command!</b>" },
                { HistoryLine.Validity.NEUTRAL, "> {0}" },
                { HistoryLine.Validity.ERROR, "> <b>CMD ERROR:</b> {0}" }
            };

            public const string AUTO_COMPLETION_OPTIONS_SPLIT = "  |  ";
            public const string AUTO_COMPLETION_SELECTED_FORMAT = "<color=white><b>[ {0} ]</b></color>"; // Highlight the autocompletion selection.
            public const string CMD_HELP_FORMAT = "<b><i>{0}</i></b>  -  <i>{1}</i>"; // 0 is command format, 1 is description.
            public const string CONTROL_NAME = "ConsoleInputEntry";
            public const string CURRENT_NAVIGATED_IN_HISTORY_MARKER = "<b>  <<<</b>";
            public const string HELP_TEXT = "Type <b>\"h\"</b> to display available commands and console hotkeys.";

            public const int BOXES_SPACING = 1;
            public const int LINES_SPACING = 16;
            public const int ENTRY_BOX_HEIGHT = 30;
            public const int HELP_BOX_HEIGHT = 96;
        }

        public class HistoryLine
        {
            public struct ValidityComparer : IEqualityComparer<Validity>
            {
                public bool Equals(Validity x, Validity y)
                {
                    return x.Equals(y);
                }

                public int GetHashCode(Validity obj)
                {
                    return obj.GetHashCode();
                }
            }

            public HistoryLine(string cmd, Validity validity, bool isExternalLog)
            {
                Cmd = cmd;
                CmdValidity = validity;
                LinesCount = System.Text.RegularExpressions.Regex.Matches(cmd, "\n").Count + 1;
                IsExternalLog = isExternalLog;
            }

            public enum Validity
            {
                NA,
                VALID,
                INVALID,
                NEUTRAL,
                ERROR
            }

            public string Cmd { get; }
            public Validity CmdValidity { get; }
            public int LinesCount { get; }
            public bool IsExternalLog { get; }
        }

        #if !ODIN_INSPECTOR
        [Header("DATA")]
        #endif
        
        #if ODIN_INSPECTOR
        [FoldoutGroup("Data")]
        #endif
        [SerializeField] private bool _buildEnabled = false;

        #if !ODIN_INSPECTOR
        [Header("STYLE")]
        #endif
        
        #if ODIN_INSPECTOR
        [FoldoutGroup("Style")]
        #endif
        [SerializeField, Min(512)] private int _width = 640;
        
        #if ODIN_INSPECTOR
        [FoldoutGroup("Style")]
        #endif
        [SerializeField, Min(32)] private int _height = 146;
        
        #if ODIN_INSPECTOR
        [FoldoutGroup("Style")]
        #endif
        [SerializeField] private Color _consoleColor = new Color(0f, 0f, 0f, 0.9f);
        
        #if ODIN_INSPECTOR
        [FoldoutGroup("Style")]
        #endif
        [SerializeField] private Color _validColor = new Color(0f, 1f, 0f, 1f);
        
        #if ODIN_INSPECTOR
        [FoldoutGroup("Style")]
        #endif
        [SerializeField] private Color _invalidColor = new Color(1f, 0f, 0f, 1f);
        
        #if ODIN_INSPECTOR
        [FoldoutGroup("Style")]
        #endif
        [SerializeField] private Color _neutralColor = new Color(1f, 1f, 1f, 1f);

        private GUIStyle _consoleStyle;
        private GUIStyle _helpTextStyle;
        private GUIStyle _invalidCmdTextStyle;
        private GUIStyle _autoCompletionTextStyle;
        private GUIStyle _historyLineTextStyle;

        private Vector2 _helpScroll;
        private Vector2 _historyScroll;

        private int _autoCompletionNavIndex = -1;
        private readonly List<CommandBase> _autoCompletionOptions = new List<CommandBase>(16);
        private string _autoCompletionStr;
        private int _historyNavIndex = -1;
        private string _inputStr;
        private string _inputStrBeforeAutoComplete;
        private bool _showHelp;

        private bool _textCursorNeedsRefocus;

        private readonly List<HistoryLine> _cmdsHistory = new List<HistoryLine>();
        private readonly List<CommandBase> _registeredCmds = new List<CommandBase>();

        private Dictionary<HistoryLine.Validity, Color> _colorsByValidity;

        public delegate void DebugConsoleToggledEventHandler(bool state);
        public event DebugConsoleToggledEventHandler DebugConsoleToggled;

        private bool _isOpen;
        public bool IsOpen
        {
            get => _isOpen;
            private set
            {
                _isOpen = value;
                DebugConsoleToggled?.Invoke(_isOpen);
            }
        }

        public static void AddCommand(CommandBase cmd, bool overrideIfCmdExists = false)
        {
            if (!Exists())
                return;

            if (string.IsNullOrEmpty(cmd.Id))
            {
                Instance.LogError("Command ID is null or empty!", Instance.gameObject);
                return;
            }

            if (cmd.Id.Contains(" "))
            {
                Instance.LogError($"Command ID \"<b>{cmd.Id}</b>\" can not contain an empty space!", Instance.gameObject);
                return;
            }

            for (int i = Instance._registeredCmds.Count - 1; i >= 0; --i)
            {
                if (Instance._registeredCmds[i].Id == cmd.Id && Instance._registeredCmds[i].ParamsCount == cmd.ParamsCount)
                {
                    if (overrideIfCmdExists)
                    {
                        if (Instance._registeredCmds[i].IsConsoleNative)
                            Instance.LogError("Can not override a console native command!", Instance.gameObject);
                        else
                            Instance._registeredCmds[i] = cmd;
                    }
                    else
                        Instance.LogWarning($"A command with the same ID \"<b>{cmd.Id}</b>\" and the same parameters count (<b>{cmd.ParamsCount}</b>) is already registered!", Instance.gameObject);

                    return;
                }
            }

            Instance._registeredCmds.Add(cmd);
        }

        public static void AddCommand(string id, string desc, System.Action cmd, bool showInHistory = true, bool isConsoleNative = false)
        {
            AddCommand(new Command(id, desc, showInHistory, isConsoleNative, cmd));
        }
        public static void AddCommand<T>(string id, string desc, System.Action<T> cmd, bool showInHistory = true, bool isConsoleNative = false)
        {
            AddCommand(new Command<T>(id, desc, showInHistory, isConsoleNative, cmd));
        }
        public static void AddCommand<T1, T2>(string id, string desc, System.Action<T1, T2> cmd, bool showInHistory = true, bool isConsoleNative = false)
        {
            AddCommand(new Command<T1, T2>(id, desc, showInHistory, isConsoleNative, cmd));
        }
        
        public static void OverrideCommand(CommandBase cmd)
        {
            AddCommand(cmd, true);
        }

        public static void OverrideCommand(string id, string desc, System.Action cmd, bool showInHistory = true, bool isConsoleNative = false)
        {
            OverrideCommand(new Command(id, desc, showInHistory, isConsoleNative, cmd));
        }
        public static void OverrideCommand<T>(string id, string desc, System.Action<T> cmd, bool showInHistory = true, bool isConsoleNative = false)
        {
            OverrideCommand(new Command<T>(id, desc, showInHistory, isConsoleNative, cmd));
        }
        public static void OverrideCommand<T1, T2>(string id, string desc, System.Action<T1, T2> cmd, bool showInHistory = true, bool isConsoleNative = false)
        {
            OverrideCommand(new Command<T1, T2>(id, desc, showInHistory, isConsoleNative, cmd));
        }
        
        public static void RemoveCommand(string id, int paramsCount = -1)
        {
            if (!Exists())
                return;

            for (int i = Instance._registeredCmds.Count - 1; i >= 0; --i)
            {
                if (Instance._registeredCmds[i].Id != id)
                    continue;
                
                if (paramsCount != -1)
                {
                    if (Instance._registeredCmds[i].ParamsCount != paramsCount)
                        continue;
                    
                    if (Instance._registeredCmds[i].IsConsoleNative)
                    {
                        Instance.LogWarning("Can not remove a console native command!", Instance.gameObject);
                        return;
                    }

                    Instance._registeredCmds.RemoveAt(i);
                    return;
                }
                else
                {
                    if (Instance._registeredCmds[i].IsConsoleNative)
                    {
                        Instance.LogWarning("Can not remove a console native command!", Instance.gameObject);
                        return;
                    }

                    Instance._registeredCmds.RemoveAt(i);
                    return;
                }
            }
        }

        /// <summary>
        /// Logs an entry to the console that is not a command.
        /// </summary>
        /// <param name="log">Message to log.</param>
        public static void LogExternal(string log)
        {
            if (!Exists())
                return;

            if (Instance.IsOpen)
                Instance.StartCoroutine(LogToConsoleCoroutine(log, HistoryLine.Validity.NEUTRAL));
        }

        /// <summary>
        /// Logs an entry to the console that is not a command, formatted as an error.
        /// </summary>
        /// <param name="log">Message to log.</param>
        public static void LogExternalError(string log)
        {
            if (!Exists())
                return;

            if (Instance.IsOpen)
                Instance.StartCoroutine(LogToConsoleCoroutine(log, HistoryLine.Validity.ERROR));
        }

        public void Enable(bool state)
        {
            _buildEnabled = state;
            if (!state && !Application.isEditor)
                IsOpen = false;
        }
           
        private static System.Collections.IEnumerator LogToConsoleCoroutine(string log, HistoryLine.Validity validity)
        {
            yield return new WaitForEndOfFrame();
            Instance._cmdsHistory.Add(new HistoryLine(log, validity, true));
        }

        private float ComputeHistoryHeight()
        {
            float h = 0f;
            for (int i = _cmdsHistory.Count - 1; i >= 0; --i)
                h += _cmdsHistory[i].LinesCount * Constants.LINES_SPACING;

            return h;
        }

        private GUIStyle ComputeConsoleStyle(int h, int w)
        {
            GUIStyle style = new GUIStyle(GUI.skin.box);

            Color[] pix = new Color[w * h];
            for (int i = pix.Length - 1; i >= 0; --i)
                pix[i] = _consoleColor;

            Texture2D result = new Texture2D(w, h);
            result.SetPixels(pix);
            result.Apply();
            style.normal.background = result;

            return style;
        }

        private void ComputeAutoCompletion()
        {
            _autoCompletionOptions.Clear();

            if (string.IsNullOrEmpty(_inputStrBeforeAutoComplete) || string.IsNullOrWhiteSpace(_inputStrBeforeAutoComplete))
                return;

            string inputToLower = _inputStrBeforeAutoComplete.ToLower();

            for (int i = 0, registeredCommandsCount = _registeredCmds.Count; i < registeredCommandsCount; ++i)
                if (_registeredCmds[i].Id.ToLower().StartsWith(inputToLower) || _registeredCmds[i].Id.ExtractCapitalLetters(true).ToLower().StartsWith(inputToLower))
                    _autoCompletionOptions.Add(_registeredCmds[i]);
        }

        private string ConvertAutoCompletionToString()
        {
            _autoCompletionStr = string.Empty;

            for (int i = 0, autoCompletionOptionsCount = _autoCompletionOptions.Count; i < autoCompletionOptionsCount; ++i)
                _autoCompletionStr += _autoCompletionNavIndex == i
                    ? $"{string.Format(Constants.AUTO_COMPLETION_SELECTED_FORMAT, _autoCompletionOptions[i].GetFormat())}{Constants.AUTO_COMPLETION_OPTIONS_SPLIT}"
                    : $"{_autoCompletionOptions[i].GetFormat()}{Constants.AUTO_COMPLETION_OPTIONS_SPLIT}";

            if (!string.IsNullOrEmpty(_autoCompletionStr))
                _autoCompletionStr = _autoCompletionStr.RemoveLast(Constants.AUTO_COMPLETION_OPTIONS_SPLIT.Length);

            return _autoCompletionStr;
        }

        private void HandleInputCommand()
        {
            if (string.IsNullOrEmpty(_inputStr) || string.IsNullOrWhiteSpace(_inputStr))
                return;

            HistoryLine.Validity validity = HistoryLine.Validity.INVALID;
            CommandBase currentCmd = null;

            string inputStr = _inputStr;
            inputStr = inputStr.Trim();
            inputStr = System.Text.RegularExpressions.Regex.Replace(inputStr, @"\s+", " ");
            string[] inputStrProperties = inputStr.Split(' ');

            // Loop through all the registered commands and check if the ID matches the input ID.
            // If so, check if the parameters count also matches, and then try to parse the parameters according to the command type.
            // If the parsing is done correctly then the command is executed, else the command is displayed as "invalid".
            for (int i = _registeredCmds.Count - 1; i >= 0; --i)
            {
                if (inputStrProperties[0] != _registeredCmds[i].Id)
                    continue;

                currentCmd = _registeredCmds[i];

                if (inputStrProperties.Length - 1 != currentCmd.ParamsCount)
                    continue;

                switch (currentCmd.ParamsCount)
                {
                    case 0:
                    {
                        if (currentCmd is Command cmd)
                        {
                            cmd.Execute();
                            validity = HistoryLine.Validity.VALID;
                        }

                        break;
                    }

                    case 1:
                    {
                        if (currentCmd is Command<bool> cmdBool)
                        {
                            if (bool.TryParse(inputStrProperties[1], out bool param))
                            {
                                cmdBool.Execute(param);
                                validity = HistoryLine.Validity.VALID;
                            }

                            break;
                        }

                        if (currentCmd is Command<float> cmdFloat)
                        {
                            if (float.TryParse(inputStrProperties[1], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float param))
                            {
                                cmdFloat.Execute(param);
                                validity = HistoryLine.Validity.VALID;
                            }

                            break;
                        }

                        if (currentCmd is Command<int> cmdInt)
                        {
                            if (int.TryParse(inputStrProperties[1], out int param))
                            {
                                cmdInt.Execute(param);
                                validity = HistoryLine.Validity.VALID;
                            }

                            break;
                        }

                        if (currentCmd is Command<string> cmdString)
                        {
                            cmdString.Execute(inputStrProperties[1]);
                            validity = HistoryLine.Validity.VALID;
                        }

                        break;
                    }

                    case 2:
                    {
                        if (currentCmd is Command<bool, bool> cmdBoolBool)
                        {
                            if (bool.TryParse(inputStrProperties[1], out bool param1)
                                && bool.TryParse(inputStrProperties[2], out bool param2))
                            {
                                cmdBoolBool.Execute(param1, param2);
                                validity = HistoryLine.Validity.VALID;
                            }

                            break;
                        }

                        if (currentCmd is Command<bool, float> cmdBoolFloat)
                        {
                            if (bool.TryParse(inputStrProperties[1], out bool param1)
                                && float.TryParse(inputStrProperties[2], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float param2))
                            {
                                cmdBoolFloat.Execute(param1, param2);
                                validity = HistoryLine.Validity.VALID;
                            }

                            break;
                        }

                        if (currentCmd is Command<bool, int> cmdBoolInt)
                        {
                            if (bool.TryParse(inputStrProperties[1], out bool param1)
                                && int.TryParse(inputStrProperties[2], out int param2))
                            {
                                cmdBoolInt.Execute(param1, param2);
                                validity = HistoryLine.Validity.VALID;
                            }

                            break;
                        }

                        if (currentCmd is Command<bool, string> cmdBoolString)
                        {
                            if (bool.TryParse(inputStrProperties[1], out bool param1))
                            {
                                cmdBoolString.Execute(param1, inputStrProperties[2]);
                                validity = HistoryLine.Validity.VALID;
                            }

                            break;
                        }

                        if (currentCmd is Command<float, bool> cmdFloatBool)
                        {
                            if (float.TryParse(inputStrProperties[1], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float param1)
                                && bool.TryParse(inputStrProperties[2], out bool param2))
                            {
                                cmdFloatBool.Execute(param1, param2);
                                validity = HistoryLine.Validity.VALID;
                            }

                            break;
                        }

                        if (currentCmd is Command<float, float> cmdFloatFloat)
                        {
                            if (float.TryParse(inputStrProperties[1], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float param1)
                                && float.TryParse(inputStrProperties[2], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float param2))
                            {
                                cmdFloatFloat.Execute(param1, param2);
                                validity = HistoryLine.Validity.VALID;
                            }

                            break;
                        }

                        if (currentCmd is Command<float, int> cmdFloatInt)
                        {
                            if (float.TryParse(inputStrProperties[1], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float param1)
                                && int.TryParse(inputStrProperties[2], out int param2))
                            {
                                cmdFloatInt.Execute(param1, param2);
                                validity = HistoryLine.Validity.VALID;
                            }

                            break;
                        }

                        if (currentCmd is Command<float, string> cmdFloatString)
                        {
                            if (float.TryParse(inputStrProperties[1], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float param1))
                            {
                                cmdFloatString.Execute(param1, inputStrProperties[2]);
                                validity = HistoryLine.Validity.VALID;
                            }

                            break;
                        }

                        if (currentCmd is Command<int, bool> cmdIntBool)
                        {
                            if (int.TryParse(inputStrProperties[1], out int param1)
                                && bool.TryParse(inputStrProperties[2], out bool param2))
                            {
                                cmdIntBool.Execute(param1, param2);
                                validity = HistoryLine.Validity.VALID;
                            }

                            break;
                        }

                        if (currentCmd is Command<int, float> cmdIntFloat)
                        {
                            if (int.TryParse(inputStrProperties[1], out int param1)
                                && float.TryParse(inputStrProperties[2], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float param2))
                            {
                                cmdIntFloat.Execute(param1, param2);
                                validity = HistoryLine.Validity.VALID;
                            }

                            break;
                        }

                        if (currentCmd is Command<int, int> cmdIntInt)
                        {
                            if (int.TryParse(inputStrProperties[1], out int param1)
                                && int.TryParse(inputStrProperties[2], out int param2))
                            {
                                cmdIntInt.Execute(param1, param2);
                                validity = HistoryLine.Validity.VALID;
                            }

                            break;
                        }

                        if (currentCmd is Command<int, string> cmdIntString)
                        {
                            if (int.TryParse(inputStrProperties[1], out int param1))
                            {
                                cmdIntString.Execute(param1, inputStrProperties[2]);
                                validity = HistoryLine.Validity.VALID;
                            }

                            break;
                        }

                        if (currentCmd is Command<string, bool> cmdStringBool)
                        {
                            if (bool.TryParse(inputStrProperties[2], out bool param2))
                            {
                                cmdStringBool.Execute(inputStrProperties[1], param2);
                                validity = HistoryLine.Validity.VALID;
                            }

                            break;
                        }

                        if (currentCmd is Command<string, float> cmdStringFloat)
                        {
                            if (float.TryParse(inputStrProperties[2], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float param2))
                            {
                                cmdStringFloat.Execute(inputStrProperties[1], param2);
                                validity = HistoryLine.Validity.VALID;
                            }

                            break;
                        }

                        if (currentCmd is Command<string, int> cmdStringInt)
                        {
                            if (int.TryParse(inputStrProperties[2], out int param2))
                            {
                                cmdStringInt.Execute(inputStrProperties[1], param2);
                                validity = HistoryLine.Validity.VALID;
                            }

                            break;
                        }

                        if (currentCmd is Command<string, string> cmdStringString)
                        {
                            cmdStringString.Execute(inputStrProperties[1], inputStrProperties[2]);
                            validity = HistoryLine.Validity.VALID;
                        }

                        break;
                    }
                }
            }

            if (currentCmd == null || currentCmd.ShowInHistory)
                _cmdsHistory.Add(new HistoryLine(_inputStr, validity, false));

            // Reset console data.
            _inputStr = string.Empty;
            _inputStrBeforeAutoComplete = string.Empty;
            ResetHistoryNavigation();
            _autoCompletionOptions.Clear();
        }

        private void NavigateThroughAutoCompletionOptions(int step)
        {
            if (_autoCompletionOptions.Count == 0)
                return;

            _autoCompletionNavIndex = (_autoCompletionNavIndex + step) % _autoCompletionOptions.Count;
            _autoCompletionNavIndex = Mathf.Clamp(_autoCompletionNavIndex, -1, _autoCompletionOptions.Count - 1);

            _inputStr = _autoCompletionNavIndex == -1
                ? _inputStrBeforeAutoComplete
                : _inputStr = _autoCompletionOptions[_autoCompletionNavIndex].Id;
        }

        private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode)
        {
            // Clear the console style, else it won't be rendered correctly after loading a scene.
            // Not a problem since it's a debug tool although there may be a better way to fix this issue.
            _consoleStyle = null;
            _helpTextStyle = null;
            _invalidCmdTextStyle = null;
            _autoCompletionTextStyle = null;
        }

        private void RegisterNativeCommands()
        {
            ////////////////////////////////////////////////////////////////////////////////////
            //// Commands that are inherent in the console system (clear, help, remove command).
            ////////////////////////////////////////////////////////////////////////////////////

            AddCommand(new Command("h", "Shows available commands and hotkeys.", false, true, () =>
            {
                _showHelp = !_showHelp;
                if (_showHelp)
                    _registeredCmds.Sort();
            }));

            //AddCommand(new DebugCommand("help", "Shows available commands and hotkeys.", false, true, () =>
            //{
            //    _showHelp = !_showHelp;
            //    if (_showHelp)
            //        _registeredCmds.Sort();
            //}));

            AddCommand(new Command("c", "Clears commands history.", false, true, () => _cmdsHistory.Clear()));
            //AddCommand(new DebugCommand("clear", "Clears commands history.", false, true, () => _cmdsHistory.Clear()));
            //AddCommand(new DebugCommand<string, int>("removeCmd", "Removes a registered command by ID and parameters count.", true, true, (id, paramsCount) => RemoveCommand(id, paramsCount)));


            ////////////////////////////////////////////////////////////////////////////////////////////////
            //// Commands that are not inherent in the console but potentially useful in any Unity project :
            ////////////////////////////////////////////////////////////////////////////////////////////////

            AddCommand(new Command("r", "Reloads the currently active scene.", true, true,
                () => UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex)));

            //AddCommand(new DebugCommand("reloadActiveScene", "Reloads the currently active scene.", true, true,
            //    () => UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex)));

            AddCommand(new Command<string>("loadSceneByName", "Loads a scene by its name.", true, true, UnityEngine.SceneManagement.SceneManager.LoadScene));
            AddCommand(new Command<int>("loadSceneByIndex", "Loads a scene by its build settings index.", true, true, UnityEngine.SceneManagement.SceneManager.LoadScene));

            AddCommand(new Command("q", "Quits application.", false, true, () =>
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }));

            AddCommand(new Command("OpenPersistentDataPath", "Opens Application.persistentDataPath folder.", OpenPersistentDataPath));
        }

        private void ResetHistoryNavigation()
        {
            _historyNavIndex = -1;
        }

        private void RewindCommandsHistory(int step)
        {
            int previousHistoryNavIndex = _historyNavIndex;
            _historyNavIndex = Mathf.Clamp(_historyNavIndex + step, -1, _cmdsHistory.Count - 1);

            while (_historyNavIndex > -1 && _historyNavIndex < _cmdsHistory.Count && _cmdsHistory[_cmdsHistory.Count - 1 - _historyNavIndex].IsExternalLog)
            {
                _historyNavIndex = Mathf.Clamp(_historyNavIndex + step, -1, _cmdsHistory.Count - 1);
                if (_historyNavIndex == _cmdsHistory.Count - 1 && step == 1 && _cmdsHistory[_cmdsHistory.Count - 1 - _historyNavIndex].IsExternalLog)
                {
                    _historyNavIndex = previousHistoryNavIndex; // Last cmd of history is an external log, no need to change index.
                    break;
                }
            }

            _inputStr = _historyNavIndex != -1 ? _cmdsHistory[_cmdsHistory.Count - 1 - _historyNavIndex].Cmd : string.Empty;
        }

        private void OnGUI()
        {
            if (!IsOpen)
                return;


            ///////////////////////////////
            //// GUI styles initialization.
            ///////////////////////////////

            if (_consoleStyle == null)
                _consoleStyle = ComputeConsoleStyle(Constants.ENTRY_BOX_HEIGHT, _width);

            if (_helpTextStyle == null)
                _helpTextStyle = new GUIStyle()
                {
                    fontStyle = FontStyle.Italic,
                    alignment = TextAnchor.MiddleLeft,
                    normal = new GUIStyleState() { textColor = new Color(1f, 1f, 1f, 0.33f) }
                };

            if (_invalidCmdTextStyle == null)
                _invalidCmdTextStyle = new GUIStyle()
                {
                    alignment = TextAnchor.MiddleLeft,
                    normal = new GUIStyleState() { textColor = new Color(1f, 0f, 0f, 0.8f) }
                };

            if (_autoCompletionTextStyle == null)
                _autoCompletionTextStyle = new GUIStyle()
                {
                    alignment = TextAnchor.MiddleRight,
                    normal = new GUIStyleState() { textColor = new Color(1f, 1f, 1f, 0.5f) }
                };

            if (_historyLineTextStyle == null)
            {
                _historyLineTextStyle = new GUIStyle() { alignment = TextAnchor.UpperLeft };
                _historyLineTextStyle.normal.textColor = Color.white;
            }


            ////////////////////
            //// Input handling.
            ////////////////////

            Event currentEvent = Event.current;

            if (currentEvent.Equals(Event.KeyboardEvent("quote")))
            {
                IsOpen = false;
                _showHelp = false;
                _inputStr = string.Empty;
                _autoCompletionNavIndex = -1;
                ResetHistoryNavigation();
            }

            // Return : validate command OR validate autocompletion.
            if (currentEvent.Equals(Event.KeyboardEvent("return")))
            {
                if (_historyNavIndex > -1)
                {
                    ResetHistoryNavigation();
                    _textCursorNeedsRefocus = true;
                }
                else if (_autoCompletionNavIndex > -1)
                {
                    _inputStrBeforeAutoComplete = _inputStr;
                    _autoCompletionNavIndex = -1;
                    _textCursorNeedsRefocus = true;
                }
                else
                {
                    HandleInputCommand();
                }
            }

            if (currentEvent.Equals(Event.KeyboardEvent("tab")))
                NavigateThroughAutoCompletionOptions(1);

            if (currentEvent.Equals(Event.KeyboardEvent("#tab"))) // Shift + tab.
                NavigateThroughAutoCompletionOptions(-1);

            // Escape : erase command OR cancel autocompletion.
            if (currentEvent.Equals(Event.KeyboardEvent("escape")))
            {
                if (_autoCompletionNavIndex == -1)
                {
                    _inputStr = string.Empty;
                    ResetHistoryNavigation();
                }
                else
                {
                    _inputStr = _inputStrBeforeAutoComplete;
                    _autoCompletionNavIndex = -1;
                }
            }

            // Cancel autocompletion.
            if (currentEvent.Equals(Event.KeyboardEvent("backspace"))
                || currentEvent.type == EventType.KeyDown
                && (currentEvent.keyCode.ToString().Length == 1 && char.IsLetter(currentEvent.keyCode.ToString()[0]) || char.IsDigit(currentEvent.keyCode.ToString()[currentEvent.keyCode.ToString().Length - 1])))
            {
                if (_autoCompletionNavIndex != -1)
                {
                    _inputStr = _inputStrBeforeAutoComplete;
                    _autoCompletionNavIndex = -1;
                    return;
                }
            }

            if (currentEvent.Equals(Event.KeyboardEvent("up")))
                RewindCommandsHistory(1);

            if (currentEvent.Equals(Event.KeyboardEvent("down")))
                RewindCommandsHistory(-1);


            /////////////////////
            //// Console display.
            /////////////////////

            float y = Screen.height;

            // Help box.
            if (_showHelp)
            {
                float helpBoxPosY = y - Constants.HELP_BOX_HEIGHT - Constants.ENTRY_BOX_HEIGHT - _height - Constants.BOXES_SPACING * 2;
                GUI.Box(new Rect(0f, helpBoxPosY, _width, Constants.HELP_BOX_HEIGHT), string.Empty, _consoleStyle);
                Rect helpViewport = new Rect(0f, 0f, _width - 30f, Constants.LINES_SPACING * 0.5f + Constants.LINES_SPACING * (_registeredCmds.Count + Constants.HotkeyHelps.Length + 1));

                float helpScrollPosY = y - Constants.HELP_BOX_HEIGHT - 25f - _height;
                _helpScroll = GUI.BeginScrollView(new Rect(5f, helpScrollPosY, _width - 10f, Constants.HELP_BOX_HEIGHT - 15f), _helpScroll, helpViewport);

                // Hotkeys.
                for (int i = Constants.HotkeyHelps.Length - 1; i >= 0; --i)
                {
                    Rect hotkeysRect = new Rect(5f, Constants.LINES_SPACING * i, helpViewport.width - 15f, 20f);
                    GUI.Label(hotkeysRect, Constants.HotkeyHelps[i]);
                }

                for (int i = _registeredCmds.Count - 1; i >= 0; --i)
                {
                    CommandBase cmd = _registeredCmds[i];
                    string cmdHelp = string.Format(Constants.CMD_HELP_FORMAT, cmd.GetFormat(), (cmd.IsConsoleNative ? "(Native) " : "") + cmd.Description);
                    Rect cmdHelpRect = new Rect(5f, Constants.LINES_SPACING * (i + Constants.HotkeyHelps.Length + 1), helpViewport.width - 15f, 20f);
                    GUI.Label(cmdHelpRect, cmdHelp);
                }

                GUI.EndScrollView();
            }

            // History box.
            GUI.Box(new Rect(0f, y - _height - Constants.ENTRY_BOX_HEIGHT - Constants.BOXES_SPACING, _width, _height), string.Empty, _consoleStyle);
            Rect historyViewport = new Rect(0f, 0f, _width - 30f, ComputeHistoryHeight());
            _historyScroll = GUI.BeginScrollView(new Rect(5f, y - _height - 25f, _width - 10f, _height - 15f), _historyScroll, historyViewport);

            float lineY = ComputeHistoryHeight();
            for (int i = _cmdsHistory.Count - 1; i >= 0; --i)
            {
                string cmdDisplay = string.Format(Constants.ValidityFormats[_cmdsHistory[i].CmdValidity], _cmdsHistory[i].Cmd).ToColored(_colorsByValidity[_cmdsHistory[i].CmdValidity]);
                if (_historyNavIndex != -1 && i == _cmdsHistory.Count - 1 - _historyNavIndex)
                    cmdDisplay += Constants.CURRENT_NAVIGATED_IN_HISTORY_MARKER;

                lineY -= _cmdsHistory[i].LinesCount * Constants.LINES_SPACING;
                Rect cmdHistoryRect = new Rect(5f, lineY, historyViewport.width - 100f, _cmdsHistory[i].LinesCount * Constants.LINES_SPACING);
                GUI.Label(cmdHistoryRect, cmdDisplay, _historyLineTextStyle);
            }

            GUI.EndScrollView();

            // Entry box.
            GUI.Box(new Rect(0f, y - 30f, _width, Constants.ENTRY_BOX_HEIGHT), string.Empty, _consoleStyle);
            GUI.backgroundColor = new Color(0f, 0f, 0f, 0f);

            GUI.SetNextControlName(Constants.CONTROL_NAME);
            Rect logEntryRect = new Rect(5f, y - 25f, _width - 20f, Constants.ENTRY_BOX_HEIGHT - 5f);
            _inputStr = GUI.TextField(logEntryRect, _inputStr);
            GUI.FocusControl(Constants.CONTROL_NAME);

            if (_textCursorNeedsRefocus)
            {
                TextEditor textEditor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
                if (textEditor != null && !string.IsNullOrEmpty(_inputStr))
                {
                    textEditor.cursorIndex = _inputStr.Length;
                    textEditor.SelectNone();
                }

                _textCursorNeedsRefocus = false;
            }

            // Reset history navigation if something has been typed.
            if (_historyNavIndex != -1 && _inputStr != _cmdsHistory[_cmdsHistory.Count - 1 - _historyNavIndex].Cmd)
                ResetHistoryNavigation();

            // Reset autocompletion navigation if something has been typed.
            if (_autoCompletionNavIndex != -1 && _inputStr != _autoCompletionOptions[_autoCompletionNavIndex].Id)
            {
                _inputStr = _inputStrBeforeAutoComplete;
                _autoCompletionNavIndex = -1;
            }

            // Auto completion.
            if (!string.IsNullOrEmpty(_inputStr) && !string.IsNullOrWhiteSpace(_inputStr))
            {
                if (_autoCompletionNavIndex == -1)
                    _inputStrBeforeAutoComplete = _inputStr;

                ComputeAutoCompletion();
                GUI.Label(logEntryRect, ConvertAutoCompletionToString(), _autoCompletionTextStyle);
            }
            else
            {
                _autoCompletionOptions.Clear();
            }

            // Display help tooltip message.
            if (!_showHelp && (string.IsNullOrEmpty(_inputStr) || string.IsNullOrWhiteSpace(_inputStr)))
                GUI.Label(logEntryRect, Constants.HELP_TEXT, _helpTextStyle);
        }

        [ContextMenu("Open Persistent Data Path")]
        private void OpenPersistentDataPath()
        {
            System.Diagnostics.Process.Start($@"{Application.persistentDataPath}");
        }
        
        protected override void Awake()
        {
            base.Awake();

            if (!IsValid)
                return;

            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;

            _colorsByValidity = new Dictionary<HistoryLine.Validity, Color>(new HistoryLine.ValidityComparer())
            {
                { HistoryLine.Validity.VALID, _validColor },
                { HistoryLine.Validity.INVALID, _invalidColor },
                { HistoryLine.Validity.NEUTRAL, _neutralColor },
                { HistoryLine.Validity.ERROR, _invalidColor }
            };

            RegisterNativeCommands();
        }

        private void Update()
        {
            if ((_buildEnabled || Application.isEditor)
                && !IsOpen
                && Input.GetKeyDown(KeyCode.Quote))
            {
                IsOpen = true;
            }
        }

        private void OnDestroy()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}