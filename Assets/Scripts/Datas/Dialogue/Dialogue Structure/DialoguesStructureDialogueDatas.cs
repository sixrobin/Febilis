namespace Templar.Datas.Dialogue.DialogueStructure
{
    using RSLib.Extensions;
    using System.Linq;
    using System.Xml.Linq;

    public class DialoguesStructureDialogueDatas : Datas
    {
        private const string ELEMENT_NAME_DIALOGUE_NEVER_DONE = "DialogueNeverDone";
        private const string ELEMENT_NAME_ITEM_UNSOLD = "ItemUnsold";
        private const string ELEMENT_NAME_PLAYER_HAS_ITEM = "PlayerHasItem";
        private const string ELEMENT_NAME_PLAYER_DOESNT_HAVE_ITEM = "PlayerDoesntHaveItem";
        private const string ELEMENT_NAME_BOARD_DISCOVERED = "BoardDiscovered";
        private const string ELEMENT_NAME_ZONE_DISCOVERED = "ZoneDiscovered";

        public DialoguesStructureDialogueDatas(XContainer container) : base(container)
        {
        }

        public string DialogueId { get; private set; }

        public DialoguesStructureDialogueConditionDatas[] ConditionsDatas { get; private set; }

        public override void Deserialize(XContainer container)
        {
            XElement dialogueStructureElement = container as XElement;

            XAttribute idAttribute = dialogueStructureElement.Attribute("Id");
            UnityEngine.Assertions.Assert.IsFalse(idAttribute.IsNullOrEmpty(), "DialogueStructure Id attribute is null or empty.");
            DialogueId = idAttribute.Value;

            XElement conditionsElement = dialogueStructureElement.Element("Conditions");
            if (conditionsElement != null)
                DeserializeConditions(conditionsElement);
        }

        private void DeserializeConditions(XElement conditionsElement)
        {
            System.Collections.Generic.IEnumerable<XElement> conditionsElements = conditionsElement.Elements();
            ConditionsDatas = new DialoguesStructureDialogueConditionDatas[conditionsElements.Count()];

            int i = 0;
            foreach (XElement conditionElement in conditionsElements)
            {
                if (conditionElement.Name.LocalName == ELEMENT_NAME_DIALOGUE_NEVER_DONE)
                    ConditionsDatas[i] = new DialogueNeverDoneDialogueConditionDatas(conditionElement);
                else if (conditionElement.Name.LocalName == ELEMENT_NAME_ITEM_UNSOLD)
                    ConditionsDatas[i] = new ItemUnsoldDialogueConditionDatas(conditionElement);
                else if (conditionElement.Name.LocalName == ELEMENT_NAME_PLAYER_HAS_ITEM)
                    ConditionsDatas[i] = new PlayerHasItemDialogueConditionDatas(conditionElement);
                else if (conditionElement.Name.LocalName == ELEMENT_NAME_PLAYER_DOESNT_HAVE_ITEM)
                    ConditionsDatas[i] = new PlayerDoesntHaveItemDialogueConditionDatas(conditionElement);
                else if (conditionElement.Name.LocalName == ELEMENT_NAME_BOARD_DISCOVERED)
                    ConditionsDatas[i] = new BoardDiscoveredDialogueConditionDatas(conditionElement);
                else if (conditionElement.Name.LocalName == ELEMENT_NAME_ZONE_DISCOVERED)
                    ConditionsDatas[i] = new ZoneDiscoveredDialogueConditionDatas(conditionElement);
                else
                    CProLogger.LogError(this, $"Unhandled Contextual Condition name {conditionElement.Name.LocalName}.");

                i++;
            }
        }
    }
}