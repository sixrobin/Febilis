namespace Templar.Datas.Unit.Enemy
{
    using RSLib.Extensions;
    using System.Xml.Linq;

    public class RandomChanceConditionDatas : EnemyConditionDatas
    {
        public const string ID = "RandomChance";

        public RandomChanceConditionDatas(XContainer container) : base(container)
        {
        }

        public float Chance { get; private set; }

        public override void Deserialize(XContainer container)
        {
            base.Deserialize(container);

            XElement rndChanceElement = container as XElement;

            XAttribute chanceAttribute = rndChanceElement.Attribute("Chance");
            UnityEngine.Assertions.Assert.IsFalse(chanceAttribute.IsNullOrEmpty(), "RandomChance element needs a Chance attribute.");

            Chance = chanceAttribute.ValueToFloat();
            UnityEngine.Assertions.Assert.IsTrue(Chance > 0f && Chance < 1f, "RandomChance condition chance value must be between 0 and 1.");
        }
    }
}