namespace Templar.Datas.Dialogue
{
    using RSLib.Extensions;
    using System.Xml.Linq;

    public class SpeakerDisplayDatas : Datas
    {
        public SpeakerDisplayDatas(XContainer container) : base(container)
        {
        }

        public string Id { get; private set; }

        public string PortraitId { get; private set; }
        public PortraitAnchor PortraitAnchor { get; private set; }

        public override void Deserialize(XContainer container)
        {
            XElement speakerDisplayElement = container as XElement;

            XAttribute idAttribute = speakerDisplayElement.Attribute("Id");
            UnityEngine.Assertions.Assert.IsFalse(idAttribute.IsNullOrEmpty(), "SpeakerDisplayDatas Id attribute is null or empty.");
            Id = idAttribute.Value;

            XElement portraitIdElement = speakerDisplayElement.Element("PortraitId");
            PortraitId = portraitIdElement?.Value ?? Id;

            XElement portraitAnchorElement = speakerDisplayElement.Element("PortraitAnchor");
            UnityEngine.Assertions.Assert.IsFalse(portraitAnchorElement.IsNullOrEmpty(), "SpeakerDisplayDatas needs a PortraitAnchor element.");
            PortraitAnchor = portraitAnchorElement.ValueToEnum<PortraitAnchor>();
        }
    }
}