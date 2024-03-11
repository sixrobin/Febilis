namespace Templar.Datas.Item
{
    using System.Xml.Linq;

    public class ItemActionConditionsCheckerDatas : Datas
    {
        public ItemActionConditionsCheckerDatas(XContainer container) : base(container)
        {
        }

        public System.Collections.Generic.List<ItemActionConditionDatas> Conditions { get; private set; }

        public bool HasConditions => Conditions != null;

        public override void Deserialize(XContainer container)
        {
            if (!(container is XElement conditionsElement))
                return;

            Conditions = new System.Collections.Generic.List<ItemActionConditionDatas>();
            foreach (XElement conditionElement in conditionsElement.Elements())
            {
                switch (conditionElement.Name.LocalName)
                {
                    case FullHealthItemActionConditionDatas.ID:
                        Conditions.Add(new FullHealthItemActionConditionDatas(conditionElement));
                        break;
                    case OnValidInteractableItemActionConditionDatas.ID:
                        Conditions.Add(new OnValidInteractableItemActionConditionDatas(conditionElement));
                        break;
                    default:
                        Database.ItemDatabase.Instance.LogError($"Unknown Item Action Condition Id {conditionElement.Name.LocalName}.");
                        break;
                }
            }
        }
    }
}