namespace Templar.Item
{
    public abstract class Item
    {
        public Item(string id, ItemType type)
        {
            Id = id;
            Type = type;
        }

        public string Id { get; private set; }
        public ItemType Type { get; private set; }
    }
}