namespace Templar.Datas.Attack
{
    using RSLib.Extensions;
    using System.Xml.Linq;

    public class AttackDatas
    {
        public AttackDatas()
        {
        }

        public AttackDatas(XContainer container)
        {
            Deserialize(container);
        }

        public static EnemyAttackDatas Default => new EnemyAttackDatas()
        {
            Dmg = 10,
            HitDur = 0.1f,
            HitLayer = Templar.Attack.HitLayer.PLAYER,
            HitDirComputationType = Templar.Attack.HitDirComputationType.ATTACK_DIR,
            HitFreezeFrameDur = 0f,
            BaseTraumaDatas = ShakeTraumaDatas.Default,
            HitTraumaDatas = ShakeTraumaDatas.Default
        };

        public string Id { get; protected set; }

        public int Dmg { get; protected set; }

        public float HitDur { get; protected set; }
        public Templar.Attack.HitLayer HitLayer { get; protected set; }
        public Templar.Attack.HitDirComputationType HitDirComputationType { get; protected set; }

        public float HitFreezeFrameDur { get; protected set; }
        public ShakeTraumaDatas BaseTraumaDatas { get; protected set; }
        public ShakeTraumaDatas HitTraumaDatas { get; protected set; }

        public virtual void Deserialize(XContainer container)
        {
            XElement attackElement = container as XElement;

            XAttribute idAttribute = attackElement.Attribute("Id");
            UnityEngine.Assertions.Assert.IsFalse(idAttribute.IsNullOrEmpty(), "Enemy Id attribute is null or empty.");
            Id = idAttribute.Value;

            XElement dmgElement = attackElement.Element("Dmg");
            UnityEngine.Assertions.Assert.IsNotNull(dmgElement, "AttackDatas must have a Dmg element.");
            Dmg = dmgElement.ValueToInt();

            XElement hitElement = attackElement.Element("Hit");
            UnityEngine.Assertions.Assert.IsFalse(hitElement.IsNullOrEmpty(), "AttackDatas Hit element is null or empty.");

            XElement hitDurElement = hitElement.Element("Dur");
            UnityEngine.Assertions.Assert.IsFalse(hitDurElement.IsNullOrEmpty(), "Hit Dur element is null or empty.");
            HitDur = hitDurElement.ValueToFloat();

            XElement hitLayerElement = hitElement.Element("Layer");
            UnityEngine.Assertions.Assert.IsFalse(hitLayerElement.IsNullOrEmpty(), "Hit Layer element is null or empty.");
            HitLayer = hitLayerElement.ValueToEnum<Templar.Attack.HitLayer>();

            XElement hitDirComputationTypeElement = hitElement.Element("DirComputationType");
            HitDirComputationType = hitDirComputationTypeElement?.ValueToEnum<Templar.Attack.HitDirComputationType>() ?? Templar.Attack.HitDirComputationType.ATTACK_DIR;

            XElement hitFreezeFrameDurElement = hitElement.Element("FreezeFrameDur");
            HitFreezeFrameDur = hitFreezeFrameDurElement?.ValueToFloat() ?? 0f;

            XElement traumasElement = attackElement.Element("Traumas");
            if (traumasElement != null)
            {
                XElement baseTraumaElement = traumasElement.Element("Base");
                if (!baseTraumaElement.IsNullOrEmpty())
                    BaseTraumaDatas = new ShakeTraumaDatas(baseTraumaElement);

                XElement hitTraumaElement = traumasElement.Element("Hit");
                if (!hitTraumaElement.IsNullOrEmpty())
                    HitTraumaDatas = new ShakeTraumaDatas(hitTraumaElement);
            }
        }
    }
}