﻿namespace Templar.Datas.Unit.Enemy
{
    using System.Xml.Linq;

    public abstract class EnemyConditionsCheckerDatas : Datas
    {
        public EnemyConditionsCheckerDatas(XContainer container) : base(container)
        {
        }

        public System.Collections.Generic.List<EnemyConditionDatas> Conditions { get; private set; }

        public bool HasConditions => Conditions != null;

        public override void Deserialize(XContainer container)
        {
            XElement conditionsElement = (container as XElement).Element("Conditions");

            if (conditionsElement == null)
                return;

            Conditions = new System.Collections.Generic.List<EnemyConditionDatas>();
            foreach (XElement conditionElement in conditionsElement.Elements())
            {
                switch (conditionElement.Name.LocalName)
                {
                    case FullHealthEnemyConditionDatas.ID:
                        Conditions.Add(new FullHealthEnemyConditionDatas(conditionElement));
                        break;

                    case HealthMaxEnemyConditionDatas.ID:
                        Conditions.Add(new HealthMaxEnemyConditionDatas(conditionElement));
                        break;

                    case HealthMinEnemyConditionDatas.ID:
                        Conditions.Add(new HealthMinEnemyConditionDatas(conditionElement));
                        break;

                    case HealthPercentageMaxEnemyConditionDatas.ID:
                        Conditions.Add(new HealthPercentageMaxEnemyConditionDatas(conditionElement));
                        break;

                    case HealthPercentageMinEnemyConditionDatas.ID:
                        Conditions.Add(new HealthPercentageMinEnemyConditionDatas(conditionElement));
                        break;

                    case LastActionsCheckConditionDatas.ID:
                        Conditions.Add(new LastActionsCheckConditionDatas(conditionElement));
                        break;

                    case MaxHeightOffsetConditionDatas.ID:
                        Conditions.Add(new MaxHeightOffsetConditionDatas(conditionElement));
                        break;

                    case PlayerAboveEnemyConditionDatas.ID:
                        Conditions.Add(new PlayerAboveEnemyConditionDatas(conditionElement));
                        break;

                    case PlayerAliveEnemyConditionDatas.ID:
                        Conditions.Add(new PlayerAliveEnemyConditionDatas(conditionElement));
                        break;

                    case PlayerDetectedEnemyConditionDatas.ID:
                        Conditions.Add(new PlayerDetectedEnemyConditionDatas(conditionElement));
                        break;

                    case PlayerInRangeEnemyConditionDatas.ID:
                        Conditions.Add(new PlayerInRangeEnemyConditionDatas(conditionElement));
                        break;

                    case RandomChanceConditionDatas.ID:
                        Conditions.Add(new RandomChanceConditionDatas(conditionElement));
                        break;

                    default:
                        Database.EnemyDatabase.Instance.LogError($"Unknown Enemy Condition Id {conditionElement.Name.LocalName}.");
                        break;
                }
            }
        }
    }
}