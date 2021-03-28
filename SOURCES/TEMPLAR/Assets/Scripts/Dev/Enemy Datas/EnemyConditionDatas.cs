namespace Templar.Dev
{
    using RSLib.Extensions;
    using System.Xml.Linq;

    public abstract class EnemyConditionDatas
    {
        public EnemyConditionDatas(XContainer container)
        {
            Deserialize(container);
        }

        public bool Negate { get; private set; }

        protected virtual void Deserialize(XContainer container)
        {
            XElement conditionElement = container as XElement;

            XAttribute negateAttribute = conditionElement.Attribute("Negate");
            if (!negateAttribute.IsNullOrEmpty())
            {
                if (!bool.TryParse(negateAttribute.Value, out bool negate))
                {
                    EnemyDatabase.Instance.LogError($"Could not parse {negateAttribute.Value} to a valid bool value.");
                    return;
                }

                Negate = negate;
            }
        }
    }


    public class HealthMaxEnemyConditionDatas : EnemyConditionDatas
    {
        public const string ID = "HealthMax";

        public HealthMaxEnemyConditionDatas(XContainer container) : base(container)
        {
        }

        public int Threshold { get; private set; }

        protected override void Deserialize(XContainer container)
        {
            base.Deserialize(container);

            XElement healthMaxElement = container as XElement;

            XAttribute thresholdAttribute = healthMaxElement.Attribute("Threshold");
            UnityEngine.Assertions.Assert.IsFalse(thresholdAttribute.IsNullOrEmpty(), "HealthMax element needs a Threshold attribute.");

            Threshold = thresholdAttribute.ValueToInt();
            UnityEngine.Assertions.Assert.IsTrue(Threshold > 0, "HealthMax condition threshold must be higher than 0.");
        }
    }


    public class HealthMinEnemyConditionDatas : EnemyConditionDatas
    {
        public const string ID = "HealthMin";

        public HealthMinEnemyConditionDatas(XContainer container) : base(container)
        {
        }

        public int Threshold { get; private set; }

        protected override void Deserialize(XContainer container)
        {
            base.Deserialize(container);

            XElement healthMinElement = container as XElement;

            XAttribute thresholdAttribute = healthMinElement.Attribute("Threshold");
            UnityEngine.Assertions.Assert.IsFalse(thresholdAttribute.IsNullOrEmpty(), "HealthMin element needs a Threshold attribute.");

            Threshold = thresholdAttribute.ValueToInt();
            UnityEngine.Assertions.Assert.IsTrue(Threshold > 0, "HealthMin condition threshold must be higher than 0.");
        }
    }


    public class FullHealthEnemyConditionDatas : EnemyConditionDatas
    {
        public const string ID = "FullHealth";

        public FullHealthEnemyConditionDatas(XContainer container) : base(container)
        {
        }
    }

    
    public class PlayerAboveEnemyConditionDatas : EnemyConditionDatas
    {
        public const string ID = "PlayerAbove";

        public PlayerAboveEnemyConditionDatas(XContainer container) : base(container)
        {
        }
    }


    public class PlayerAliveEnemyConditionDatas : EnemyConditionDatas
    {
        public const string ID = "PlayerAlive";

        public PlayerAliveEnemyConditionDatas(XContainer container) : base(container)
        {
        }
    }


    public class PlayerDetectedEnemyConditionDatas : EnemyConditionDatas
    {
        public const string ID = "PlayerDetected";

        public PlayerDetectedEnemyConditionDatas(XContainer container) : base(container)
        {
        }
    }


    public class PlayerInRangeEnemyConditionDatas : EnemyConditionDatas
    {
        public const string ID = "PlayerInRange";

        public PlayerInRangeEnemyConditionDatas(XContainer container) : base(container)
        {
        }

        public float Range { get; private set; }

        protected override void Deserialize(XContainer container)
        {
            base.Deserialize(container);

            XElement rangeElement = container as XElement;

            XAttribute rangeAttribute = rangeElement.Attribute("Range");
            UnityEngine.Assertions.Assert.IsFalse(rangeAttribute.IsNullOrEmpty(), "PlayerInRange element needs a Range attribute.");

            Range = rangeAttribute.ValueToFloat();
            UnityEngine.Assertions.Assert.IsTrue(Range > 0f, "PlayerInRange condition range must be higher than 0.");
        }
    }


    public class RandomChanceConditionDatas : EnemyConditionDatas
    {
        public const string ID = "RandomChance";

        public RandomChanceConditionDatas(XContainer container) : base(container)
        {
        }

        public float Chance { get; private set; }

        protected override void Deserialize(XContainer container)
        {
            base.Deserialize(container);

            XElement rndChanceElement = container as XElement;

            XAttribute chanceAttribute = rndChanceElement.Attribute("Chance");
            UnityEngine.Assertions.Assert.IsFalse(chanceAttribute.IsNullOrEmpty(), "RandomChance element needs a Chance attribute.");

            Chance = chanceAttribute.ValueToFloat();
            UnityEngine.Assertions.Assert.IsTrue(Chance > 0f && Chance < 1f, "RandomChance condition chance value must be between 0 and 1.");
        }
    }
}