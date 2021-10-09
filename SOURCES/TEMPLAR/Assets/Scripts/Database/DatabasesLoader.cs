namespace Templar.Database
{
    using System.Linq;
    using UnityEngine;

    [DisallowMultipleComponent]
    public sealed class DatabasesLoader : MonoBehaviour
    {
        private void Awake()
        {
            IDatabase[] _databases = GetComponentsInChildren<IDatabase>();
            _databases = RSLib.Framework.TopologicSorter.Sort(_databases).ToArray();

            for (int i = 0; i < _databases.Length; ++i)
            {
                CProLogger.Log(this, $"Loading IDatabase {_databases[i].GetType().Name}.", gameObject);
                _databases[i].Load();
            }
        }
    }
}