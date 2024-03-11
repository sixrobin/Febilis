namespace Templar.Boards
{
    /// <summary>
    /// Every entity that managers a transition between two boards, whether it is in the same scene, different scenes, or
    /// anything else similar, should implement this interface.
    /// The method can be left empty as it mostly exists to regroup different entities together.
    /// </summary>
    public interface IBoardTransitionHandler
    {
        void OnBoardsTransitionBegan();
        void OnBoardsTransitionOver();
    }
}