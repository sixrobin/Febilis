namespace RSLib.Audio
{
    public interface IClipProvider
    {
        UnityEngine.Audio.AudioMixerGroup MixerGroup { get; }
        float VolumeMultiplier { get; }
        float PitchOffset { get; }

        AudioClipPlayDatas GetNextClipData();
    }
}