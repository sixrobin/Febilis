﻿namespace Templar.Datas
{
    using RSLib.Extensions;
    using static RSLib.Maths.Easing;
    using System.Xml.Linq;

    public class TriggerableDatas : Datas
    {
        public TriggerableDatas(XContainer container) : base(container)
        {
        }

        public string Id { get; private set; }

        public Physics.Triggerables.TriggerableSourceType ValidSources { get; private set; } = Physics.Triggerables.TriggerableSourceType.NONE;
        public int MaxTriggersCount { get; private set; }

        public LootDatas LootDatas { get; private set; }
        public ShakeTraumaDatas TraumaDatas { get; protected set; }
        public System.Collections.Generic.List<string> ToSpawnFromPool { get; private set; }
        public string AnimatorTrigger { get; private set; }

        public override void Deserialize(XContainer container)
        {
            XElement triggerableElement = container as XElement;

            XAttribute idAttribute = triggerableElement.Attribute("Id");
            UnityEngine.Assertions.Assert.IsFalse(idAttribute.IsNullOrEmpty(), "Triggerable Id attribute is null or empty.");
            Id = idAttribute.Value;

            XElement validSourcesElement = triggerableElement.Element("ValidSources");
            if (validSourcesElement == null)
                ValidSources = Physics.Triggerables.TriggerableSourceType.PHYSICS;
            else
                foreach (XElement validSourceElement in validSourcesElement.Elements("SourceType"))
                    ValidSources |= validSourceElement.ValueToEnum<Physics.Triggerables.TriggerableSourceType>();

            XElement maxTriggersCount = triggerableElement.Element("MaxTriggersCount");
            MaxTriggersCount = maxTriggersCount?.ValueToInt() ?? -1;

            ToSpawnFromPool = new System.Collections.Generic.List<string>();

            XElement onTriggerElement = triggerableElement.Element("OnTrigger");
            if (onTriggerElement != null)
            {
                XElement lootElement = onTriggerElement.Element("Loot");
                if (lootElement != null)
                    LootDatas = new LootDatas(lootElement);

                XElement traumaElement = onTriggerElement.Element("Trauma");
                if (traumaElement != null)
                    TraumaDatas = new ShakeTraumaDatas(traumaElement);

                XElement animatorTriggerElement = onTriggerElement.Element("AnimatorTrigger");
                if (animatorTriggerElement != null)
                    AnimatorTrigger = animatorTriggerElement.Value;

                foreach (XElement spawnFromPoolElement in onTriggerElement.Elements("SpawnFromPool"))
                    ToSpawnFromPool.Add(spawnFromPoolElement.Value);
            }
        }

        public bool IsSourceValid(Physics.Triggerables.TriggerableSourceType source)
        {
            return (ValidSources & source) != 0;
        }
    }
}