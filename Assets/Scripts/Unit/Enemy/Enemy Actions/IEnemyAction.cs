namespace Templar.Unit.Enemy.Actions
{
    public interface IEnemyAction
    {
        bool ShouldApplyGravity { get; }
        bool CantBeHurt { get; }

        bool CheckConditions();
        bool CanExit();
        void Execute();
        void OnEnter();
        void OnExit();
    }
}