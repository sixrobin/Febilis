namespace Templar.Datas.Attack
{
    using RSLib.Extensions;
    using System.Xml.Linq;

    public class AttackDatas : Datas
    {
        public AttackDatas() : base(null)
        {
        }

        public AttackDatas(XContainer container) : base(container)
        {
        }

        public static EnemyAttackDatas Default => new EnemyAttackDatas()
        {
            Dmg = 10,
            HitDur = 0.1f,
            HitLayer = Templar.Attack.HitLayer.PLAYER,
            HitDirComputationType = Templar.Attack.HitDirComputationType.ATTACK_DIR,
            HitFreezeFrameDur = 0f,
            BaseTraumaDatas = ShakeTraumaDatas.Default,
            HitTraumaDatas = ShakeTraumaDatas.Default,
            RecoilDatas = RecoilDatas.NullRecoil,
            AnimSpeedMult = 1f
        };

        public static EnemyAttackDatas InfiniteDamage => new EnemyAttackDatas()
        {
            Dmg = int.MaxValue,
            HitDur = 0.1f,
            HitLayer = Templar.Attack.HitLayer.PLAYER,
            HitDirComputationType = Templar.Attack.HitDirComputationType.ATTACK_DIR,
            HitFreezeFrameDur = 0f,
            BaseTraumaDatas = ShakeTraumaDatas.Default,
            HitTraumaDatas = ShakeTraumaDatas.Default,
            RecoilDatas = RecoilDatas.NullRecoil,
            AnimSpeedMult = 1f
        };

        public string Id { get; protected set; }

        public int Dmg { get; protected set; }
        
        public float HitDelay { get; private set; }
        public float HitDur { get; protected set; }
        public Templar.Attack.HitLayer HitLayer { get; protected set; }
        public Templar.Attack.HitDirComputationType HitDirComputationType { get; protected set; }

        public bool Unstoppable { get; private set; }
        public bool ForceHurt { get; private set; }

        public RecoilDatas RecoilDatas { get; private set; }

        public float HitFreezeFrameDur { get; protected set; }
        public ShakeTraumaDatas BaseTraumaDatas { get; protected set; }
        public ShakeTraumaDatas HitTraumaDatas { get; protected set; }

        public float AnimSpeedMult { get; protected set; }

        public override void Deserialize(XContainer container)
        {
            XElement attackElement = container as XElement;

            XAttribute idAttribute = attackElement.Attribute("Id");
            UnityEngine.Assertions.Assert.IsFalse(idAttribute.IsNullOrEmpty(), "Attack Id attribute is null or empty.");
            Id = idAttribute.Value;

            XElement dmgElement = attackElement.Element("Dmg");
            UnityEngine.Assertions.Assert.IsNotNull(dmgElement, $"AttackDatas {Id} must have a Dmg element.");
            Dmg = dmgElement.ValueToInt();

            XElement hitElement = attackElement.Element("Hit");
            UnityEngine.Assertions.Assert.IsFalse(hitElement.IsNullOrEmpty(), $"AttackDatas Hit element is null or empty for attack {Id}.");

            XElement hitDelayElement = hitElement.Element("Delay");
            HitDelay = hitDelayElement?.ValueToFloat() ?? 0f;

            XElement hitDurElement = hitElement.Element("Dur");
            UnityEngine.Assertions.Assert.IsFalse(hitDurElement.IsNullOrEmpty(), $"Hit Dur element is null or empty for attack {Id}.");
            HitDur = hitDurElement.ValueToFloat();

            XElement hitLayersElement = hitElement.Element("Layers");
            UnityEngine.Assertions.Assert.IsFalse(hitLayersElement.IsNullOrEmpty(), $"Layers element is null or empty for attack {Id}.");
            foreach (XElement hitLayerElement in hitLayersElement.Elements("LayerName"))
                HitLayer |= hitLayerElement.ValueToEnum<Templar.Attack.HitLayer>();

            XElement hitDirComputationTypeElement = hitElement.Element("DirComputationType");
            HitDirComputationType = hitDirComputationTypeElement?.ValueToEnum<Templar.Attack.HitDirComputationType>() ?? Templar.Attack.HitDirComputationType.ATTACK_DIR;

            Unstoppable = attackElement.Element("Unstoppable") != null;
            ForceHurt = attackElement.Element("ForceHurt") != null;

            XElement recoilElement = attackElement.Element("Recoil");
            UnityEngine.Assertions.Assert.IsFalse(recoilElement.IsNullOrEmpty(), $"Recoil element is null or empty for attack {Id}.");
            RecoilDatas = new RecoilDatas(recoilElement);

            XElement hitFreezeFrameDurElement = hitElement.Element("FreezeFrameDur");
            HitFreezeFrameDur = hitFreezeFrameDurElement?.ValueToFloat() ?? 0f;

            XElement traumasElement = attackElement.Element("Traumas");
            if (traumasElement != null)
            {
                XElement baseTraumaElement = traumasElement.Element("Base");
                if (baseTraumaElement != null)
                    BaseTraumaDatas = new ShakeTraumaDatas(baseTraumaElement);

                XElement hitTraumaElement = traumasElement.Element("Hit");
                if (hitTraumaElement != null)
                    HitTraumaDatas = new ShakeTraumaDatas(hitTraumaElement);
            }

            XElement animSpeedMultElement = attackElement.Element("AnimSpeedMult");
            AnimSpeedMult = animSpeedMultElement?.ValueToFloat() ?? 1f;
        }
    }
}