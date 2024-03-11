namespace RSLib.Audio
{
    public abstract class ClipProvider : UnityEngine.ScriptableObject, IClipProvider
    {
        [UnityEngine.SerializeField] private UnityEngine.Audio.AudioMixerGroup _mixerGroup = null;
        [UnityEngine.SerializeField, UnityEngine.Range(0f, 1f)] private float _volumeMultiplier = 1f;
        [UnityEngine.SerializeField, UnityEngine.Range(-4f, 2f)] private float _pitchOffset = 0f;
        
        public UnityEngine.Audio.AudioMixerGroup MixerGroup => _mixerGroup;
        public float VolumeMultiplier => _volumeMultiplier;
        public float PitchOffset => _pitchOffset;

        public virtual void Init() { }
        public abstract AudioClipPlayDatas GetNextClipData();

        #if UNITY_EDITOR
        [UnityEngine.ContextMenu("Debug Play Sound")]
        public void DebugPlaySound()
        {
            if (!UnityEngine.Application.isPlaying)
            {
                UnityEngine.Debug.LogWarning($"Cannot play {this.name} while application is not playing!");
                return;
            }
            
            AudioManager.PlaySound(this);
        }
        #endif
    }
}