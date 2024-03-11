namespace RSLib.Audio
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Random Weighted Playlist", menuName = "RSLib/Audio/Playlist/Random Weighted")]
    public class AudioPlaylistRandomWeighted : ClipProvider
    {
        [System.Serializable]
        private class AudioClipPlayDatasWeighted : AudioClipPlayDatas
        {
            [SerializeField] private float _weight = 1f;

            public float Weight => _weight;
        }

        [SerializeField] private AudioClipPlayDatasWeighted[] _clipsPlayDatas = null;

        private Framework.Collections.WeightedList<AudioClipPlayDatasWeighted> _clipsList;

        public override AudioClipPlayDatas GetNextClipData()
        {
            if (_clipsList == null)
                Init();

            return _clipsList.Peek();
        }

        [ContextMenu("Init")]
        public override void Init()
        {
            _clipsList = new Framework.Collections.WeightedList<AudioClipPlayDatasWeighted>();
            for (int i = _clipsPlayDatas.Length - 1; i >= 0; --i)
                _clipsList.Add(_clipsPlayDatas[i], _clipsPlayDatas[i].Weight);
        }
    }
}