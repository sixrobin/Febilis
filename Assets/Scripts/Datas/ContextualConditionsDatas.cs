namespace Templar.Datas.ContextualConditions
{
    using RSLib.Extensions;
    using System.Linq;
    using System.Xml.Linq;

    public class ContextualConditionsDatas : Datas
    {
        private const string ELEMENT_NAME_HAS_ITEM = "HasItem";
        private const string ELEMENT_NAME_DOESNT_HAVE_ITEM = "DoesntHaveItem";

        public ContextualConditionsDatas(XContainer container) : base(container)
        {
        }

        public string Id { get; private set; }

        public ContextualConditionDatas[] ConditionsDatas { get; private set; }

        public override void Deserialize(XContainer container)
        {
            XElement conditionsElement = container as XElement;

            XAttribute idAttribute = conditionsElement.Attribute("Id");
            UnityEngine.Assertions.Assert.IsFalse(idAttribute.IsNullOrEmpty(), "Contextual Conditions Id attribute is null or empty.");
            Id = idAttribute.Value;

            System.Collections.Generic.IEnumerable<XElement> conditionsElements = conditionsElement.Elements();
            ConditionsDatas = new ContextualConditionDatas[conditionsElements.Count()];

            int i = 0;
            foreach (XElement conditionElement in conditionsElements)
            {
                if (conditionElement.Name.LocalName == ELEMENT_NAME_HAS_ITEM)
                {
                    ConditionsDatas[i] = new HasItemContextualConditionDatas(conditionElement);
                }
                else if (conditionElement.Name.LocalName == ELEMENT_NAME_DOESNT_HAVE_ITEM)
                {
                    ConditionsDatas[i] = new DoesntHaveItemContextualConditionDatas(conditionElement);
                }
                else
                {
                    CProLogger.LogError(this, $"Unhandled Contextual Condition name {conditionElement.Name.LocalName}.");
                    break;
                }

                i++;
            }
        }
    }
}