namespace Templar.Datas.Attack
{
    using RSLib.Extensions;
    using System.Xml.Linq;

    public class PlayerAttackDatas : AttackDatas
    {
        public PlayerAttackDatas() : base()
        {
        }

        public PlayerAttackDatas(XContainer container) : base(container)
        {
            Deserialize(container);
        }

        public new static PlayerAttackDatas Default => new PlayerAttackDatas()
        {
            Dmg = 10,
            HitDur = 0.1f,
            HitLayer = Templar.Attack.HitLayer.PLAYER,
            HitDirComputationType = Templar.Attack.HitDirComputationType.ATTACK_DIR,
            HitFreezeFrameDur = 0f,
            BaseTraumaDatas = ShakeTraumaDatas.Default,
            HitTraumaDatas = ShakeTraumaDatas.Default,
            Dur = 0.5f,
            ChainAllowedTime = 0.35f,
            AnimSpeedMult = 1f,
            ControlVelocity = false
        };

        public float Dur { get; private set; }
        public float ChainAllowedTime { get; private set; }

        public bool ControlVelocity { get; private set; }
        public float MoveSpeed { get; private set; }
        public float Gravity { get; private set; }

        public override void Deserialize(XContainer container)
        {
            base.Deserialize(container);

            XElement attackElement = container as XElement;

            XElement durElement = attackElement.Element("Dur");
            UnityEngine.Assertions.Assert.IsNotNull(durElement, "PlayerAttackDatas must have a Dur element.");
            Dur = durElement.ValueToFloat();

            XElement chainAllowedTimeElement = attackElement.Element("ChainAllowedTime");
            ChainAllowedTime = chainAllowedTimeElement?.ValueToFloat() ?? 0f;

            XElement controlVelocityElement = attackElement.Element("ControlVelocity");
            ControlVelocity = !controlVelocityElement.IsNullOrEmpty();

            if (ControlVelocity)
            {
                XElement moveSpeedElement = controlVelocityElement.Element("MoveSpeed");
                UnityEngine.Assertions.Assert.IsNotNull(moveSpeedElement, "ControlVelocity must have a MoveSpeed element.");
                MoveSpeed = moveSpeedElement.ValueToFloat();

                XElement gravityElement = controlVelocityElement.Element("Gravity");
                UnityEngine.Assertions.Assert.IsNotNull(gravityElement, "ControlVelocity must have a Gravity element.");
                Gravity = gravityElement.ValueToFloat();
            }
        }
    }
}