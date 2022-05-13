namespace Templar.Manager
{
    using System.Collections.Generic;
    using System.Xml.Linq;
    using RSLib.Extensions;

    public partial class DialoguesStructuresManager : RSLib.Framework.ConsoleProSingleton<DialoguesStructuresManager>
    {
        private class DialoguesStructureData
        {
            public DialoguesStructureData()
            {
                DialoguesDone = new List<string>();
                SoldItems = new Dictionary<string, int>();
            }
            
            public DialoguesStructureData(List<string> dialoguesDone, Dictionary<string, int> soldItems)
            {
                DialoguesDone = dialoguesDone;
                SoldItems = soldItems;
            }
            
            public List<string> DialoguesDone { get; }
            public System.Collections.Generic.Dictionary<string, int> SoldItems { get; }
        }
        
        private static Dictionary<string, DialoguesStructureData> s_dialoguesStructureDataBySpeaker = new Dictionary<string, DialoguesStructureData>();

        public static bool TryGetDialoguesDoneBySpeaker(string speakerId, out List<string> dialoguesDone)
        {
            if (s_dialoguesStructureDataBySpeaker.TryGetValue(speakerId, out DialoguesStructureData dialoguesStructureData))
            {
                dialoguesDone = dialoguesStructureData.DialoguesDone;
                return true;
            }

            dialoguesDone = null;
            return false;
        }
        
        public static bool TryGetSoldItemsBySpeaker(string speakerId, out Dictionary<string, int> soldItems)
        {
            if (s_dialoguesStructureDataBySpeaker.TryGetValue(speakerId, out DialoguesStructureData dialoguesStructureData))
            {
                soldItems = dialoguesStructureData.SoldItems;
                return true;
            }

            soldItems = null;
            return false;
        }

        public static void RegisterDialogueForSpeaker(string speakerId, string dialogueId)
        {
            if (!s_dialoguesStructureDataBySpeaker.ContainsKey(speakerId))
                s_dialoguesStructureDataBySpeaker.Add(speakerId, new DialoguesStructureData());
            
            s_dialoguesStructureDataBySpeaker[speakerId].DialoguesDone.Add(dialogueId);
        }

        public static void RegisterSoldItemForSpeaker(string speakerId, string itemId, int quantity)
        {
            if (!s_dialoguesStructureDataBySpeaker.ContainsKey(speakerId))
            {
                s_dialoguesStructureDataBySpeaker.Add(speakerId, new DialoguesStructureData());
                s_dialoguesStructureDataBySpeaker[speakerId].SoldItems.Add(itemId, quantity);
            }
            else
            {
                if (!s_dialoguesStructureDataBySpeaker[speakerId].SoldItems.ContainsKey(itemId))
                    s_dialoguesStructureDataBySpeaker[speakerId].SoldItems.Add(itemId, quantity);
                else
                    s_dialoguesStructureDataBySpeaker[speakerId].SoldItems[itemId] += quantity;
            }
        }

        public static void Clear()
        {
            s_dialoguesStructureDataBySpeaker.Clear();
        }
    }

    public partial class DialoguesStructuresManager : RSLib.Framework.ConsoleProSingleton<DialoguesStructuresManager>
    {
        public static void Load(XElement dialoguesStructuresElement)
        {
            Clear();

            foreach (XElement dialoguesDoneBySpeakerElement in dialoguesStructuresElement.Elements("DialoguesStructure"))
            {
                XAttribute speakerIdAttribute = dialoguesDoneBySpeakerElement.Attribute("SpeakerId");

                List<string> dialoguesDone = new List<string>();
                foreach (XElement dialogueIdElement in dialoguesDoneBySpeakerElement.Elements("DialogueDone"))
                    dialoguesDone.Add(dialogueIdElement.Value);

                Dictionary<string, int> soldItems = new Dictionary<string, int>();
                foreach (XElement soldItemElement in dialoguesDoneBySpeakerElement.Elements("SoldItem"))
                {
                    XAttribute itemIdAttribute = soldItemElement.Attribute("ItemId");
                    XAttribute quantityAttribute = soldItemElement.Attribute("Quantity");
                    int quantity = quantityAttribute?.ValueToInt() ?? 1;
                    soldItems.Add(itemIdAttribute.Value, quantity);
                }

                DialoguesStructureData dialoguesStructureData = new DialoguesStructureData(dialoguesDone, soldItems);
                s_dialoguesStructureDataBySpeaker.Add(speakerIdAttribute.Value, dialoguesStructureData);
            }
        }

        public static XElement Save()
        {
            XElement dialoguesStructuresElement = new XElement("DialoguesStructures");

            foreach (KeyValuePair<string, DialoguesStructureData> dialoguesStructureDataBySpeaker in s_dialoguesStructureDataBySpeaker)
            {
                XElement dialoguesStructureElement = new XElement("DialoguesStructure");
                dialoguesStructureElement.Add(new XAttribute("SpeakerId", dialoguesStructureDataBySpeaker.Key));

                for (int i = dialoguesStructureDataBySpeaker.Value.DialoguesDone.Count - 1; i >= 0; --i)
                    dialoguesStructureElement.Add(new XElement("DialogueDone", dialoguesStructureDataBySpeaker.Value.DialoguesDone[i]));

                foreach (KeyValuePair<string, int> soldItem in dialoguesStructureDataBySpeaker.Value.SoldItems)
                    dialoguesStructureElement.Add(new XElement("SoldItem", new XAttribute("ItemId", soldItem.Key), new XAttribute("Quantity", soldItem.Value)));
                
                dialoguesStructuresElement.Add(dialoguesStructureElement);
            }

            return dialoguesStructuresElement;
        }
    }
}