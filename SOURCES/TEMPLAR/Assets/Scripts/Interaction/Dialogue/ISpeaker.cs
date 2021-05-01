namespace Templar.Interaction.Dialogue
{
    public interface ISpeaker
    {
        string SpeakerId { get; }
        UnityEngine.Vector3 SpeakerPos { get; }
        UnityEngine.Vector3 PlayerDialoguePos { get; }
        
        void OnSentenceStart();
        void OnSentenceStop();
    }
}