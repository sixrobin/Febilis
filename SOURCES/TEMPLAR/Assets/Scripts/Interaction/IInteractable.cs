namespace Templar.Interaction
{
    public interface IInteractable
    {
        string[] ValidItems { get; }

        void Focus();
        void Unfocus();
        void Interact();
    }
}