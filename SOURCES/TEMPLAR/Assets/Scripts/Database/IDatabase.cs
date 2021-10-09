namespace Templar.Database
{
    public interface IDatabase : RSLib.Framework.ITopologicSortedItem<IDatabase>
    {
        void Load();
    }
}