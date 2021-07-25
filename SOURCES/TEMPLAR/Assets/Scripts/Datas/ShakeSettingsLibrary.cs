namespace Templar.Datas
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Shake Settings Library", menuName = "Datas/Shake/Shakes Library")]
    public class ShakeSettingsLibrary : ScriptableObject
    {
        [System.Serializable]
        public class ShakeSettingsEntry
        {
            [SerializeField] private string _id = string.Empty;
            [SerializeField] private ShakeSettingsDatas _shakeSettings = null;

            public string Id => _id;
            public ShakeSettingsDatas ShakeSettingsDatas => _shakeSettings;
        }

        [SerializeField] private ShakeSettingsEntry[] _shakes = null;

        private System.Collections.Generic.Dictionary<string, ShakeSettingsDatas> _shakesById;

        private System.Collections.Generic.Dictionary<string, ShakeSettingsDatas> ShakesById
        {
            get
            {
                if (_shakesById == null)
                    Init();

                return _shakesById;
            }
        }

        public System.Collections.Generic.Dictionary<string, ShakeSettingsDatas> GetShakes()
        {
            return ShakesById;
        }

        public ShakeSettingsDatas GetShake(string id)
        {
            return ShakesById[id];
        }

        private void Init()
        {
            _shakesById = new System.Collections.Generic.Dictionary<string, ShakeSettingsDatas>();
            for (int i = _shakes.Length - 1; i >= 0; --i)
                _shakesById.Add(_shakes[i].Id, _shakes[i].ShakeSettingsDatas);
        }
    }
}