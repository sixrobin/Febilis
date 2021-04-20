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
            XElement delayElement = container as XElement;
            Dur = delayElement.ValueToFloat();
        }
    }
}