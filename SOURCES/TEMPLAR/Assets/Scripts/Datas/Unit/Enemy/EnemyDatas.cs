namespace Templar.Datas.Unit.Enemy
{
    using RSLib.Extensions;
    using System.Xml.Linq;

    public class EnemyDatas
    {
        public EnemyDatas(XContainer container)
        {
            Deserialize(container);
        }

        public string Id { get; private set; }

        public int Health { get; private set; }
        public float WalkSpeed { get; private set; }
        public float RunSpeed { get; private set; }
        public float HurtDur { get; private set; }

        public float PlayerDetectionDist { get; private set; }
        public float PlayerLoseDist { get; private set; }
        public float PlayerDetectionDistSqr => PlayerDetectionDist * PlayerDetectionDist;
        public float PlayerLoseDistSqr => PlayerLoseDist * PlayerLoseDist;

        public float OnKilledTrauma { get; private set; }

        public System.Collections.Generic.List<EnemyBehaviourDatas> Behaviours { get; private set; }

        private void Deserialize(XContainer container)
        {
            XElement enemyElement = container as XElement;

            XAttribute idAttribute = enemyElement.Attribute("Id");
            UnityEngine.Assertions.Assert.IsFalse(idAttribute.IsNullOrEmpty(), "Enemy Id attribute is null or empty.");
            Id = idAttribute.Value;

            EnemyDatabase.Instance.Log($"Generating definition for enemy <b>{Id}</b>...");

            XElement healthElement = enemyElement.Element("Health");
            UnityEngine.Assertions.Assert.IsNotNull(healthElement, "EnemyDatas must have a Health element.");
            Health = healthElement.ValueToInt();

            XElement walkSpeedElement = enemyElement.Element("WalkSpeed");
            UnityEngine.Assertions.Assert.IsNotNull(walkSpeedElement, "EnemyDatas must have a WalkSpeed element.");
            WalkSpeed = walkSpeedElement.ValueToFloat();

            XElement runSpeedElement = enemyElement.Element("RunSpeed");
            UnityEngine.Assertions.Assert.IsNotNull(runSpeedElement, "EnemyDatas must have a RunSpeed element.");
            RunSpeed = runSpeedElement.ValueToFloat();

            XElement hurtDurElement = enemyElement.Element("HurtDur");
            UnityEngine.Assertions.Assert.IsNotNull(hurtDurElement, "EnemyDatas must have a HurtDur element.");
            HurtDur = hurtDurElement.ValueToFloat();

            XElement playerDetectionDistElement = enemyElement.Element("PlayerDetectionDist");
            UnityEngine.Assertions.Assert.IsNotNull(playerDetectionDistElement, "EnemyDatas must have a PlayerDetectionDist element.");
            PlayerDetectionDist = playerDetectionDistElement.ValueToFloat();

            XElement playerLoseDistElement = enemyElement.Element("PlayerLoseDist");
            UnityEngine.Assertions.Assert.IsNotNull(playerLoseDistElement, "EnemyDatas must have a PlayerLoseDist element.");
            PlayerLoseDist = playerLoseDistElement.ValueToFloat();

            XElement behavioursElement = enemyElement.Element("Behaviours");
            UnityEngine.Assertions.Assert.IsNotNull(behavioursElement, "EnemyDatas must have a Behaviours element.");

            Behaviours = new System.Collections.Generic.List<EnemyBehaviourDatas>();
            foreach (XElement behaviourElement in behavioursElement.Elements("Behaviour"))
                Behaviours.Add(new EnemyBehaviourDatas(behaviourElement));

            XElement onKilledTraumaElement = enemyElement.Element("OnKilledTrauma");
            if (onKilledTraumaElement != null)
                OnKilledTrauma = onKilledTraumaElement.ValueToFloat();
        }
    }
}