namespace RSLib.Audio.Demo
{
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    public class AudioManagerDemo : MonoBehaviour
    {
        [Header("SFX")]
        [SerializeField] private ClipProvider _playlist = null;

        [Header("MUSIC")]
        [SerializeField] private ClipProvider[] _musicProviders = null;
        [SerializeField] private MusicTransitionsDatas _transitionDatas = null;

        private int _nextMusicIndex;

        public void PlayNextPlaylistSound()
        {
            AudioManager.PlaySound(_playlist);
        }

        public void PlayNextMusic()
        {
            AudioManager.PlayMusic(_musicProviders[_nextMusicIndex++ % _musicProviders.Length], _transitionDatas ?? MusicTransitionsDatas.Default);
        }

        public void PlayNextMusicInstantaneous()
        {
            AudioManager.PlayMusic(_musicProviders[_nextMusicIndex++ % _musicProviders.Length], MusicTransitionsDatas.Instantaneous);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(AudioManagerDemo))]
    public class AudioManagerDemoEditor : EditorUtilities.ButtonProviderEditor<AudioManagerDemo>
    {
        protected override void DrawButtons()
        {
            DrawButton("Play Next Playlist Sound", Obj.PlayNextPlaylistSound);

            if (EditorApplication.isPlaying)
            {
                DrawButton("Play Next Track", Obj.PlayNextMusic);
                DrawButton("Play Next Track Instantaneous", Obj.PlayNextMusicInstantaneous);
            }
        }
    }
#endif
}