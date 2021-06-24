namespace Templar.Item
{
    public class Item
    {
        public Item(string id)
        {
            Id = id;
            Datas = Database.ItemDatabase.ItemsDatas[Id];
        }

        public string Id { get; private set; }

        public Datas.Item.ItemDatas Datas { get; private set; }
    }
}