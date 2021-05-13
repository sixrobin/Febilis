namespace Templar.Database
{
    using UnityEngine;

    [DisallowMultipleComponent]
    public sealed class DatabasesLoader : MonoBehaviour
    {
        private void Awake()
        {
            IDatabase[] _databases = GetComponentsInChildren<IDatabase>();
            for (int i = _databases.Length - 1; i >= 0; --i)
                _databases[i].Load();
        }
    }
}