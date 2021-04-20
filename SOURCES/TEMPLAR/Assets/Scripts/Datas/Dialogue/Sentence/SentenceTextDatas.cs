namespace Templar.Datas.Dialogue
{
    public class SentenceTextDatas : SentenceSequenceElementDatas
    {
        public SentenceTextDatas(string value) : base(value)
        {
            Value = value;
        }

        public string Value { get; private set; }
    }
}