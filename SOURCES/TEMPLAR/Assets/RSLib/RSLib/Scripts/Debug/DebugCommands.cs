namespace Doomlike.Console
{
    public abstract class DebugCommandBase : System.IComparable
    {
        public DebugCommandBase(string id, string desc, bool showInHistory, bool isConsoleNative)
        {
            Id = id;
            Description = desc;
            IsConsoleNative = isConsoleNative;
            ShowInHistory = showInHistory;
        }

        public string Id { get; private set; }

        public string Description { get; private set; }

        public abstract int ParametersCount { get; }
    
        public bool IsConsoleNative { get; private set; }

        public bool ShowInHistory { get; private set; }

        public int CompareTo(object obj)
        {
            return Id.CompareTo((obj as DebugCommandBase).Id);
        }

        public abstract string GetFormat();
    }

    public class DebugCommand : DebugCommandBase
    {
        private System.Action _cmd;

        public DebugCommand(string id, string description, System.Action cmd)
            : base(id, description, true, false)
        {
            _cmd = cmd;
        }

        public DebugCommand(string id, string description, bool showInHistory, System.Action cmd)
            : base(id, description, showInHistory, false)
        {
            _cmd = cmd;
        }

        public DebugCommand(string id, string description, bool showInHistory, bool isConsoleNative, System.Action cmd)
            : base(id, description, showInHistory, isConsoleNative)
        {
            _cmd = cmd;
        }

        public override int ParametersCount => 0;

        public void Execute()
        {
            _cmd.Invoke();
        }

        public override string GetFormat()
        {
            return Id;
        }
    }

    public class DebugCommand<T> : DebugCommandBase
    {
        private System.Action<T> _cmd;

        public DebugCommand(string id, string description, System.Action<T> cmd)
            : base(id, description, true, false)
        {
            _cmd = cmd;
        }

        public DebugCommand(string id, string description, bool showInHistory, System.Action<T> cmd)
            : base(id, description, showInHistory, false)
        {
            _cmd = cmd;
        }

        public DebugCommand(string id, string description, bool showInHistory, bool isConsoleNative, System.Action<T> cmd)
            : base(id, description, showInHistory, isConsoleNative)
        {
            _cmd = cmd;
        }

        public override int ParametersCount => 1;

        public void Execute(T param)
        {
            _cmd.Invoke(param);
        }
    
        public override string GetFormat()
        {
            return $"{Id} [{DebugConsole.Constants.TypesFormats[typeof(T)]}]";
        }
    }

    public class DebugCommand<T1, T2> : DebugCommandBase
    {
        private System.Action<T1, T2> _cmd;

        public DebugCommand(string id, string description, System.Action<T1, T2> cmd)
            : base(id, description, true, false)
        {
            this._cmd = cmd;
        }

        public DebugCommand(string id, string description, bool showInHistory, System.Action <T1, T2> cmd)
            : base(id, description, showInHistory, false)
        {
            this._cmd = cmd;
        }

        public DebugCommand(string id, string description, bool showInHistory, bool isConsoleNative, System.Action<T1, T2> cmd)
            : base(id, description, showInHistory, isConsoleNative)
        {
            this._cmd = cmd;
        }

        public override int ParametersCount => 2;

        public void Execute(T1 param1, T2 param2)
        {
            this._cmd.Invoke(param1, param2);
        }

        public override string GetFormat()
        {
            return $"{Id} [{DebugConsole.Constants.TypesFormats[typeof(T1)]}] [{DebugConsole.Constants.TypesFormats[typeof(T2)]}]";
        }
    }
}