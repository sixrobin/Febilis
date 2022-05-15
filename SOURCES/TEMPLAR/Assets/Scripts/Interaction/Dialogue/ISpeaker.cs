namespace Templar.Interaction.Dialogue
{
    public interface ISpeaker
    {
        string SpeakerId { get; }
        UnityEngine.Vector3 SpeakerPos { get; }

        void OnSentenceStart();
        void OnSentenceEnd();
    }
}