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
            Dmg = 20,
            HitDur = 0.1f,
            HitLayer = Templar.Attack.HitLayer.PLAYER,
            HitDirComputationType = Templar.Attack.HitDirComputationType.ATTACK_DIR,
            HitFreezeFrameDur = 0f,
            BaseTraumaDatas = ShakeTraumaDatas.Default,
            HitTraumaDatas = ShakeTraumaDatas.Default,
            AnimSpeedMult = 1f,
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
            if (clipsDurationsElement != null)
            {
                XElement anticipationDurElement = clipsDurationsElement.Element("Anticipation");
                UnityEngine.Assertions.Assert.IsNotNull(anticipationDurElement, "ClipsDurations must have an Anticipation element.");
                AnticipationDur = anticipationDurElement.ValueToFloat();

                UnityEngine.Assertions.Assert.IsTrue(AnticipationDur >= 0, "Anticipation duration can not be negative.");

                XElement attackDurElement = clipsDurationsElement.Element("Attack");
                UnityEngine.Assertions.Assert.IsNotNull(attackDurElement, "ClipsDurations must have an Attack element.");
                AttackDur = attackDurElement.ValueToFloat();

                UnityEngine.Assertions.Assert.IsTrue(AttackDur >= 0, "Attack duration can not be negative.");
            }
        }
    }
}