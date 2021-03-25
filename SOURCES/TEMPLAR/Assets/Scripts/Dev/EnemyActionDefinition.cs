namespace Templar.Dev
{
    using RSLib.Extensions;
    using System.Xml.Linq;

    public abstract class EnemyActionDefinition : EnemyConditionsDependentDefinition
    {
        public EnemyActionDefinition(XContainer container) : base(container)
        {
        }
    }

    public class AttackActionDefinition : EnemyActionDefinition
    {
        public const string ID = "Attack";

        public AttackActionDefinition(XContainer container) : base(container)
        {
        }

        // [TODO] AttackId.
        // [TODO] Delay.
    }

    public class BackAndForthActionDefinition : EnemyActionDefinition
    {
        public const string ID = "BackAndForth";

        public BackAndForthActionDefinition(XContainer container) : base(container)
        {
        }

        public float Range { get; private set; }
        public (float min, float max) PauseDur { get; private set; }
        public float RangeFluctuationOnPause { get; private set; }

        protected override void Deserialize(XContainer container)
        {
            base.Deserialize(container);

            XContainer backAndForthElement = container as XElement;

            XElement rangeElement = backAndForthElement.Element("Range");
            UnityEngine.Assertions.Assert.IsNotNull(rangeElement, "BackAndForth element must have a Range element.");
            Range = rangeElement.ValueToFloat();

            XElement pauseDur = backAndForthElement.Element("PauseDur");
            if (pauseDur != null)
                PauseDur = pauseDur.MinMaxAttributesToFloats();

            XElement rangeFluctuationOnPauseElement = backAndForthElement.Element("RangeFluctuationOnPause");
            if (rangeFluctuationOnPauseElement != null)
                RangeFluctuationOnPause = rangeFluctuationOnPauseElement.ValueToFloat();
        }
    }
}