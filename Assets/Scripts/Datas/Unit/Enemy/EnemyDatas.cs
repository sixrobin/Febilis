﻿namespace Templar.Datas.Unit.Enemy
{
    using RSLib.Extensions;
    using System.Xml.Linq;

    public class EnemyDatas : Datas
    {
        public EnemyDatas(XContainer container) : base(container)
        {
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

        public bool HurtCheckEdge { get; private set; }
        public float JumpHeight { get; private set; }
        public float JumpApexDur { get; private set; }
        public float JumpApexDurSqr => JumpApexDur * JumpApexDur;

        public LootDatas OnKilledLoot { get; private set; }
        public float OnKilledTrauma { get; private set; }

        public System.Collections.Generic.List<EnemyBehaviourDatas> Behaviours { get; private set; }

        public override void Deserialize(XContainer container)
        {
            XElement enemyElement = container as XElement;

            XAttribute idAttribute = enemyElement.Attribute("Id");
            UnityEngine.Assertions.Assert.IsFalse(idAttribute.IsNullOrEmpty(), "Enemy Id attribute is null or empty.");
            Id = idAttribute.Value;

            Database.EnemyDatabase.Instance.Log($"Generating definition for enemy <b>{Id}</b>...");

            XElement healthElement = enemyElement.Element("Health");
            UnityEngine.Assertions.Assert.IsNotNull(healthElement, $"EnemyDatas {Id} must have a Health element.");
            Health = healthElement.ValueToInt();

            XElement walkSpeedElement = enemyElement.Element("WalkSpeed");
            UnityEngine.Assertions.Assert.IsNotNull(walkSpeedElement, $"EnemyDatas {Id} must have a WalkSpeed element.");
            WalkSpeed = walkSpeedElement.ValueToFloat();

            XElement runSpeedElement = enemyElement.Element("RunSpeed");
            UnityEngine.Assertions.Assert.IsNotNull(runSpeedElement, $"EnemyDatas {Id} must have a RunSpeed element.");
            RunSpeed = runSpeedElement.ValueToFloat();

            XElement hurtDurElement = enemyElement.Element("HurtDur");
            UnityEngine.Assertions.Assert.IsNotNull(hurtDurElement, $"EnemyDatas {Id} must have a HurtDur element.");
            HurtDur = hurtDurElement.ValueToFloat();

            XElement playerDetectionDistElement = enemyElement.Element("PlayerDetectionDist");
            UnityEngine.Assertions.Assert.IsNotNull(playerDetectionDistElement, $"EnemyDatas {Id} must have a PlayerDetectionDist element.");
            PlayerDetectionDist = playerDetectionDistElement.ValueToFloat();

            XElement playerLoseDistElement = enemyElement.Element("PlayerLoseDist");
            UnityEngine.Assertions.Assert.IsNotNull(playerLoseDistElement, $"EnemyDatas {Id} must have a PlayerLoseDist element.");
            PlayerLoseDist = playerLoseDistElement.ValueToFloat();

            XElement physicsElement = enemyElement.Element("Physics");
            UnityEngine.Assertions.Assert.IsNotNull(physicsElement, $"EnemyDatas {Id} must have a Physics element.");
            DeserializePhysicsDatas(physicsElement);

            XElement behavioursElement = enemyElement.Element("Behaviours");
            UnityEngine.Assertions.Assert.IsNotNull(behavioursElement, $"EnemyDatas {Id} must have a Behaviours element.");

            Behaviours = new System.Collections.Generic.List<EnemyBehaviourDatas>();
            foreach (XElement behaviourElement in behavioursElement.Elements("Behaviour"))
                Behaviours.Add(new EnemyBehaviourDatas(behaviourElement));

            XElement onKilledTraumaElement = enemyElement.Element("OnKilledTrauma");
            if (onKilledTraumaElement != null)
                OnKilledTrauma = onKilledTraumaElement.ValueToFloat();

            XElement onKilledLootElement = enemyElement.Element("OnKilledLoot");
            if (onKilledLootElement != null)
                OnKilledLoot = new LootDatas(onKilledLootElement);
        }

        private void DeserializePhysicsDatas(XElement physicsElement)
        {
            XElement jumpElement = physicsElement.Element("Jump");
            UnityEngine.Assertions.Assert.IsNotNull(jumpElement, $"EnemyDatas {Id} Physics datas must have a Jump element.");

            XElement jumpHeightElement = jumpElement.Element("Height");
            UnityEngine.Assertions.Assert.IsNotNull(jumpHeightElement, $"EnemyDatas {Id} Jump datas must have a Height element.");
            JumpHeight = jumpHeightElement.ValueToFloat();

            XElement jumpApexDurElement = jumpElement.Element("ApexDur");
            UnityEngine.Assertions.Assert.IsNotNull(jumpApexDurElement, $"EnemyDatas {Id} Jump datas must have an ApexDur element.");
            JumpApexDur = jumpApexDurElement.ValueToFloat();

            XElement hurtCheckEdgeElement = physicsElement.Element("HurtCheckEdge");
            UnityEngine.Assertions.Assert.IsNotNull(hurtCheckEdgeElement, $"EnemyDatas {Id} Physics datas must have a HurtCheckEdge element.");
            HurtCheckEdge = hurtCheckEdgeElement.ValueToBool();
        }
    }
}