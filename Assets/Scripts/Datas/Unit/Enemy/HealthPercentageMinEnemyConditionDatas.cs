namespace Templar.Datas.Unit.Enemy
{
    using RSLib.Extensions;
    using System.Xml.Linq;

    public class HealthPercentageMinEnemyConditionDatas : EnemyConditionDatas
    {
        public const string ID = "HealthPercentageMin";

        public HealthPercentageMinEnemyConditionDatas(XContainer container) : base(container)
        {
        }

        public float Threshold { get; private set; }

        public override void Deserialize(XContainer container)
        {
            base.Deserialize(container);

            XElement healthMinElement = container as XElement;

            XAttribute thresholdAttribute = healthMinElement.Attribute("Threshold");
            UnityEngine.Assertions.Assert.IsFalse(thresholdAttribute.IsNullOrEmpty(), "HealthPercentageMin element needs a Threshold attribute.");

            Threshold = thresholdAttribute.ValueToFloat();
            UnityEngine.Assertions.Assert.IsTrue(Threshold >= 0f && Threshold <= 1f, "HealthPercentageMin condition threshold must be between 0 and 1.");
        }
    }
}