namespace Templar.Database
{
    using System.Linq;

    public class RampsDatabase : RSLib.Framework.ConsoleProSingleton<RampsDatabase>, IDatabase
    {
        [System.Serializable]
        public struct RampsGroup
        {
            public UnityEngine.Texture2D Base;
            public UnityEngine.Texture2D NoWhite;

            public bool Match(UnityEngine.Texture2D ramp)
            {
                return Base == ramp || NoWhite == ramp;
            }
        }

        [UnityEngine.SerializeField] private RampsGroup[] _rampsGroups = null;
        
        void IDatabase.Load()
        {
        }
        
        System.Collections.Generic.IEnumerable<IDatabase> RSLib.Framework.ITopologicalSortedItem<IDatabase>.GetDependencies()
        {
            return null;
        }

        public static RampsGroup GetGroupFromRamp(UnityEngine.Texture2D ramp)
        {
            return Instance._rampsGroups.FirstOrDefault(o => o.Match(ramp));
        }
    }
}