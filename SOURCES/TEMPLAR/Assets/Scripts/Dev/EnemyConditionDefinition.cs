namespace Templar.Dev
{
    using RSLib.Extensions;
    using System.Xml.Linq;

    public abstract class EnemyConditionDefinition
    {
        public EnemyConditionDefinition(XContainer container)
        {
            Deserialize(container);
        }

        protected virtual void Deserialize(XContainer container)
        {
        }
    }

    public class HealthMaxEnemyConditionDefinition : EnemyConditionDefinition
    {
        public const string ID = "HealthMax";

        public HealthMaxEnemyConditionDefinition(XContainer container) : base(container)
        {
        }

        public int Threshold { get; private set; }

        protected override void Deserialize(XContainer container)
        {
            base.Deserialize(container);

            XElement healthMaxElement = container as XElement;
            Threshold = healthMaxElement.ValueToInt();
            UnityEngine.Assertions.Assert.IsTrue(Threshold > 0, "HealthMax condition threshold must be higher than 0.");
        }
    }

    public class HealthMinEnemyConditionDefinition : EnemyConditionDefinition
    {
        public const string ID = "HealthMin";

        public HealthMinEnemyConditionDefinition(XContainer container) : base(container)
        {
        }

        public int Threshold { get; private set; }

        protected override void Deserialize(XContainer container)
        {
            base.Deserialize(container);

            XElement healthMinElement = container as XElement;
            Threshold = healthMinElement.ValueToInt();
            UnityEngine.Assertions.Assert.IsTrue(Threshold > 0, "HealthMin condition threshold must be higher than 0.");
        }
    }

    public class FullHealthEnemyConditionDefinition : EnemyConditionDefinition
    {
        public const string ID = "FullHealth";

        public FullHealthEnemyConditionDefinition(XContainer container) : base(container)
        {
        }
    }

    public class PlayerUndetectedEnemyConditionDefinition : EnemyConditionDefinition
    {
        public const string ID = "PlayerUndetected";

        public PlayerUndetectedEnemyConditionDefinition(XContainer container) : base(container)
        {
        }
    }

    public class RandomChanceConditionDefinition : EnemyConditionDefinition
    {
        public const string ID = "RandomChance";

        public RandomChanceConditionDefinition(XContainer container) : base(container)
        {
        }

        public float Chance { get; private set; }

        protected override void Deserialize(XContainer container)
        {
            base.Deserialize(container);

            XElement rndChanceElement = container as XElement;
            Chance = rndChanceElement.ValueToFloat();
            UnityEngine.Assertions.Assert.IsTrue(Chance > 0f && Chance < 1f, "RandomChance condition chance value must be between 0 and 1.");
        }
    }
}