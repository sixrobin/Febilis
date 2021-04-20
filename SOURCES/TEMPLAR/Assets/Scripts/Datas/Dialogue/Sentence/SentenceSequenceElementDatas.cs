namespace Templar.Datas.Dialogue
{
    public abstract class SentenceSequenceElementDatas
    {
        public SentenceSequenceElementDatas(string value)
        {
            RawValue = value;
        }

        public string RawValue { get; private set; }
    }
}