namespace Templar.Unit
{
    public interface IPlayerAboveHandler
    {
        bool IsPlayerAbove { get; set; }

        void SetPlayerAbove(bool state);
    }
}