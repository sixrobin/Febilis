namespace RSLib.Audio
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Single Clip", menuName = "RSLib/Audio/Playlist/Single")]
    public class AudioSingleClip : ClipProvider
    {
        [SerializeField] private AudioClipPlayDatas _clipPlayDatas = null;

        public override AudioClipPlayDatas GetNextClipData()
        {
            return _clipPlayDatas;
        }
    }
}