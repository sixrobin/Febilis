namespace Templar.Datas.Unit.Enemy
{
    using RSLib.Extensions;
    using System.Xml.Linq;

    public class HealthMaxEnemyConditionDatas : EnemyConditionDatas
    {
        public const string ID = "HealthMax";

        public HealthMaxEnemyConditionDatas(XContainer container) : base(container)
        {
        }

        public int Threshold { get; private set; }

        public override void Deserialize(XContainer container)
        {
            base.Deserialize(container);

            XElement healthMaxElement = container as XElement;

            XAttribute thresholdAttribute = healthMaxElement.Attribute("Threshold");
            UnityEngine.Assertions.Assert.IsFalse(thresholdAttribute.IsNullOrEmpty(), "HealthMax element needs a Threshold attribute.");

            Threshold = thresholdAttribute.ValueToInt();
            UnityEngine.Assertions.Assert.IsTrue(Threshold > 0, "HealthMax condition threshold must be higher than 0.");
        }
    }
}