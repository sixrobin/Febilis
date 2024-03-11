namespace Templar.Database
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Ramp Fade Database", menuName = "Datas/Ramps/Ramp Fade Database")]
    public class RampFadesDatabase : ScriptableObject
    {
        [System.Serializable]
        public class RampFadeItem
        {
            [SerializeField] private string _id = string.Empty;
            [SerializeField] private Datas.RampFadeDatas _fadeDatas = null;

            public string Id => _id;
            public Datas.RampFadeDatas FadeDatas => _fadeDatas;
        }

        [SerializeField] private RampFadeItem[] _fadesDatabase = null;

        public Datas.RampFadeDatas GetRampFade(string id)
        {
            for (int i = _fadesDatabase.Length - 1; i >= 0; --i)
                if (_fadesDatabase[i].Id == id)
                    return _fadesDatabase[i].FadeDatas;

            CProLogger.LogError(this, $"No RampFadeDatas instance has been found for Id {id}.");
            return null;
        }
    }
}