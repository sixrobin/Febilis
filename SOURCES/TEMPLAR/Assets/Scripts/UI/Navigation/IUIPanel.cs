namespace Templar.UI.Navigation
{
    public interface IUIPanel
    {
        UnityEngine.GameObject FirstSelected { get; }

        void Open();
        void Close();
        void OnBackButtonPressed();
    }
}