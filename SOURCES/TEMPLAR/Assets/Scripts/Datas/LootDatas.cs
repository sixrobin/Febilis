namespace Templar.Datas
{
    using RSLib.Extensions;
    using System.Xml.Linq;

    public class LootDatas : Datas
    {
        public LootDatas(XContainer container) : base(container)
        {
        }

        public int Value { get; protected set; }
        public float Chance { get; protected set; }

        public override void Deserialize(XContainer container)
        {
            XElement lootElement = container as XElement;

            XElement valueElement = lootElement.Element("Value");
            UnityEngine.Assertions.Assert.IsFalse(valueElement.IsNullOrEmpty(), "Loot datas need a Value element.");
            Value = valueElement.ValueToInt();

            XElement chanceElement = lootElement.Element("Chance");
            Chance = chanceElement?.ValueToFloat() ?? 1f;
            UnityEngine.Assertions.Assert.IsTrue(Chance >= 0f && Chance <= 1f, $"Loot chance is equal to {Chance} and should be clamped between 0 and 1.");
        }
    }
}