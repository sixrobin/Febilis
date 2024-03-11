namespace Templar.Datas.Unit.Enemy
{
    using RSLib.Extensions;
    using System.Xml.Linq;

    public class MaxHeightOffsetConditionDatas : EnemyConditionDatas
    {
        public const string ID = "MaxHeightOffset";

        public MaxHeightOffsetConditionDatas(XContainer container) : base(container)
        {
        }

        public float Offset { get; private set; }

        public override void Deserialize(XContainer container)
        {
            base.Deserialize(container);

            XElement maxHeightOffsetElement = container as XElement;

            XAttribute offsetAttribute = maxHeightOffsetElement.Attribute("Offset");
            UnityEngine.Assertions.Assert.IsFalse(offsetAttribute.IsNullOrEmpty(), "MaxHeightOffset element needs an Offset attribute.");

            Offset = offsetAttribute.ValueToFloat();
            UnityEngine.Assertions.Assert.IsTrue(Offset > 0, "MaxHeightOffset condition offset must be higher than 0.");
        }
    }
}