namespace Templar.Manager
{
    using RSLib.Extensions;
    using System.Xml.Linq;

    public partial class FlagsManager : RSLib.Framework.SingletonConsolePro<FlagsManager>
    {
        private static System.Collections.Generic.Dictionary<System.Type, Flags.FlagsList> s_flags;
        private static bool s_init;

        private static void Init(bool force = false)
        {
            if (s_init && !force)
                return;

            s_flags = new System.Collections.Generic.Dictionary<System.Type, Flags.FlagsList>()
            {
                { typeof(Flags.Identifier), new Flags.FlagsList(typeof(Flags.Identifier)) },
                { typeof(Flags.BoardIdentifier), new Flags.FlagsList(typeof(Flags.BoardIdentifier), true) },
                { typeof(Flags.BossIdentifier), new Flags.FlagsList(typeof(Flags.BossIdentifier), true) },
                { typeof(Flags.CheckpointIdentifier), new Flags.FlagsList(typeof(Flags.CheckpointIdentifier), true) },
                { typeof(Flags.ChestIdentifier), new Flags.FlagsList(typeof(Flags.ChestIdentifier)) },
                { typeof(Flags.ItemIdentifier), new Flags.FlagsList(typeof(Flags.ItemIdentifier)) },
                { typeof(Flags.LeverIdentifier), new Flags.FlagsList(typeof(Flags.LeverIdentifier), true) },
                { typeof(Flags.LockIdentifier), new Flags.FlagsList(typeof(Flags.LockIdentifier)) },
                { typeof(Flags.SecretWallIdentifier), new Flags.FlagsList(typeof(Flags.SecretWallIdentifier)) },
                { typeof(Flags.ZoneIdentifier), new Flags.FlagsList(typeof(Flags.ZoneIdentifier)) }
            };

            s_init = true;
        }

        public static void Clear()
        {
            Init(true);
        }

        public static bool Check(Flags.IIdentifiable identifiable)
        {
            return GetFlagsList(identifiable).Check(identifiable);
        }

        public static void Register(Flags.IIdentifiable identifiable)
        {
            GetFlagsList(identifiable).Register(identifiable);
        }

        public static bool CheckBoard(string boardId)
        {
            return s_flags[typeof(Flags.BoardIdentifier)].Check(boardId);
        }

        public static bool CheckZone(string boardId)
        {
            return s_flags[typeof(Flags.ZoneIdentifier)].Check(boardId);
        }

        private static Flags.FlagsList GetFlagsList(Flags.IIdentifiable identifiable)
        {
            System.Type identifierType = identifiable.Identifier.GetType();
            UnityEngine.Assertions.Assert.IsTrue(s_flags.ContainsKey(identifierType), $"Unhandled flag type {identifierType.Name}.");
            return s_flags[identifierType];
        }

        protected override void Awake()
        {
            base.Awake();
            if (!IsValid)
                return;

            Init();
        }
    }

    public partial class FlagsManager : RSLib.Framework.SingletonConsolePro<FlagsManager>
    {
        public static void Load(XElement flagsElement)
        {
            Clear();

            foreach (System.Collections.Generic.KeyValuePair<System.Type, Flags.FlagsList> ids in s_flags)
                if (flagsElement.TryGetElement(ids.Key.Name, out XElement idsElement))
                    ids.Value.Load(idsElement);
        }

        public static XElement Save()
        {
            XElement flagsElement = new XElement("Flags");

            foreach (System.Collections.Generic.KeyValuePair<System.Type, Flags.FlagsList> ids in s_flags)
                flagsElement.Add(ids.Value.Save());

            return flagsElement;
        }
    }
}