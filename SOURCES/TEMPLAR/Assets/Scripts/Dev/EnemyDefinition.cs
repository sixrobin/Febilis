﻿namespace Templar.Dev
{
    using RSLib.Extensions;
    using System.Xml.Linq;
    using UnityEngine;

    public class EnemyDefinition
    {
        public EnemyDefinition(XContainer container)
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

        public System.Collections.Generic.List<EnemyBehaviourDefinition> Behaviours { get; private set; }

        private void Deserialize(XContainer container)
        {
            XElement enemyElement = container as XElement;

            XAttribute idAttribute = enemyElement.Attribute("Id");
            UnityEngine.Assertions.Assert.IsFalse(idAttribute.IsNullOrEmpty(), "Enemy Id attribute is null or empty.");
            Id = idAttribute.Value;

            EnemyDatabase.Instance.Log($"Generating definition for enemy <b>{Id}</b>...");

            XElement healthElement = enemyElement.Element("Health");
            UnityEngine.Assertions.Assert.IsNotNull(healthElement, "EnemyDefinition must have a Health element.");
            Health = healthElement.ValueToInt();

            XElement walkSpeedElement = enemyElement.Element("WalkSpeed");
            UnityEngine.Assertions.Assert.IsNotNull(walkSpeedElement, "EnemyDefinition must have a WalkSpeed element.");
            WalkSpeed = walkSpeedElement.ValueToFloat();

            XElement runSpeedElement = enemyElement.Element("RunSpeed");
            UnityEngine.Assertions.Assert.IsNotNull(runSpeedElement, "EnemyDefinition must have a RunSpeed element.");
            RunSpeed = runSpeedElement.ValueToFloat();

            XElement hurtDurElement = enemyElement.Element("HurtDur");
            UnityEngine.Assertions.Assert.IsNotNull(hurtDurElement, "EnemyDefinition must have a HurtDur element.");
            HurtDur = hurtDurElement.ValueToFloat();

            XElement playerDetectionDistElement = enemyElement.Element("PlayerDetectionDist");
            UnityEngine.Assertions.Assert.IsNotNull(playerDetectionDistElement, "EnemyDefinition must have a PlayerDetectionDist element.");
            PlayerDetectionDist = playerDetectionDistElement.ValueToFloat();

            XElement playerLoseDistElement = enemyElement.Element("PlayerLoseDist");
            UnityEngine.Assertions.Assert.IsNotNull(playerLoseDistElement, "EnemyDefinition must have a PlayerLoseDist element.");
            PlayerLoseDist = playerLoseDistElement.ValueToFloat();

            XElement behavioursElement = enemyElement.Element("Behaviours");
            UnityEngine.Assertions.Assert.IsNotNull(behavioursElement, "EnemyDefinition must have a Behaviours element.");

            Behaviours = new System.Collections.Generic.List<EnemyBehaviourDefinition>();
            foreach (XElement behaviourElement in behavioursElement.Elements("Behaviour"))
                Behaviours.Add(new EnemyBehaviourDefinition(behaviourElement));
        }
    }
}