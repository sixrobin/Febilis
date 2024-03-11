namespace Templar.Datas.Unit.Enemy
{
    using RSLib.Extensions;
    using System.Xml.Linq;

    public class HealthPercentageMaxEnemyConditionDatas : EnemyConditionDatas
    {
        public const string ID = "HealthPercentageMax";

        public HealthPercentageMaxEnemyConditionDatas(XContainer container) : base(container)
        {
        }

        public float Threshold { get; private set; }

        public override void Deserialize(XContainer container)
        {
            base.Deserialize(container);

            XElement healthMaxElement = container as XElement;

            XAttribute thresholdAttribute = healthMaxElement.Attribute("Threshold");
            UnityEngine.Assertions.Assert.IsFalse(thresholdAttribute.IsNullOrEmpty(), "HealthPercentageMax element needs a Threshold attribute.");

            Threshold = thresholdAttribute.ValueToFloat();
            UnityEngine.Assertions.Assert.IsTrue(Threshold >= 0f && Threshold <= 1f, "HealthPercentageMax condition threshold must be between 0 and 1.");
        }
    }
}