namespace Templar.Datas.Dialogue
{
    using RSLib.Extensions;
    using System.Xml.Linq;

    public class DialoguePauseDatas : Datas, IDialogueSequenceElementDatas
    {
        public DialoguePauseDatas(XContainer container) : base(container)
        {
        }

        public float Dur { get; private set; }

        public override void Deserialize(XContainer container)
        {
            XElement pauseElement = container as XElement;

            XAttribute durAttribute = pauseElement.Attribute("Dur");
            UnityEngine.Assertions.Assert.IsNotNull(durAttribute, "Dialogue Pause element needs a Dur attribute.");
            Dur = durAttribute.ValueToFloat();
        }
    }
}