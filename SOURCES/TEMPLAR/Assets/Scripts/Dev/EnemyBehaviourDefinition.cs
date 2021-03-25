namespace Templar.Dev
{
    using System.Linq;
    using System.Xml.Linq;

    public class EnemyBehaviourDefinition : EnemyConditionsDependentDefinition
    {
        public EnemyBehaviourDefinition(XContainer container) : base(container)
        {
        }

        public string Name { get; private set; }

        public System.Collections.Generic.List<EnemyActionDefinition> Actions { get; private set; }

        protected override void Deserialize(XContainer container)
        {
            XElement behaviourElement = container as XElement;

            XAttribute nameAttribute = behaviourElement.Attribute("Name");
            UnityEngine.Assertions.Assert.IsNotNull(nameAttribute, "A name attribute is missing for an EnemyBehaviourDefinition.");
            Name = nameAttribute.Value;

            XElement actionsElement = behaviourElement.Element("Actions");
            UnityEngine.Assertions.Assert.IsNotNull(actionsElement, $"Behaviour {Name} has no action.");
            Actions = new System.Collections.Generic.List<EnemyActionDefinition>();

            System.Collections.Generic.IEnumerable<XElement> actionsElements = actionsElement.Elements();
            string behaviourGenLog = $"Generated behaviour <b>{Name}</b> with {actionsElements.Count()} action(s):";

            foreach (XElement actionElement in actionsElements)
            {
                switch (actionElement.Name.LocalName)
                {
                    case AttackActionDefinition.ID:
                        Actions.Add(new AttackActionDefinition(actionElement));
                        break;

                    case BackAndForthActionDefinition.ID:
                        Actions.Add(new BackAndForthActionDefinition(actionElement));
                        break;

                    default:
                        UnityEngine.Debug.LogError($"Unknown action Id {actionElement.Name.LocalName}.");
                        continue;
                }

                behaviourGenLog += $"\n- {actionElement.Name.LocalName}";
            }

            EnemyDatabase.Instance.Log(behaviourGenLog);
        }
    }
}