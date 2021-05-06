namespace Templar.Datas.Dialogue
{
    using RSLib.Extensions;
    using System.Xml.Linq;

    public class SentenceDatas : Datas, IDialogueSequenceElementDatas
    {
        private const char OPENING_SPE_TAG_CHAR = '[';
        private const char CLOSING_SPE_TAG_CHAR = ']';
        private const char TAG_ASSIGNEMENT_CHAR = '=';

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

        public PortraitAnchor OverridePortraitAnchor { get; private set; }

        public SentenceSequenceElementDatas[] SequenceElementsDatas { get; private set; }

        public override void Deserialize(XContainer container)
        {
            XElement sentenceElement = container as XElement;

            XAttribute idAttribute = sentenceElement.Attribute("Id");
            UnityEngine.Assertions.Assert.IsFalse(idAttribute.IsNullOrEmpty(), "Sentence Id attribute is null or empty.");
            Id = idAttribute.Value;

            Skippable = sentenceElement.Element("Unskippable") == null;

            XElement overridePortraitAnchorElement = sentenceElement.Element("OverridePortraitAnchor");
            OverridePortraitAnchor = overridePortraitAnchorElement?.ValueToEnum<PortraitAnchor>() ?? PortraitAnchor.NONE;

            XElement speakerElement = sentenceElement.Element("Speaker");
            UnityEngine.Assertions.Assert.IsNotNull(speakerElement, $"Sentence datas {Id} needs a Speaker element.");

            XAttribute speakerIdAttribute = speakerElement.Attribute("Id");
            UnityEngine.Assertions.Assert.IsNotNull(speakerIdAttribute, $"Speaker element for sentence {Id} needs an Id attribute.");
            SpeakerId = speakerIdAttribute.Value;

            XElement overrideDisplayNameElement = speakerElement.Element("OverrideDisplayName");
            OverrideDisplayName = overrideDisplayNameElement?.Value ?? null;

            XElement overridePortraitIdElement = speakerElement.Element("OverridePortraitId");
            OverridePortraitId = overridePortraitIdElement?.Value ?? null;

            HideSpeakerName = speakerElement.Element("HideSpeakerName") != null;

            XElement valueElement = sentenceElement.Element("Value");
            UnityEngine.Assertions.Assert.IsFalse(valueElement.IsNullOrEmpty(), $"Sentence {Id} Value element is null or empty.");
            RawValue = valueElement.Value;

            // Custom tags parsing.
            System.Collections.Generic.List<(SentenceSequenceElementDatas datas, int tagStart, int tagEnd)> customTagsElements
                = new System.Collections.Generic.List<(SentenceSequenceElementDatas datas, int tagStart, int tagEnd)>();

            System.Collections.Generic.List<int> openingIndexes = RawValue.AllIndexesOf(OPENING_SPE_TAG_CHAR);
            System.Collections.Generic.List<int> closingIndexes = RawValue.AllIndexesOf(CLOSING_SPE_TAG_CHAR);

            UnityEngine.Assertions.Assert.IsTrue(
                openingIndexes.Count == closingIndexes.Count,
                $"Sentence {Id} contains {openingIndexes.Count} opening tag chars for {closingIndexes.Count} closing tag chars.");

            for (int i = 0; i < openingIndexes.Count; ++i)
            {
                string tag = RawValue.Substring(openingIndexes[i] + 1, closingIndexes[i] - openingIndexes[i] - 1);
                string[] tagArgs = tag.Split(TAG_ASSIGNEMENT_CHAR);
                UnityEngine.Assertions.Assert.IsTrue(tagArgs.Length == 2, $"Tag {tag} could not be split in exactly 2 arguments using {TAG_ASSIGNEMENT_CHAR} as separator in sentence {Id}.");

                string tagType = tagArgs[0];
                string tagValue = tagArgs[1];

                //UnityEngine.Debug.Log($"Full tag: {tag}");
                //UnityEngine.Debug.Log($"Tag type: {tagType}");
                //UnityEngine.Debug.Log($"Tag value: {tagValue}");

                // Custom tag creations.
                if (tagType == SentencePauseDatas.TAG_ID)
                    customTagsElements.Add((new SentencePauseDatas(this, tagValue, openingIndexes[i], closingIndexes[i]), openingIndexes[i], closingIndexes[i]));
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
                    new SentenceTextDatas(this, RawValue.Substring(0, openingIndexes[0]))
                };

                SentenceValue += RawValue.Substring(0, openingIndexes[0]);

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