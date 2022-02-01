namespace Templar.Datas.Unit.Enemy
{
    using RSLib.Extensions;
    using System.Xml.Linq;

    public abstract class EnemyActionDatas : EnemyConditionsCheckerDatas
    {
        public EnemyActionDatas(XContainer container) : base(container)
        {
        }

        public bool CantBeHurt { get; private set; }
        public bool DontApplyGravity { get; private set; }

        public override void Deserialize(XContainer container)
        {
            base.Deserialize(container);

            XElement actionElement = container as XElement;

            DontApplyGravity = actionElement.Element("DontApplyGravity") != null;
            CantBeHurt = actionElement.Element("CantBeHurt") != null;
        }
    }


    public class AttackEnemyActionDatas : EnemyActionDatas
    {
        public const string ID = "Attack";

        public AttackEnemyActionDatas(XContainer container) : base(container)
        {
        }

        public string Id { get; private set; }

        public (float min, float max) Delay { get; private set; }

        public string AnimatorId { get; private set; }
        public string AnimatorEnemyIdOverride { get; private set; }

        public override void Deserialize(XContainer container)
        {
            base.Deserialize(container);

            XElement attackElement = container as XElement;

            XAttribute idAttribute = attackElement.Attribute("Id");
            UnityEngine.Assertions.Assert.IsNotNull(idAttribute, "Attack element must have an Id attribute.");
            Id = idAttribute.Value;

            XElement delayElement = attackElement.Element("Delay");
            if (delayElement != null)
                Delay = delayElement.MinMaxAttributesToFloats();

            XElement animatorSuffixElement = attackElement.Element("AnimatorId");
            AnimatorId = animatorSuffixElement?.Value ?? null;

            XElement animatorEnemyIdOverrideElement = attackElement.Element("AnimatorEnemyIdOverride");
            AnimatorEnemyIdOverride = animatorEnemyIdOverrideElement?.Value ?? null;
        }
    }


    public class BackAndForthEnemyActionDatas : EnemyActionDatas
    {
        public const string ID = "BackAndForth";

        public BackAndForthEnemyActionDatas(XContainer container) : base(container)
        {
        }

        public float Range { get; private set; }
        public (float min, float max) PauseDur { get; private set; }
        public float RangeFluctuationOnPause { get; private set; }

        public override void Deserialize(XContainer container)
        {
            base.Deserialize(container);

            XContainer backAndForthElement = container as XElement;

            XElement rangeElement = backAndForthElement.Element("Range");
            UnityEngine.Assertions.Assert.IsNotNull(rangeElement, "BackAndForth element must have a Range element.");
            Range = rangeElement.ValueToFloat();

            XElement pauseDur = backAndForthElement.Element("Pause");
            if (pauseDur != null)
                PauseDur = pauseDur.MinMaxAttributesToFloats();

            XElement rangeFluctuationOnPauseElement = backAndForthElement.Element("RangeFluctuationOnPause");
            if (rangeFluctuationOnPauseElement != null)
                RangeFluctuationOnPause = rangeFluctuationOnPauseElement.ValueToFloat();
        }
    }


    public class ChargeEnemyActionDatas : EnemyActionDatas
    {
        public const string ID = "Charge";

        public ChargeEnemyActionDatas(XContainer container) : base(container)
        {
        }

        public float AnticipationDuration { get; private set; }
        public float InitSpeed { get; private set; }
        public float Acceleration { get; private set; }
        public float MaxDuration { get; private set; }

        public ChargeActionCollisionDatas PlayerCollisionDatas { get; private set; }
        public ChargeActionCollisionDatas WallCollisionDatas { get; private set; }

        public override void Deserialize(XContainer container)
        {
            base.Deserialize(container);

            XContainer chargeElement = container as XElement;

            XElement anticipationDurationElement = chargeElement.Element("AnticipationDuration");
            UnityEngine.Assertions.Assert.IsFalse(anticipationDurationElement.IsNullOrEmpty(), $"AnticipationDuration element is null or empty for charge action.");
            AnticipationDuration = anticipationDurationElement.ValueToFloat();

            XElement speedElement = chargeElement.Element("InitSpeed");
            UnityEngine.Assertions.Assert.IsFalse(speedElement.IsNullOrEmpty(), $"Speed element is null or empty for charge action.");
            InitSpeed = speedElement.ValueToFloat();

            XElement accelerationElement = chargeElement.Element("Acceleration");
            UnityEngine.Assertions.Assert.IsFalse(accelerationElement.IsNullOrEmpty(), $"Acceleration element is null or empty for charge action.");
            Acceleration = accelerationElement.ValueToFloat();

            XElement maxDurationElement = chargeElement.Element("MaxDuration");
            MaxDuration = maxDurationElement?.ValueToFloat() ?? float.PositiveInfinity;

            XElement playerCollisionElement = chargeElement.Element("PlayerCollision");
            if (playerCollisionElement != null)
                PlayerCollisionDatas = new ChargeActionCollisionDatas(playerCollisionElement);

            XElement wallCollisionElement = chargeElement.Element("WallCollision");
            if (wallCollisionElement != null)
                WallCollisionDatas = new ChargeActionCollisionDatas(wallCollisionElement);
        }
    }


    public class ChaseEnemyActionDatas : EnemyActionDatas
    {
        public const string ID = "Chase";

        public ChaseEnemyActionDatas(XContainer container) : base(container)
        {
        }
    }


    public class FleeEnemyActionDatas : EnemyActionDatas
    {
        public const string ID = "Flee";

        public FleeEnemyActionDatas(XContainer container) : base(container)
        {
        }

        public bool FacePlayer { get; private set; }

        public override void Deserialize(XContainer container)
        {
            base.Deserialize(container);

            XContainer fleeElement = container as XElement;
            FacePlayer = fleeElement.Element("FacePlayer") != null;
        }
    }


    public class WaitEnemyActionDatas : EnemyActionDatas
    {
        public const string ID = "Wait";

        public bool FacePlayer { get; private set; }

        public WaitEnemyActionDatas(XContainer container) : base(container)
        {
        }

        public override void Deserialize(XContainer container)
        {
            base.Deserialize(container);

            XContainer waitElement = container as XElement;
            FacePlayer = waitElement.Element("FacePlayer") != null;
        }
    }
}