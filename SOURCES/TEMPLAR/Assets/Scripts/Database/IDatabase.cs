namespace Templar.Database
{
    public interface IDatabase : RSLib.Framework.ITopologicalSortedItem<IDatabase>
    {
        void Load();
    }
}