namespace Templar.Datas.Unit.Enemy
{
    using System.Linq;
    using System.Xml.Linq;

    public class EnemyBehaviourDatas : EnemyConditionsCheckerDatas
    {
        public EnemyBehaviourDatas(XContainer container) : base(container)
        {
        }

        public string Name { get; private set; }

        public System.Collections.Generic.List<EnemyActionDatas> Actions { get; private set; }

        public override void Deserialize(XContainer container)
        {
            base.Deserialize(container);

            XElement behaviourElement = container as XElement;

            XAttribute nameAttribute = behaviourElement.Attribute("Name");
            UnityEngine.Assertions.Assert.IsNotNull(nameAttribute, "A name attribute is missing for an EnemyBehaviourDatas.");
            Name = nameAttribute.Value;

            XElement actionsElement = behaviourElement.Element("Actions");
            UnityEngine.Assertions.Assert.IsNotNull(actionsElement, $"Behaviour {Name} has no action.");
            Actions = new System.Collections.Generic.List<EnemyActionDatas>();

            foreach (XElement actionElement in actionsElement.Elements())
            {
                switch (actionElement.Name.LocalName)
                {
                    case AttackEnemyActionDatas.ID:
                        Actions.Add(new AttackEnemyActionDatas(actionElement));
                        break;
                    case BackAndForthEnemyActionDatas.ID:
                        Actions.Add(new BackAndForthEnemyActionDatas(actionElement));
                        break;
                    case ChargeEnemyActionDatas.ID:
                        Actions.Add(new ChargeEnemyActionDatas(actionElement));
                        break;
                    case ChaseEnemyActionDatas.ID:
                        Actions.Add(new ChaseEnemyActionDatas(actionElement));
                        break;
                    case FleeEnemyActionDatas.ID:
                        Actions.Add(new FleeEnemyActionDatas(actionElement));
                        break;
                    case WaitEnemyActionDatas.ID:
                        Actions.Add(new WaitEnemyActionDatas(actionElement));
                        break;
                    default:
                        UnityEngine.Debug.LogError($"Unknown action Id {actionElement.Name.LocalName}.");
                        continue;
                }
            }

            Database.EnemyDatabase.Instance.Log($"Generated behaviour \"<b>{Name}</b>\" datas ({Actions.Count} action(s), " +
                $"{(Conditions != null ? Conditions.Count.ToString() : "0")} condition(s))");
        }
    }
}