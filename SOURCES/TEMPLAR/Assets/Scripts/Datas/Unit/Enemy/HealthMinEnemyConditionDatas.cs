namespace Templar.Datas.Unit.Enemy
{
    using RSLib.Extensions;
    using System.Xml.Linq;

    public class HealthMinEnemyConditionDatas : EnemyConditionDatas
    {
        public const string ID = "HealthMin";

        public HealthMinEnemyConditionDatas(XContainer container) : base(container)
        {
        }

        public int Threshold { get; private set; }

        public override void Deserialize(XContainer container)
        {
            base.Deserialize(container);

            XElement healthMinElement = container as XElement;

            XAttribute thresholdAttribute = healthMinElement.Attribute("Threshold");
            UnityEngine.Assertions.Assert.IsFalse(thresholdAttribute.IsNullOrEmpty(), "HealthMin element needs a Threshold attribute.");

            Threshold = thresholdAttribute.ValueToInt();
            UnityEngine.Assertions.Assert.IsTrue(Threshold > 0, "HealthMin condition threshold must be higher than 0.");
        }
    }
}