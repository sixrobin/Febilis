namespace Templar.Datas
{
    using RSLib.Extensions;
    using System.Linq;
    using System.Xml.Linq;

    public class LootDatas : Datas
    {
        public abstract class Loot
        {
            public Loot(float chance)
            {
                Chance = chance;
            }

            public float Chance { get; private set; }
        }

        public class CoinsLoot : Loot
        {
            public const string ID = "Coins";

            public CoinsLoot(float chance, int value) : base(chance)
            {
                Value = value;
            }

            public int Value { get; private set; }
        }

        public class ItemLoot : Loot
        {
            public const string ID = "Item";

            public ItemLoot(float chance, string itemId) : base(chance)
            {
                ItemId = itemId;
            }

            public string ItemId { get; private set; }
        }

        public LootDatas(XContainer container) : base(container)
        {
        }

        public Loot[] Loots { get; private set; }

        public override void Deserialize(XContainer container)
        {
            XElement lootElement = container as XElement;

            System.Collections.Generic.IEnumerable<XElement> lootSubElements = lootElement.Elements();
            if (lootSubElements?.Count() > 0)
            {
                Loots = new Loot[lootSubElements.Count()];
                int i = 0;

                foreach (XElement lootSubElement in lootSubElements)
                {
                    XAttribute chanceAttribute = lootSubElement.Attribute("Chance");

                    switch (lootSubElement.Name.LocalName)
                    {
                        case CoinsLoot.ID:
                            Loots[i] = new CoinsLoot(chanceAttribute?.ValueToFloat() ?? 1f, lootSubElement.ValueToInt());
                            break;

                        case ItemLoot.ID:
                            Loots[i] = new ItemLoot(chanceAttribute?.ValueToFloat() ?? 1f, lootSubElement.Value);
                            UnityEngine.Assertions.Assert.IsTrue(
                                Database.ItemDatabase.ItemsDatas.ContainsKey(lootSubElement.Value),
                                $"Item Id {lootSubElement.Value} is not referenced in {Database.ItemDatabase.Instance.GetType().Name}.");
                            break;
                    }

                    UnityEngine.Assertions.Assert.IsTrue(
                        Loots[i].Chance >= 0f && Loots[i].Chance <= 1f,
                        $"Loot chance is equal to {Loots[i].Chance} and should be clamped between 0 and 1.");

                    i++;
                }
            }
        }
    }
}