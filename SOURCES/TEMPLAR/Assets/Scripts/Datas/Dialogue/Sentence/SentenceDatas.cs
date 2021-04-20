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

        public string RawValue { get; private set; }

        public SentenceSequenceElementDatas[] SequenceElementsDatas { get; private set; }

        public override void Deserialize(XContainer container)
        {
            XElement sentenceElement = container as XElement;

            XAttribute idAttribute = sentenceElement.Attribute("Id");
            UnityEngine.Assertions.Assert.IsFalse(idAttribute.IsNullOrEmpty(), "Sentence Id attribute is null or empty.");
            Id = idAttribute.Value;

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

            UnityEngine.Assertions.Assert.IsTrue(
                openingBracketsIndexes.Count % 2 == 0,
                $"Sentence {Id} contains an odd count of brackets pairs ({openingBracketsIndexes.Count}).");

            for (int i = 0; i < openingBracketsIndexes.Count; i += 2)
            {
                string openingTag = RawValue.Substring(openingBracketsIndexes[i], closingBracketsIndexes[i] - openingBracketsIndexes[i] + 1);
                string closingTag = RawValue.Substring(openingBracketsIndexes[i + 1], closingBracketsIndexes[i + 1] - openingBracketsIndexes[i + 1]);
                string openingTagId = openingTag.Substring(1, openingTag.Length - 2).ToLower();
                string closingTagId = closingTag.Substring(2, closingTag.Length - 2).ToLower(); // Start at 2 because closing tag should have a '\' char.
                string tagValue = RawValue.Substring(closingBracketsIndexes[i] + 1, openingBracketsIndexes[i + 1] - closingBracketsIndexes[i] - 1);

                //UnityEngine.Debug.Log($"Opening tag Id: {openingTagId}");
                //UnityEngine.Debug.Log($"Closing tag Id: {closingTagId}");
                //UnityEngine.Debug.Log($"Tag value: {tagValue}");

                UnityEngine.Assertions.Assert.IsTrue(
                    openingTagId == closingTagId,
                    $"Opening tag {openingTagId} is different from closing tag {closingTagId} in sentence {Id}.");

                // Custom tag creations.
                if (openingTagId == SentencePauseDatas.TAG_ID)
                    customTagsElements.Add((new SentencePauseDatas(tagValue, openingBracketsIndexes[i], closingBracketsIndexes[i + 1]), openingBracketsIndexes[i], closingBracketsIndexes[i + 1]));
            }

            // Sequence creation.
            if (customTagsElements.Count == 0)
            {
                // No custom elements, only a simple text.
                SequenceElementsDatas = new SentenceSequenceElementDatas[] { new SentenceTextDatas(RawValue) };
            }
            else
            {
                System.Collections.Generic.List<SentenceSequenceElementDatas> sequenceElementsDatasList = new System.Collections.Generic.List<SentenceSequenceElementDatas>
                {
                    new SentenceTextDatas(RawValue.Substring(0, openingBracketsIndexes[0]))
                };

                // Take each custom tag, add it to the sequence, and add the normal text that's following if it exists (= not another tag right after).
                for (int i = 0; i < customTagsElements.Count; ++i)
                {
                    sequenceElementsDatasList.Add(customTagsElements[i].datas);

                    int nextTextEnd = i == customTagsElements.Count - 1
                        ? RawValue.Length - customTagsElements[i].tagEnd - 1
                        : customTagsElements[i + 1].tagStart - customTagsElements[i].tagEnd - 1;

                    string nextText = RawValue.Substring(customTagsElements[i].tagEnd + 1, nextTextEnd);
                    if (!string.IsNullOrEmpty(nextText))
                        sequenceElementsDatasList.Add(new SentenceTextDatas(nextText));
                }

                SequenceElementsDatas = sequenceElementsDatasList.ToArray();
            }
        }
    }
}