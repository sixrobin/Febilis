namespace Templar.Interaction.Dialogue
{
    public interface INPCSpeaker : ISpeaker
    {
        UnityEngine.Transform PlayerDialoguePivot { get; }
    }
}