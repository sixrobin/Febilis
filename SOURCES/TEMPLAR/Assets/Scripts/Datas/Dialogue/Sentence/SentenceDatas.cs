namespace Templar.Datas.Dialogue
{
    using RSLib.Extensions;
    using System.Xml.Linq;

    public class SentenceDatas : Datas, IDialogueSequenceElementDatas
    {
        public SentenceDatas(XContainer container) : base(container)
        {
        }

        public string Id { get; private set; }

        public string SpeakerId { get; private set; }

        public string OverrideDisplayName { get; private set; }
        public string OverridePortraitId { get; private set; }
        public bool HideSpeakerName { get; private set; }

        public string RawValue { get; private set; }
        public string SentenceValue { get; private set; }

        public bool Skippable { get; private set; }

        public PortraitAnchor PortraitAnchor { get; private set; }

        public SentenceSequenceElementDatas[] SequenceElementsDatas { get; private set; }

        public override void Deserialize(XContainer container)
        {
            XElement sentenceElement = container as XElement;

            XAttribute idAttribute = sentenceElement.Attribute("Id");
            UnityEngine.Assertions.Assert.IsFalse(idAttribute.IsNullOrEmpty(), "Sentence Id attribute is null or empty.");
            Id = idAttribute.Value;

            Skippable = sentenceElement.Element("Unskippable") == null;

            XElement portraitAnchorElement = sentenceElement.Element("PortraitAnchor");
            PortraitAnchor = portraitAnchorElement?.ValueToEnum<PortraitAnchor>() ?? PortraitAnchor.TOP_LEFT;

            XElement speakerElement = sentenceElement.Element("Speaker");
            UnityEngine.Assertions.Assert.IsNotNull(speakerElement, "Sentence datas needs a Speaker element.");

            XAttribute speakerIdAttribute = speakerElement.Attribute("Id");
            UnityEngine.Assertions.Assert.IsNotNull(speakerIdAttribute, "Speaker element needs an Id attribute.");
            SpeakerId = speakerIdAttribute.Value;

            XElement overrideDisplayNameElement = speakerElement.Element("OverrideDisplayName");
            OverrideDisplayName = overrideDisplayNameElement?.Value ?? null;

            XElement overridePortraitIdElement = speakerElement.Element("OverridePortraitId");
            OverridePortraitId = overridePortraitIdElement?.Value ?? null;

            HideSpeakerName = speakerElement.Element("HideSpeakerName") != null;

            XElement valueElement = sentenceElement.Element("Value");
            UnityEngine.Assertions.Assert.IsFalse(valueElement.IsNullOrEmpty(), "Sentence Value element is null or empty.");
            RawValue = valueElement.Value;

            // Custom tags parsing.
            System.Collections.Generic.List<(SentenceSequenceElementDatas datas, int tagStart, int tagEnd)> customTagsElements
                = new System.Collections.Generic.List<(SentenceSequenceElementDatas datas, int tagStart, int tagEnd)>();

            System.Collections.Generic.List<int> openingBracketsIndexes = new System.Collections.Generic.List<int>();
            System.Collections.Generic.List<int> closingBracketsIndexes = new System.Collections.Generic.List<int>();

            for (int c = 0; c < RawValue.Length; ++c)
            {
                if (RawValue[c] == '<')
                    openingBracketsIndexes.Add(c);
                else if (RawValue[c] == '>')
                    closingBracketsIndexes.Add(c);
            }

            UnityEngine.Assertions.Assert.IsTrue(
                openingBracketsIndexes.Count == closingBracketsIndexes.Count,
                $"Sentence {Id} contains {openingBracketsIndexes.Count} opening brackets for {closingBracketsIndexes.Count} closing brackets.");

            for (int i = 0; i < openingBracketsIndexes.Count; ++i)
            {
                int closingBracketIndex = closingBracketsIndexes[i];
                string tag = RawValue.Substring(openingBracketsIndexes[i] + 1, closingBracketsIndexes[i] - openingBracketsIndexes[i] - 1);

                string[] tagArgs = tag.Split('=');
                UnityEngine.Assertions.Assert.IsTrue(tagArgs.Length == 2, $"Tag {tag} could not be split in exactly 2 arguments using = as separator in sentence {Id}.");

                string tagType = tagArgs[0];
                string tagValue = tagArgs[1];

                //UnityEngine.Debug.Log($"Full tag: {tag}");
                //UnityEngine.Debug.Log($"Tag type: {tagType}");
                //UnityEngine.Debug.Log($"Tag value: {tagValue}");

                // Custom tag creations.
                if (tagType == SentencePauseDatas.TAG_ID)
                    customTagsElements.Add((new SentencePauseDatas(this, tagValue, openingBracketsIndexes[i], closingBracketsIndexes[i]), openingBracketsIndexes[i], closingBracketsIndexes[i]));
            }

            // Sequence creation.
            if (customTagsElements.Count == 0)
            {
                // No custom elements, only a simple text.
                SequenceElementsDatas = new SentenceSequenceElementDatas[] { new SentenceTextDatas(this, RawValue) };
                SentenceValue = RawValue;
            }
            else
            {
                System.Collections.Generic.List<SentenceSequenceElementDatas> sequenceElementsDatasList = new System.Collections.Generic.List<SentenceSequenceElementDatas>
                {
                    new SentenceTextDatas(this, RawValue.Substring(0, openingBracketsIndexes[0]))
                };

                SentenceValue += RawValue.Substring(0, openingBracketsIndexes[0]);

                // Take each custom tag, add it to the sequence, and add the normal text that's following if it exists (= not another tag right after).
                for (int i = 0; i < customTagsElements.Count; ++i)
                {
                    sequenceElementsDatasList.Add(customTagsElements[i].datas);

                    int nextTextEnd = i == customTagsElements.Count - 1
                        ? RawValue.Length - customTagsElements[i].tagEnd - 1
                        : customTagsElements[i + 1].tagStart - customTagsElements[i].tagEnd - 1;

                    string nextText = RawValue.Substring(customTagsElements[i].tagEnd + 1, nextTextEnd);
                    if (!string.IsNullOrEmpty(nextText))
                    {
                        sequenceElementsDatasList.Add(new SentenceTextDatas(this, nextText));
                        SentenceValue += nextText;
                    }
                }

                SequenceElementsDatas = sequenceElementsDatasList.ToArray();
            }
        }
    }
}