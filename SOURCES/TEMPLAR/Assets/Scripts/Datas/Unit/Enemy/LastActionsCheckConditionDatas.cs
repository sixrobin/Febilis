namespace Templar.Datas.Unit.Enemy
{
    using RSLib.Extensions;
    using System.Xml.Linq;

    public class LastActionsCheckConditionDatas : EnemyConditionDatas
    {
        public const string ID = "LastActionsCheck";

        public LastActionsCheckConditionDatas(XContainer container) : base(container)
        {
        }

        public int ActionsCount { get; private set; }

        public System.Collections.Generic.List<System.Type> Exclude { get; private set; } = new System.Collections.Generic.List<System.Type>();
        public System.Collections.Generic.List<System.Type> Include { get; private set; } = new System.Collections.Generic.List<System.Type>();

        public override void Deserialize(XContainer container)
        {
            System.Type ParseActionType(XElement element)
            {
                switch (element.Value)
                {
                    case AttackEnemyActionDatas.ID: return typeof(Templar.Unit.Enemy.Actions.AttackEnemyAction);
                    case BackAndForthEnemyActionDatas.ID: return typeof(Templar.Unit.Enemy.Actions.BackAndForthEnemyAction);
                    case ChargeEnemyActionDatas.ID: return typeof(Templar.Unit.Enemy.Actions.ChargeEnemyAction);
                    case ChaseEnemyActionDatas.ID: return typeof(Templar.Unit.Enemy.Actions.ChaseEnemyAction);
                    case FleeEnemyActionDatas.ID: return typeof(Templar.Unit.Enemy.Actions.FleeEnemyAction);

                    default:
                        CProLogger.LogError(this, $"Unhandled EnemyActionDatas Id {element.Value}.");
                        return null;
                }
            }

            base.Deserialize(container);

            XElement lastActionsCheckElement = container as XElement;

            XAttribute actionsCountAttribute = lastActionsCheckElement.Attribute("ActionsCount");
            UnityEngine.Assertions.Assert.IsFalse(actionsCountAttribute.IsNullOrEmpty(), "LastActionsCheck element needs a ActionsCount attribute.");
            ActionsCount = actionsCountAttribute.ValueToInt();

            foreach (XElement excludeElement in lastActionsCheckElement.Elements("Exclude"))
            {
                System.Type actionType = ParseActionType(excludeElement);
                if (!Exclude.Contains(actionType))
                    Exclude.Add(actionType);   
            }

            foreach (XElement includeElement in lastActionsCheckElement.Elements("Include"))
            {
                System.Type actionType = ParseActionType(includeElement);
                if (!Include.Contains(actionType))
                    Include.Add(actionType);
            }
        }
    }
}