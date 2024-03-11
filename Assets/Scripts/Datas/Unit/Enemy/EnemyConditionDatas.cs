namespace Templar.Datas.Unit.Enemy
{
    using RSLib.Extensions;
    using System.Xml.Linq;

    public abstract class EnemyConditionDatas : Datas
    {
        public EnemyConditionDatas(XContainer container) : base(container)
        {
        }

        public bool Negate { get; private set; }

        public override void Deserialize(XContainer container)
        {
            XElement conditionElement = container as XElement;

            XAttribute negateAttribute = conditionElement.Attribute("Negate");
            if (!negateAttribute.IsNullOrEmpty())
            {
                if (!bool.TryParse(negateAttribute.Value, out bool negate))
                {
                    Database.EnemyDatabase.Instance.LogError($"Could not parse {negateAttribute.Value} to a valid bool value.");
                    return;
                }

                Negate = negate;
            }
        }
    }
}