namespace Templar.Datas.Unit.Enemy
{
    using RSLib.Extensions;
    using System.Xml.Linq;

    public class PlayerInRangeEnemyConditionDatas : EnemyConditionDatas
    {
        public const string ID = "PlayerInRange";

        public PlayerInRangeEnemyConditionDatas(XContainer container) : base(container)
        {
        }

        public float Range { get; private set; }
        public float RangeSqr => Range * Range;

        public override void Deserialize(XContainer container)
        {
            base.Deserialize(container);

            XElement rangeElement = container as XElement;

            XAttribute rangeAttribute = rangeElement.Attribute("Range");
            UnityEngine.Assertions.Assert.IsFalse(rangeAttribute.IsNullOrEmpty(), "PlayerInRange element needs a Range attribute.");

            Range = rangeAttribute.ValueToFloat();
            UnityEngine.Assertions.Assert.IsTrue(Range > 0f, "PlayerInRange condition range must be higher than 0.");
        }
    }
}