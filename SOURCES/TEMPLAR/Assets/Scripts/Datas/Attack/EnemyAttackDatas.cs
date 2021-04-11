namespace Templar.Datas.Attack
{
    using RSLib.Extensions;
    using System.Xml.Linq;

    public class EnemyAttackDatas : AttackDatas
    {
        public EnemyAttackDatas() : base()
        {
        }

        public EnemyAttackDatas(XContainer container) : base(container)
        {
            Deserialize(container);
        }

        public new static EnemyAttackDatas Default => new EnemyAttackDatas()
        {
            Dmg = 10,
            HitDur = 0.1f,
            HitLayer = Templar.Attack.HitLayer.PLAYER,
            HitDirComputationType = Templar.Attack.HitDirComputationType.ATTACK_DIR,
            HitFreezeFrameDur = 0f,
            BaseTraumaDatas = ShakeTraumaDatas.Default,
            HitTraumaDatas = ShakeTraumaDatas.Default,
            AnticipationDur = 0.5f,
            AttackDur = 0.5f
        };

        public float AnticipationDur { get; private set; }
        public float AttackDur { get; private set; }

        public override void Deserialize(XContainer container)
        {
            base.Deserialize(container);

            XElement attackElement = container as XElement;

            XElement clipsDurationsElement = attackElement.Element("ClipsDurations");
            UnityEngine.Assertions.Assert.IsNotNull(clipsDurationsElement, "EnemyAttackDatas must have a ClipsDurations element.");

            XElement anticipationDurElement = clipsDurationsElement.Element("Anticipation");
            UnityEngine.Assertions.Assert.IsNotNull(anticipationDurElement, "ClipsDurations must have an Anticipation element.");
            AnticipationDur = anticipationDurElement.ValueToFloat();

            XElement attackDurElement = clipsDurationsElement.Element("Attack");
            UnityEngine.Assertions.Assert.IsNotNull(attackDurElement, "ClipsDurations must have an Attack element.");
            AttackDur = attackDurElement.ValueToFloat();
        }
    }
}