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
        public float StunDelay { get; private set; }

        public bool CantSuicide { get; private set; }
        
        public ShakeTraumaDatas Trauma { get; private set; }

        public override void Deserialize(XContainer container)
        {
            XElement chargeCollisionElement = container as XElement;

            XElement attackIdElement = chargeCollisionElement.Element("AttackId");
            AttackId = attackIdElement?.Value ?? string.Empty;

            XElement stunElement = chargeCollisionElement.Element("Stun");
            if (stunElement != null)
            {
                XAttribute stunDurAttribute = stunElement.Attribute("Dur");
                StunDur = stunDurAttribute?.ValueToFloat() ?? 0;

                XAttribute stunDelayAttribute = stunElement.Attribute("Delay");
                StunDelay = stunDelayAttribute?.ValueToFloat() ?? 0;
            }

            CantSuicide = chargeCollisionElement.Element("CantSuicide") != null;
                
            XElement traumaElement = chargeCollisionElement.Element("Trauma");
            if (traumaElement != null)
                Trauma = new ShakeTraumaDatas(traumaElement);
        }
    }
}