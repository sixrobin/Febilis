﻿namespace Templar.Datas.Dialogue
{
    public class SentencePauseDatas : SentenceSequenceElementDatas
    {
        public const string TAG_ID = "pause";

        public SentencePauseDatas(SentenceDatas container, string value, int tagStart, int tagEnd) : base(container, value)
        {
            if (!float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float dur))
                Database.DialogueDatabase.Instance.LogError($"Could not parse {value} to a valid float value to create a sentence pause.");

            Dur = dur;

            TagStart = tagStart;
            TagEnd = tagEnd;
        }

        public float Dur { get; private set; }

        public int TagStart { get; private set; }
        public int TagEnd { get; private set; }
    }
}