namespace Templar.Datas.Unit.Enemy
{
    using RSLib.Extensions;
    using System.Xml.Linq;

    public class ChargeActionCollisionDatas : Datas
    {
        public ChargeActionCollisionDatas(XContainer container) : base(container)
        {
        }

        public string AttackId { get; private set; }
        public float StunDur { get; private set; }

        public ShakeTraumaDatas Trauma { get; private set; }

        public override void Deserialize(XContainer container)
        {
            XElement chargeCollisionElement = container as XElement;

            XElement attackIdElement = chargeCollisionElement.Element("AttackId");
            AttackId = attackIdElement?.Value ?? string.Empty;

            XElement stunElement = chargeCollisionElement.Element("Stun");
            UnityEngine.Assertions.Assert.IsFalse(stunElement.IsNullOrEmpty(), $"Stun element is null or empty for charge collision datas.");
            StunDur = stunElement.ValueToFloat();

            XElement traumaElement = chargeCollisionElement.Element("Trauma");
            if (traumaElement != null)
                Trauma = new ShakeTraumaDatas(traumaElement);
        }
    }
}