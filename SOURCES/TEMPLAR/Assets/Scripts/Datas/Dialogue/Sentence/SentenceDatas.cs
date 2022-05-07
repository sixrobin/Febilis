namespace Templar.Datas.Dialogue
{
    using RSLib.Extensions;
    using System.Xml.Linq;

    public class SentenceDatas : Datas, IDialogueSequenceElementDatas
    {
        private const char OPENING_SPE_TAG_CHAR = '[';
        private const char CLOSING_SPE_TAG_CHAR = ']';
        private const char TAG_ASSIGNMENT_CHAR = '=';

        public SentenceDatas(XContainer container) : base(container)
        {
        }

        public string Id { get; private set; }

        public string SpeakerId { get; private set; }

        public string OverrideDisplayName { get; private set; }
        public string OverridePortraitId { get; private set; }
        public bool HidePortraitBox { get; private set; }
        public bool HideSpeakerName { get; private set; }

        public System.Collections.Generic.Dictionary<string, string> SentenceValueByLanguage { get; private set; }
        
        public bool Skippable { get; private set; }

        public PortraitAnchor OverridePortraitAnchor { get; private set; }

        public System.Collections.Generic.Dictionary<string, SentenceSequenceElementDatas[]> SequenceElementsDatasByLanguage { get; private set; }

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
            OverrideDisplayName = overrideDisplayNameElement?.Value;

            XElement overridePortraitIdElement = speakerElement.Element("OverridePortraitId");
            OverridePortraitId = overridePortraitIdElement?.Value;

            HideSpeakerName = speakerElement.Element("HideSpeakerName") != null;
            HidePortraitBox = speakerElement.Element("HidePortraitBox") != null;

            SequenceElementsDatasByLanguage = new System.Collections.Generic.Dictionary<string, SentenceSequenceElementDatas[]>();
            SentenceValueByLanguage = new System.Collections.Generic.Dictionary<string, string>();
            
            for (int i = Localizer.Instance.Languages.Length - 1; i >= 0; --i)
                SequenceElementsDatasByLanguage.Add(Localizer.Instance.Languages[i], ParseSentenceSequenceForLanguage(Localizer.Instance.Languages[i]));
        }

        private SentenceSequenceElementDatas[] ParseSentenceSequenceForLanguage(string languageName)
        {
            if (!Localizer.TryGet($"{Localization.Dialogue.SENTENCE_PREFIX}{Id}", languageName, out string rawValue))
            {
                CProLogger.LogWarning(this, $"Could not find raw value for sentence {Id}.");
                rawValue = Id;
            }

            SentenceValueByLanguage.Add(languageName, string.Empty);
            
            // Custom tags parsing.
            System.Collections.Generic.List<(SentenceSequenceElementDatas datas, int tagStart, int tagEnd)> customTagsElements = new System.Collections.Generic.List<(SentenceSequenceElementDatas datas, int tagStart, int tagEnd)>();
            System.Collections.Generic.List<int> openingIndexes = rawValue.AllIndexesOf(OPENING_SPE_TAG_CHAR);
            System.Collections.Generic.List<int> closingIndexes = rawValue.AllIndexesOf(CLOSING_SPE_TAG_CHAR);

            UnityEngine.Assertions.Assert.IsTrue(openingIndexes.Count == closingIndexes.Count, $"Sentence {Id} contains {openingIndexes.Count} opening tag chars for {closingIndexes.Count} closing tag chars.");

            for (int i = 0; i < openingIndexes.Count; ++i)
            {
                string tag = rawValue.Substring(openingIndexes[i] + 1, closingIndexes[i] - openingIndexes[i] - 1);
                string[] tagArgs = tag.Split(TAG_ASSIGNMENT_CHAR);
                UnityEngine.Assertions.Assert.IsTrue(tagArgs.Length == 2, $"Tag {tag} could not be split in exactly 2 arguments using {TAG_ASSIGNMENT_CHAR} as separator in sentence {Id}.");

                string tagType = tagArgs[0];
                string tagValue = tagArgs[1];

                //UnityEngine.Debug.Log($"Full tag: {tag}");
                //UnityEngine.Debug.Log($"Tag type: {tagType}");
                //UnityEngine.Debug.Log($"Tag value: {tagValue}");

                // Custom tag creations.
                if (tagType == SentencePauseDatas.TAG_ID)
                    customTagsElements.Add((new SentencePauseDatas(this, tagValue, openingIndexes[i], closingIndexes[i]), openingIndexes[i], closingIndexes[i]));
            }

            System.Collections.Generic.List<SentenceSequenceElementDatas> sequenceElementsDataList = new System.Collections.Generic.List<SentenceSequenceElementDatas>();
            
            // Sequence creation.
            if (customTagsElements.Count == 0)
            {
                // No custom elements, only a simple text.
                sequenceElementsDataList.Add(new SentenceTextDatas(this, rawValue));
                SentenceValueByLanguage[languageName] = rawValue;
            }
            else
            {
                sequenceElementsDataList.Add(new SentenceTextDatas(this, rawValue.Substring(0, openingIndexes[0])));
                SentenceValueByLanguage[languageName] += rawValue.Substring(0, openingIndexes[0]);


                // Take each custom tag, add it to the sequence, and add the normal text that's following if it exists (i.e. not another tag right after).
                for (int i = 0; i < customTagsElements.Count; ++i)
                {
                    sequenceElementsDataList.Add(customTagsElements[i].datas);

                    int nextTextEnd = i == customTagsElements.Count - 1
                        ? rawValue.Length - customTagsElements[i].tagEnd - 1
                        : customTagsElements[i + 1].tagStart - customTagsElements[i].tagEnd - 1;

                    string nextText = rawValue.Substring(customTagsElements[i].tagEnd + 1, nextTextEnd);
                    if (!string.IsNullOrEmpty(nextText))
                    {
                        sequenceElementsDataList.Add(new SentenceTextDatas(this, nextText));
                        SentenceValueByLanguage[languageName] += nextText;
                    }
                }
            }

            return sequenceElementsDataList.ToArray();
        }
    }
}