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

        public string DisplayName { get; private set; }
        public string PortraitId { get; private set; }

        public override void Deserialize(XContainer container)
        {
            XElement speakerDisplayElement = container as XElement;

            XAttribute idAttribute = speakerDisplayElement.Attribute("Id");
            UnityEngine.Assertions.Assert.IsFalse(idAttribute.IsNullOrEmpty(), "SpeakerDisplayDatas Id attribute is null or empty.");
            Id = idAttribute.Value;

            XElement displayNameElement = speakerDisplayElement.Element("DisplayName");
            UnityEngine.Assertions.Assert.IsFalse(displayNameElement.IsNullOrEmpty(), "SpeakerDisplayDatas needs a DisplayName element.");
            DisplayName = displayNameElement.Value;

            XElement portraitIdElement = speakerDisplayElement.Element("PortraitId");
            PortraitId = portraitIdElement?.Value ?? Id;
        }
    }
}