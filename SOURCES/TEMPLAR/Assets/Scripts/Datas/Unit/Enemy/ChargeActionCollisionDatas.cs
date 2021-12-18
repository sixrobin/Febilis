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
        public bool StopCharge { get; private set; }

        public ShakeTraumaDatas Trauma { get; private set; }

        public override void Deserialize(XContainer container)
        {
            XElement chargeCollisionElement = container as XElement;

            XElement attackIdElement = chargeCollisionElement.Element("AttackId");
            AttackId = attackIdElement?.Value ?? string.Empty;

            StopCharge = chargeCollisionElement.Element("StopCharge") != null;

            XElement traumaElement = chargeCollisionElement.Element("Trauma");
            if (traumaElement != null)
                Trauma = new ShakeTraumaDatas(traumaElement);
        }
    }
}