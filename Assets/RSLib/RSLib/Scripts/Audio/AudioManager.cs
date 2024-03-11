namespace RSLib.Audio
{
    using RSLib.Maths;
    using UnityEngine;
    using UnityEngine.Audio;
    #if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
    #endif

    public class AudioManager : Framework.Singleton<AudioManager>
    {
        [System.Serializable]
        private class SFXPlayer
        {
            [SerializeField, Min(2)] private int _audioSourcesCount = 2;
            [SerializeField] private AudioMixerGroup _mixerGroup = null;

            public int AudioSourcesCount => _audioSourcesCount;
            public AudioMixerGroup MixerGroup => _mixerGroup;
        }

        private class RuntimeSFXPlayer
        {
            private int _nextSourceIndex = 0;
            private AudioSource[] _sources;

            public RuntimeSFXPlayer(int sourcesCount, AudioMixerGroup mixerGroup, Transform sourcesContainer)
            {
                _sources = new AudioSource[sourcesCount];
                for (int i = 0; i < sourcesCount; ++i)
                    _sources[i] = CreateAudioSource($"SFX Source {i}", sourcesContainer, mixerGroup);
            }

            public AudioSource GetNextSource()
            {
                return _sources[_nextSourceIndex++ % _sources.Length];
            }
        }

        #if ODIN_INSPECTOR
        [FoldoutGroup("SFX")]
        #endif
        [SerializeField] private SFXPlayer[] _sfxPlayers = null;
        
        #if ODIN_INSPECTOR
        [FoldoutGroup("Mixers")]
        #endif
        [SerializeField] private AudioMixerGroup _musicMixerGroup = null;
        
        #if ODIN_INSPECTOR
        [FoldoutGroup("Mixers")]
        #endif
        [SerializeField] private AudioMixerGroup _clipsDefaultMixerGroup = null;
        
        #if ODIN_INSPECTOR
        [FoldoutGroup("Mixers")]
        #endif
        [SerializeField] private AudioMixer _audioMixer = null;

        private static System.Collections.Generic.Dictionary<AudioMixerGroup, RuntimeSFXPlayer> s_sfxPlayersDict;
        private static AudioSource[] s_musicSources;
        private static int s_nextMusicIndex;

        private static System.Collections.IEnumerator s_musicFadeCoroutine;

        /// <summary>
        /// Remaps a value from [0.0001f, 1f] to [-80f, 0f], to adjust a linear percentage to the decibels scaling.
        /// </summary>
        /// <param name="value">Value to convert from linear percentage to decibels scale.</param>
        /// <returns>Decibels scaled value.</returns>
        public static float LinearToDecibels(float value)
        {
            return Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        }
        
        /// <summary>
        /// Remaps a value from [-80f, 0f] to [0.0001f, 1f], to adjust a decibel scaled value to a linear percentage.
        /// </summary>
        /// <param name="value">Value to convert from decibel scaling to linear percentage.</param>
        /// <returns>Linear percentage value.</returns>
        public static float DecibelsToLinear(float value)
        {
            return Mathf.Clamp01(Mathf.Pow(10f, value / 20f));
        }

        public static void PlaySound(IClipProvider clipProvider)
        {
            if (!Exists())
            {
                Debug.LogWarning($"Trying to play a sound while no {nameof(AudioManager)} instance exists!");
                return;
            }

            if (clipProvider == null)
            {
                Instance.LogWarning($"Trying to play a sound using a null {nameof(IClipProvider)} reference!");
                return;
            }

            AudioSource source = GetSFXSource(clipProvider);
            AudioClipPlayDatas clipData = clipProvider.GetNextClipData();

            source.clip = clipData.Clip;
            source.volume = clipData.RandomVolume * clipProvider.VolumeMultiplier;
            source.pitch = 1f + clipData.PitchVariation + clipProvider.PitchOffset;
            
            source.Play();
        }
        
        public static void PlaySound(IClipProvider clipProvider, float delay)
        {
            if (delay == 0f)
                PlaySound(clipProvider);
            else
                Instance.StartCoroutine(PlaySoundDelayedCoroutine(clipProvider, delay));
        }

        public static void PlaySound(AudioClip clip, float volume)
        {
            if (clip == null)
            {
                Instance.LogWarning($"Trying to play a sound using a null {nameof(AudioClip)} reference!");
                return;
            }
            
            if (Instance._clipsDefaultMixerGroup == null)
            {
                Instance.LogWarning($"Trying to play an AudioClip using a null {nameof(Instance._clipsDefaultMixerGroup)} reference!");
                return;
            }
            
            AudioSource source = s_sfxPlayersDict[Instance._clipsDefaultMixerGroup].GetNextSource();
            
            source.clip = clip;
            source.volume = volume;
            
            source.Play();
        }
        
        public static void PlaySound(AudioClip clip, float volume, float delay)
        {
            if (delay == 0f)
                PlaySound(clip, volume);
            else
                Instance.StartCoroutine(PlaySoundDelayedCoroutine(clip, volume, delay));
        }
        
        public static void PlaySounds(System.Collections.Generic.IEnumerable<IClipProvider> clipProviders)
        {
            if (!Exists())
            {
                Debug.LogWarning($"Trying to play sounds while no {nameof(AudioManager)} instance exists!");
                return;
            }

            foreach (IClipProvider clipProvider in clipProviders)
                PlaySound(clipProvider);
        }
        
        public static void PlaySounds(System.Collections.Generic.IEnumerable<IClipProvider> clipProviders, float delay)
        {
            if (delay == 0f)
                PlaySounds(clipProviders);
            else
                Instance.StartCoroutine(PlaySoundsDelayedCoroutine(clipProviders, delay));
        }
        
        public static void PlayMusic(IClipProvider musicProvider, MusicTransitionsDatas transitionData)
        {
            if (!Exists())
            {
                LogWarningStatic($"Trying to play music while no {nameof(AudioManager)} instance exists!");
                return;
            }
            
            if (musicProvider == null)
            {
                Instance.LogWarning($"Trying to play a music using a null {nameof(IClipProvider)} reference!");
                return;
            }

            if (transitionData == null)
            {
                Instance.LogWarning($"Trying to play a music using a null {nameof(transitionData)} reference, using default Instantaneous transition!");
                transitionData = MusicTransitionsDatas.Instantaneous;
            }

            AudioSource currentMusicSource = GetCurrentMusicSource();
            if (currentMusicSource.isPlaying && currentMusicSource.clip == musicProvider.GetNextClipData().Clip)
            {
                Instance.Log($"Music {nameof(currentMusicSource)} is already playing, aborting.");
                return;
            }
            
            if (s_musicFadeCoroutine != null)
                Instance.StopCoroutine(s_musicFadeCoroutine);

            if (transitionData.CrossFade)
                Instance.StartCoroutine(s_musicFadeCoroutine = CrossFadeMusicCoroutine(musicProvider, transitionData));
            else
                Instance.StartCoroutine(s_musicFadeCoroutine = FadeMusicCoroutine(musicProvider, transitionData));
        }

        public static void StopMusic(float duration, Curve curve)
        {
            if (!Exists())
            {
                LogWarningStatic($"Trying to stop music while no {nameof(AudioManager)} instance exists!");
                return;
            }

            if (!GetCurrentMusicSource().isPlaying)
            {
                Instance.Log($"Trying to stop music while no music is playing in {nameof(AudioManager)} instance.");
                return;
            }
            
            if (s_musicFadeCoroutine != null)
                Instance.StopCoroutine(s_musicFadeCoroutine);
            
            Instance.StartCoroutine(s_musicFadeCoroutine = FadeOutMusicCoroutine(duration, curve));
        }

        public static bool TryGetMixerFloatParameterValue(string mixerParameterName, out float value)
        {
            return Instance._audioMixer.GetFloat(mixerParameterName, out value);
        }
        
        public static void SetMixerVolumePercentage(string mixerParameterName, float percentage)
        {
            SetMixerVolumeDecibels(mixerParameterName, LinearToDecibels(percentage));
        }
        
        public static void SetMixerVolumeDecibels(string mixerParameterName, float decibels)
        {
            Instance._audioMixer.SetFloat(mixerParameterName, decibels);
        }
        
        private static System.Collections.IEnumerator CrossFadeMusicCoroutine(IClipProvider musicProvider, MusicTransitionsDatas transitionData)
        {
            AudioSource prev = GetCurrentMusicSource();
            AudioSource next = GetNextMusicSource();
            AudioClipPlayDatas clipData = musicProvider.GetNextClipData();

            float prevVol = prev.volume;
            float nextVol = clipData.RandomVolume * musicProvider.VolumeMultiplier;

            next.clip = clipData.Clip;
            next.pitch = 1f + clipData.PitchVariation;
            next.Play();

            for (float t = 0f; t <= 1f; t += Time.deltaTime / transitionData.Duration)
            {
                prev.volume = (1f - t).Ease(transitionData.CurveIn) * prevVol;
                next.volume = t.Ease(transitionData.CurveOut) * nextVol;
                yield return null;
            }

            next.volume = nextVol;
            prev.volume = 0f;
            prev.Stop();
        }

        private static System.Collections.IEnumerator FadeMusicCoroutine(IClipProvider musicProvider, MusicTransitionsDatas transitionData)
        {
            yield return FadeOutMusicCoroutine(transitionData.Duration / 2f, transitionData.CurveOut);
            
            AudioSource next = GetNextMusicSource();
            AudioClipPlayDatas clipData = musicProvider.GetNextClipData();
            float nextVol = clipData.RandomVolume * musicProvider.VolumeMultiplier;

            next.clip = clipData.Clip;
            next.pitch = 1f + clipData.PitchVariation;
            next.Play();

            for (float t = 0f; t <= 1f; t += Time.deltaTime / transitionData.Duration * 2f)
            {
                next.volume = t.Ease(transitionData.CurveOut) * nextVol;
                yield return null;
            }

            next.volume = nextVol;
        }

        private static System.Collections.IEnumerator FadeOutMusicCoroutine(float duration, Curve curve)
        {
            AudioSource source = GetCurrentMusicSource();
            float previousVolume = source.volume;

            for (float t = 1f; t >= 0f; t -= Time.deltaTime / duration)
            {
                source.volume = t.Ease(curve) * previousVolume;
                yield return null;
            }

            source.volume = 0f;
            source.Stop();
        }
        
        private static System.Collections.IEnumerator PlaySoundDelayedCoroutine(IClipProvider clipProvider, float delay)
        {
            yield return RSLib.Yield.SharedYields.WaitForSeconds(delay);
            PlaySound(clipProvider);
        }

        private static System.Collections.IEnumerator PlaySoundDelayedCoroutine(AudioClip clip, float volume, float delay)
        {
            yield return RSLib.Yield.SharedYields.WaitForSeconds(delay);
            PlaySound(clip, volume);
        }
        
        private static System.Collections.IEnumerator PlaySoundsDelayedCoroutine(System.Collections.Generic.IEnumerable<IClipProvider> clipProviders, float delay)
        {
            yield return RSLib.Yield.SharedYields.WaitForSeconds(delay);
            PlaySounds(clipProviders);
        }

        private static AudioSource GetSFXSource(IClipProvider clipProvider)
        {
            UnityEngine.Assertions.Assert.IsTrue(
                s_sfxPlayersDict.ContainsKey(clipProvider.MixerGroup),
                $"No AudioMixerGroup named {clipProvider.MixerGroup.name} has been registered in {Instance.GetType().Name}.");

            return s_sfxPlayersDict[clipProvider.MixerGroup].GetNextSource();
        }

        private static AudioSource GetCurrentMusicSource()
        {
            return s_musicSources[Helpers.Mod(s_nextMusicIndex - 1, s_musicSources.Length)];
        }

        private static AudioSource GetNextMusicSource()
        {
            return s_musicSources[s_nextMusicIndex++ % s_musicSources.Length];
        }

        private static void InitSFXSources()
        {
            s_sfxPlayersDict = new System.Collections.Generic.Dictionary<AudioMixerGroup, RuntimeSFXPlayer>();
            for (int i = 0; i < Instance._sfxPlayers.Length; ++i)
            {
                GameObject newPlayerParent = new GameObject($"SFX Group Player - {Instance._sfxPlayers[i].MixerGroup.name}");
                newPlayerParent.transform.SetParent(Instance.transform);
                s_sfxPlayersDict.Add(
                    Instance._sfxPlayers[i].MixerGroup,
                    new RuntimeSFXPlayer(Instance._sfxPlayers[i].AudioSourcesCount, Instance._sfxPlayers[i].MixerGroup, newPlayerParent.transform));
            }
        }

        private static void InitMusicSources()
        {
            s_musicSources = new AudioSource[2];
            GameObject newPlayerParent = new GameObject($"Music Group Player - {Instance._musicMixerGroup.name}");
            newPlayerParent.transform.SetParent(Instance.transform);

            for (int i = 0; i < 2; ++i)
            {
                s_musicSources[i] = CreateAudioSource($"Music Source {i}", newPlayerParent.transform, Instance._musicMixerGroup);
                s_musicSources[i].loop = true;
            }
        }

        private static AudioSource CreateAudioSource(string gameObjectName, Transform parent, AudioMixerGroup mixerGroup)
        {
            GameObject newSource = new GameObject(gameObjectName);
            newSource.transform.SetParent(parent);
            AudioSource source = newSource.AddComponent<AudioSource>();
            source.outputAudioMixerGroup = mixerGroup;

            return source;
        }

        protected override void Awake()
        {
            base.Awake();
            InitSFXSources();
            InitMusicSources();
            
            RSLib.Debug.Console.DebugConsole.OverrideCommand<string, float>("VolumeSetPercentage", "Sets volume parameter.", SetMixerVolumePercentage);
            RSLib.Debug.Console.DebugConsole.OverrideCommand<string, float>("VolumeSetDecibels", "Sets volume parameter.", SetMixerVolumeDecibels);
        }
    }
}