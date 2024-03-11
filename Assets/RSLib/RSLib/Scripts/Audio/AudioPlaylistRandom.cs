namespace RSLib.Audio
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Random Playlist", menuName = "RSLib/Audio/Playlist/Random")]
    public class AudioPlaylistRandom : ClipProvider
    {
        [SerializeField] private AudioClipPlayDatas[] _clipsPlayDatas = null;

        private Framework.Collections.Loop<AudioClipPlayDatas> _clipsLoop;

        public override AudioClipPlayDatas GetNextClipData()
        {
            if (_clipsLoop == null)
                Init();

            return _clipsLoop.Next();
        }

        [ContextMenu("Init")]
        public override void Init()
        {
            _clipsLoop = new Framework.Collections.Loop<AudioClipPlayDatas>(_clipsPlayDatas, true, true);
        }
    }
}