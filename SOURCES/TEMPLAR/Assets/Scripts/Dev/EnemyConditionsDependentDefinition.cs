namespace Templar.Dev
{
    using System.Xml.Linq;

    public abstract class EnemyConditionsDependentDefinition
    {
        public EnemyConditionsDependentDefinition(XContainer container)
        {
            Deserialize(container);
        }

        public System.Collections.Generic.List<EnemyConditionDefinition> Conditions { get; private set; }

        public bool HasConditions => Conditions != null;

        protected virtual void Deserialize(XContainer container)
        {
            XElement conditionsElement = (container as XElement).Element("Conditions");

            if (conditionsElement == null)
                return;

            Conditions = new System.Collections.Generic.List<EnemyConditionDefinition>();
            foreach (XElement conditionElement in conditionsElement.Elements())
            {
                switch (conditionElement.Name.LocalName)
                {
                    case HealthMaxEnemyConditionDefinition.ID:
                        Conditions.Add(new HealthMaxEnemyConditionDefinition(conditionElement));
                        break;

                    case HealthMinEnemyConditionDefinition.ID:
                        Conditions.Add(new HealthMinEnemyConditionDefinition(conditionElement));
                        break;

                    case FullHealthEnemyConditionDefinition.ID:
                        Conditions.Add(new FullHealthEnemyConditionDefinition(conditionElement));
                        break;

                    case PlayerUndetectedEnemyConditionDefinition.ID:
                        Conditions.Add(new PlayerUndetectedEnemyConditionDefinition(conditionElement));
                        break;

                    case RandomChanceConditionDefinition.ID:
                        Conditions.Add(new RandomChanceConditionDefinition(conditionElement));
                        break;
                }
            }
        }
    }
}