namespace Templar.Unit.Enemy.Actions
{
    public interface IEnemyAction
    {
        bool ShouldApplyGravity { get; }

        bool CheckConditions();
        bool CanExit();
        void Execute();
        void Reset();
    }
}