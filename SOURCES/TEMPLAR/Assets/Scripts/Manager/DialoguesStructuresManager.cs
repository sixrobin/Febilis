namespace Templar.Manager
{
    using System.Collections.Generic;
    using System.Xml.Linq;

    public partial class DialoguesStructuresManager : RSLib.Framework.ConsoleProSingleton<DialoguesStructuresManager>
    {
        private static Dictionary<string, List<string>> s_dialoguesDoneBySpeaker = new Dictionary<string, List<string>>();

        public static bool TryGetDialoguesDoneBySpeaker(string speakerId, out List<string> dialoguesDone)
        {
            return s_dialoguesDoneBySpeaker.TryGetValue(speakerId, out dialoguesDone);
        }

        public static void RegisterDialogueForSpeaker(string speakerId, string dialogueId)
        {
            if (!s_dialoguesDoneBySpeaker.ContainsKey(speakerId))
                s_dialoguesDoneBySpeaker.Add(speakerId, new List<string>() { dialogueId });
            else if (!s_dialoguesDoneBySpeaker[speakerId].Contains(dialogueId))
                s_dialoguesDoneBySpeaker[speakerId].Add(dialogueId);
        }
    }

    public partial class DialoguesStructuresManager : RSLib.Framework.ConsoleProSingleton<DialoguesStructuresManager>
    {
        public static void Load(XElement dialoguesStructuresElement)
        {
            foreach (XElement dialoguesDoneBySpeakerElement in dialoguesStructuresElement.Elements("DialoguesDone"))
            {
                XAttribute speakerIdAttribute = dialoguesDoneBySpeakerElement.Attribute("SpeakerId");

                List<string> dialoguesDone = new List<string>();
                foreach (XElement dialogueIdElement in dialoguesDoneBySpeakerElement.Elements("DialogueId"))
                    dialoguesDone.Add(dialogueIdElement.Value);

                s_dialoguesDoneBySpeaker.Add(speakerIdAttribute.Value, dialoguesDone);
            }
        }

        public static XElement Save()
        {
            XElement dialoguesStructuresElement = new XElement("DialoguesStructures");

            foreach (KeyValuePair<string, List<string>> dialoguesDoneBySpeaker in s_dialoguesDoneBySpeaker)
            {
                XElement dialoguesStructureElement = new XElement("DialoguesDone");
                dialoguesStructureElement.Add(new XAttribute("SpeakerId", dialoguesDoneBySpeaker.Key));

                for (int i = dialoguesDoneBySpeaker.Value.Count - 1; i >= 0; --i)
                    dialoguesStructureElement.Add(new XElement("DialogueId", dialoguesDoneBySpeaker.Value[i]));

                dialoguesStructuresElement.Add(dialoguesStructureElement);
            }

            return dialoguesStructuresElement;
        }
    }
}