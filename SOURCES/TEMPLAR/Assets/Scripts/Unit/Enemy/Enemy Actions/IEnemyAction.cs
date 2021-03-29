namespace Templar.Unit.Enemy.Actions
{
    public interface IEnemyAction
    {
        bool CheckConditions();
        bool CanExit();

        void Execute();
        void Reset();
    }
}