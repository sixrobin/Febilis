namespace Templar.Datas
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Ramp Fade Database", menuName = "Datas/Ramps/Ramp Fade Database")]
    public class RampFadesDatabase : ScriptableObject
    {
        [System.Serializable]
        public class RampFadeItem
        {
            [SerializeField] private string _id = string.Empty;
            [SerializeField] private RampFadeDatas _fadeDatas = null;

            public string Id => _id;
            public RampFadeDatas FadeDatas => _fadeDatas;
        }

        [SerializeField] private RampFadeItem[] _fadesDatabase = null;

        public RampFadeDatas GetRampFade(string id)
        {
            for (int i = _fadesDatabase.Length - 1; i >= 0; --i)
                if (_fadesDatabase[i].Id == id)
                    return _fadesDatabase[i].FadeDatas;

            Debug.LogError($"No RampFadeDatas instance has been found for Id {id}.");
            return null;
        }
    }
}