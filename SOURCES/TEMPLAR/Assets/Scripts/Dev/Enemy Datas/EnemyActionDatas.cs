namespace Templar.Dev
{
    using RSLib.Extensions;
    using System.Xml.Linq;

    public abstract class EnemyActionDatas : EnemyConditionsCheckerDatas
    {
        public EnemyActionDatas(XContainer container) : base(container)
        {
        }
    }

    public class AttackEnemyActionDatas : EnemyActionDatas
    {
        public const string ID = "Attack";

        public AttackEnemyActionDatas(XContainer container) : base(container)
        {
        }

        public string Id { get; private set; }

        public float Delay { get; private set; }

        protected override void Deserialize(XContainer container)
        {
            base.Deserialize(container);

            XElement attackElement = container as XElement;

            XAttribute idAttribute = attackElement.Attribute("Id");
            UnityEngine.Assertions.Assert.IsNotNull(idAttribute, "Attack element must have an Id attribute.");
            Id = idAttribute.Value;

            XElement delayElement = attackElement.Element("Delay");
            if (delayElement != null)
                Delay = delayElement.ValueToFloat();
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

    public class FleeEnemyActionDatas : EnemyActionDatas
    {
        public const string ID = "Flee";

        public FleeEnemyActionDatas(XContainer container) : base(container)
        {
        }
    }
}