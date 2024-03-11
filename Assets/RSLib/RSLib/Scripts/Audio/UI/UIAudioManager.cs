namespace RSLib.Audio.UI
{
    using UnityEngine;

    public class UIAudioManager : Framework.Singleton<UIAudioManager>
    {
        [SerializeField] private ClipProvider _genericNavigationClipProvider = null;
        [SerializeField] private ClipProvider _hoverClipProvider = null;
        [SerializeField] private ClipProvider _buttonClickClipProvider = null;
        [SerializeField] private ClipProvider _sliderValueChangedClickClipProvider = null;
        [SerializeField] private ClipProvider _dropdownValueChangedClickClipProvider = null;
        [SerializeField] private ClipProvider _toggleValueChangedClickClipProvider = null;

        [Header("SOUND OVERLAP")]
        [SerializeField, Min(1)] private int _maxClipsPerFrame = 1;
        [SerializeField, Min(1)] private int _frameClearDelay = 1;

        private static int s_playedThisFrame;
        
        public static void PlayGenericNavigationClip()
        {
            if (!Exists())
            {
                Instance.LogWarning($"Trying to play a UI sound while no {nameof(UIAudioManager)} instance exists!");
                return;
            }
            
            PlaySound(Instance._genericNavigationClipProvider);
        }
        
        public static void PlayHoverClip()
        {
            if (!Exists())
            {
                Instance.LogWarning($"Trying to play a UI sound while no {nameof(UIAudioManager)} instance exists!");
                return;
            }
            
            PlaySound(Instance._hoverClipProvider);
        }
        
        public static void PlayButtonClickClip()
        {
            if (!Exists())
            {
                Instance.LogWarning($"Trying to play a UI sound while no {nameof(UIAudioManager)} instance exists!");
                return;
            }
            
            PlaySound(Instance._buttonClickClipProvider);
        }
        
        public static void PlaySliderValueChangedClip()
        {
            if (!Exists())
            {
                Instance.LogWarning($"Trying to play a UI sound while no {nameof(UIAudioManager)} instance exists!");
                return;
            }
            
            PlaySound(Instance._sliderValueChangedClickClipProvider);
        }
                
        public static void PlayDropdownValueChangedClip()
        {
            if (!Exists())
            {
                Instance.LogWarning($"Trying to play a UI sound while no {nameof(UIAudioManager)} instance exists!");
                return;
            }
            
            PlaySound(Instance._dropdownValueChangedClickClipProvider);
        }
                
        public static void PlayToggleValueChangedClip()
        {
            if (!Exists())
            {
                Instance.LogWarning($"Trying to play a UI sound while no {nameof(UIAudioManager)} instance exists!");
                return;
            }
            
            PlaySound(Instance._toggleValueChangedClickClipProvider);
        }

        private static void PlaySound(IClipProvider clipProvider)
        {
            if (s_playedThisFrame == Instance._maxClipsPerFrame)
                return;
            
            AudioManager.PlaySound(clipProvider);

            s_playedThisFrame++;
            if (s_playedThisFrame == 1)
                Instance.StartCoroutine(ClearPlayedThisFrameCoroutine());
        }

        private static System.Collections.IEnumerator ClearPlayedThisFrameCoroutine()
        {
            for (int i = 0; i < Instance._frameClearDelay; ++i)
                yield return RSLib.Yield.SharedYields.WaitForEndOfFrame;
    
            s_playedThisFrame = 0;
        }
    }
}