namespace Templar.Datas.Dialogue
{
    public abstract class SentenceSequenceElementDatas
    {
        public SentenceSequenceElementDatas(SentenceDatas container, string value)
        {
            Container = container;
            RawValue = value;
        }

        public SentenceDatas Container { get; private set; }

        public string RawValue { get; private set; }
    }
}