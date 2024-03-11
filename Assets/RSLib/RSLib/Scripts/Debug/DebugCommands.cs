namespace RSLib.Debug.Console
{
    public abstract class CommandBase : System.IComparable
    {
        public CommandBase(string id, string desc, bool showInHistory, bool isConsoleNative)
        {
            Id = id;
            Description = desc;
            IsConsoleNative = isConsoleNative;
            ShowInHistory = showInHistory;
        }

        public string Id { get; private set; }
        public string Description { get; private set; }
        public abstract int ParamsCount { get; }
    
        public bool IsConsoleNative { get; private set; }
        public bool ShowInHistory { get; private set; }

        int System.IComparable.CompareTo(object obj)
        {
            return Id.CompareTo((obj as CommandBase).Id);
        }

        public abstract string GetFormat();
    }

    public class Command : CommandBase
    {
        private System.Action _cmd;

        public Command(string id, string description, System.Action cmd)
            : base(id, description, true, false)
        {
            _cmd = cmd;
        }

        public Command(string id, string description, bool showInHistory, System.Action cmd)
            : base(id, description, showInHistory, false)
        {
            _cmd = cmd;
        }

        public Command(string id, string description, bool showInHistory, bool isConsoleNative, System.Action cmd)
            : base(id, description, showInHistory, isConsoleNative)
        {
            _cmd = cmd;
        }

        public override int ParamsCount => 0;

        public void Execute()
        {
            _cmd.Invoke();
        }

        public override string GetFormat()
        {
            return Id;
        }
    }

    public class Command<T> : CommandBase
    {
        private System.Action<T> _cmd;

        public Command(string id, string description, System.Action<T> cmd)
            : base(id, description, true, false)
        {
            _cmd = cmd;
        }

        public Command(string id, string description, bool showInHistory, System.Action<T> cmd)
            : base(id, description, showInHistory, false)
        {
            _cmd = cmd;
        }

        public Command(string id, string description, bool showInHistory, bool isConsoleNative, System.Action<T> cmd)
            : base(id, description, showInHistory, isConsoleNative)
        {
            _cmd = cmd;
        }

        public override int ParamsCount => 1;

        public void Execute(T param)
        {
            _cmd.Invoke(param);
        }
    
        public override string GetFormat()
        {
            return $"{Id} [{DebugConsole.Constants.TypesFormats[typeof(T)]}]";
        }
    }

    public class Command<T1, T2> : CommandBase
    {
        private System.Action<T1, T2> _cmd;

        public Command(string id, string description, System.Action<T1, T2> cmd)
            : base(id, description, true, false)
        {
            _cmd = cmd;
        }

        public Command(string id, string description, bool showInHistory, System.Action <T1, T2> cmd)
            : base(id, description, showInHistory, false)
        {
            _cmd = cmd;
        }

        public Command(string id, string description, bool showInHistory, bool isConsoleNative, System.Action<T1, T2> cmd)
            : base(id, description, showInHistory, isConsoleNative)
        {
            _cmd = cmd;
        }

        public override int ParamsCount => 2;

        public void Execute(T1 param1, T2 param2)
        {
            _cmd.Invoke(param1, param2);
        }

        public override string GetFormat()
        {
            return $"{Id} [{DebugConsole.Constants.TypesFormats[typeof(T1)]}] [{DebugConsole.Constants.TypesFormats[typeof(T2)]}]";
        }
    }
}