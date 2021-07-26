namespace Templar.Boards
{
    public interface IBoardTransitionHandler
    {
        void OnBoardsTransitionBegan();
        void OnBoardsTransitionOver();

        // IBoardTransitionHandler GetTargetTransitionHandler() ?
    }
}