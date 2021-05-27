namespace Templar.Datas
{
    using RSLib.Extensions;
    using System.Xml.Linq;

    public class DestroyableDatas : Datas
    {
        public DestroyableDatas(XContainer container) : base(container)
        {
        }

        public string Id { get; private set; }

        public Physics.Destroyables.DestroyableSourceType ValidSources { get; private set; } = Physics.Destroyables.DestroyableSourceType.NONE;

        public LootDatas LootDatas { get; private set; }

        public ShakeTraumaDatas TraumaDatas { get; protected set; }
        public System.Collections.Generic.List<string> ToSpawnFromPool { get; private set; }

        public override void Deserialize(XContainer container)
        {
            XElement destroyableElement = container as XElement;

            XAttribute idAttribute = destroyableElement.Attribute("Id");
            UnityEngine.Assertions.Assert.IsFalse(idAttribute.IsNullOrEmpty(), "Destroyable Id attribute is null or empty.");
            Id = idAttribute.Value;

            XElement validSourcesElement = destroyableElement.Element("ValidSources");
            if (validSourcesElement == null)
                ValidSources = Physics.Destroyables.DestroyableSourceType.ALL;
            else
                foreach (XElement validSourceElement in validSourcesElement.Elements("SourceType"))
                    ValidSources |= validSourceElement.ValueToEnum<Physics.Destroyables.DestroyableSourceType>();

            ToSpawnFromPool = new System.Collections.Generic.List<string>();

            XElement onDestroyElement = destroyableElement.Element("OnDestroy");
            if (onDestroyElement != null)
            {

                XElement lootElement = onDestroyElement.Element("Loot");
                if (lootElement != null)
                    LootDatas = new LootDatas(lootElement);

                XElement traumaElement = onDestroyElement.Element("Trauma");
                if (traumaElement != null)
                    TraumaDatas = new ShakeTraumaDatas(traumaElement);

                foreach (XElement spawnFromPoolElement in onDestroyElement.Elements("SpawnFromPool"))
                    ToSpawnFromPool.Add(spawnFromPoolElement.Value);
            }
        }

        public bool IsSourceValid(Physics.Destroyables.DestroyableSourceType source)
        {
            return (ValidSources & source) != 0;
        }
    }
}