﻿namespace Templar.Manager
{
    using RSLib.Extensions;
    using System.Xml.Linq;

    public partial class FlagsManager : RSLib.Framework.ConsoleProSingleton<FlagsManager>
    {
        private static System.Collections.Generic.Dictionary<System.Type, Flags.FlagsList> s_flags;
        private static bool s_init;

        private static void Init()
        {
            if (s_init)
                return;

            s_flags = new System.Collections.Generic.Dictionary<System.Type, Flags.FlagsList>()
            {
                { typeof(Flags.Identifier), new Flags.FlagsList(typeof(Flags.Identifier)) },
                { typeof(Flags.ZoneIdentifier), new Flags.FlagsList(typeof(Flags.ZoneIdentifier)) },
                { typeof(Flags.BoardIdentifier), new Flags.FlagsList(typeof(Flags.BoardIdentifier), true) },
                { typeof(Flags.ItemIdentifier), new Flags.FlagsList(typeof(Flags.ItemIdentifier)) },
                { typeof(Flags.ChestIdentifier), new Flags.FlagsList(typeof(Flags.ChestIdentifier)) },
                { typeof(Flags.LockIdentifier), new Flags.FlagsList(typeof(Flags.LockIdentifier)) },
                { typeof(Flags.CheckpointIdentifier), new Flags.FlagsList(typeof(Flags.CheckpointIdentifier), true) }
            };

            s_init = true;
        }

        public static bool Check(Flags.IIdentifiable identifiable)
        {
            return GetFlagsList(identifiable).Check(identifiable);
        }

        public static void Register(Flags.IIdentifiable identifiable)
        {
            GetFlagsList(identifiable).Register(identifiable);
        }

        private static Flags.FlagsList GetFlagsList(Flags.IIdentifiable identifiable)
        {
            System.Type identifierType = identifiable.Identifier.GetType();
            UnityEngine.Assertions.Assert.IsTrue(s_flags.ContainsKey(identifierType), $"Unhandled flag type {identifierType.Name}");
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

    public partial class FlagsManager : RSLib.Framework.ConsoleProSingleton<FlagsManager>
    {
        public static void Load(XElement flagsElement)
        {
            Init();

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