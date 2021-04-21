namespace Templar.Datas.Dialogue
{
    public class SentenceTextDatas : SentenceSequenceElementDatas
    {
        public SentenceTextDatas(SentenceDatas container, string value) : base(container, value)
        {
            Value = value;
        }

        public string Value { get; private set; }
    }
}