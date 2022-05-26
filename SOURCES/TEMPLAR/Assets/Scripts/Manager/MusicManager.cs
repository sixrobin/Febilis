namespace Templar.Manager
{
    using UnityEngine;

    public class MusicManager : RSLib.Framework.SingletonConsolePro<MusicManager>
    {
        [Header("MAIN THEME")]
        [SerializeField] private RSLib.Audio.ClipProvider _mainTheme = null;
        [SerializeField] private RSLib.Audio.MusicTransitionsDatas _mainThemeTransitionsDatas = null;
        
        [Header("LEVEL")]
        [SerializeField] private RSLib.Audio.ClipProvider _levelMusic = null;
        [SerializeField] private RSLib.Audio.MusicTransitionsDatas _levelMusicTransitionsDatas = null;

        public static void PlayMainTheme()
        {
            RSLib.Audio.AudioManager.PlayMusic(Instance._mainTheme, Instance._mainThemeTransitionsDatas != null ? Instance._mainThemeTransitionsDatas : RSLib.Audio.MusicTransitionsDatas.Instantaneous);
        }
        
        public static void PlayLevelMusic()
        {
            RSLib.Audio.AudioManager.PlayMusic(Instance._levelMusic, Instance._levelMusicTransitionsDatas != null ? Instance._levelMusicTransitionsDatas : RSLib.Audio.MusicTransitionsDatas.Instantaneous);
        }
        
        public static void PlayBossMusic(Boss.BossFight bossFight)
        {
            RSLib.Audio.AudioManager.PlayMusic(bossFight.BossMusicClipProvider, RSLib.Audio.MusicTransitionsDatas.Instantaneous);
        }

        public static void StopMusic()
        {
            RSLib.Audio.AudioManager.StopMusic(0f, RSLib.Maths.Curve.Linear);
        }

        public static void StopMusic(float duration, RSLib.Maths.Curve curve)
        {
            RSLib.Audio.AudioManager.StopMusic(duration, curve);
        }
    }
}
