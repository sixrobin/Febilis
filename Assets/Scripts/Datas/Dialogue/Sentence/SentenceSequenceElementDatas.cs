namespace Templar.Datas.Dialogue
{
    public abstract class SentenceSequenceElementDatas
    {
        public SentenceSequenceElementDatas(SentenceDatas container, string value)
        {
            Container = container;
            RawValue = value;
        }

        public SentenceDatas Container { get; }

        public string RawValue { get; }
    }
}