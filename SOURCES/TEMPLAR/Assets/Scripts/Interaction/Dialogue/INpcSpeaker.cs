namespace Templar.Interaction.Dialogue
{
    public interface INpcSpeaker : ISpeaker
    {
        UnityEngine.Transform PlayerDialoguePivot { get; }
    }
}