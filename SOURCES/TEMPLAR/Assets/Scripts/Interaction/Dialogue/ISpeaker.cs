namespace Templar.Interaction.Dialogue
{
    public interface ISpeaker
    {
        string SpeakerId { get; }
        bool IsDialoguing { get; set; }
        UnityEngine.Vector3 SpeakerPos { get; }

        void OnSentenceStart();
        void OnSentenceEnd();
    }
}