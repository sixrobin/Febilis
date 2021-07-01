namespace Templar.Item
{
    public class Item
    {
        public Item(string id)
        {
            Datas = Database.ItemDatabase.ItemsDatas[id];

            UseConditionsChecker = new ItemActionConditionsChecker(this, Datas.UseConditionsCheckerDatas);
            EquipConditionsChecker = new ItemActionConditionsChecker(this, Datas.EquipConditionsCheckerDatas);
            DropConditionsChecker = new ItemActionConditionsChecker(this, Datas.DropConditionsCheckerDatas);
            MoveConditionsChecker = new ItemActionConditionsChecker(this, Datas.MoveConditionsCheckerDatas);
        }

        public Datas.Item.ItemDatas Datas { get; private set; }
        public string Id => Datas.Id;

        public ItemActionConditionsChecker UseConditionsChecker { get; private set; }
        public ItemActionConditionsChecker EquipConditionsChecker { get; private set; }
        public ItemActionConditionsChecker DropConditionsChecker { get; private set; }
        public ItemActionConditionsChecker MoveConditionsChecker { get; private set; }
    }
}