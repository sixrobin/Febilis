namespace Templar.Interaction.Dialogue.DialogueStructure
{
    using System.Linq;

    public class DialogueStructureController
    {
        private Datas.Dialogue.DialogueStructure.DialoguesStructureDatas _dialoguesStructureDatas;
        private string _speakerId;
        private System.Collections.Generic.Dictionary<string, IDialogueStructureConditionChecker[]> _dialogueStructureConditionsCheckers;

        private System.Collections.Generic.List<string> _doneDialogues = new System.Collections.Generic.List<string>();
        private System.Collections.Generic.Dictionary<string, int> _soldItems = new System.Collections.Generic.Dictionary<string, int>();

        public DialogueStructureController(string dialoguesStructureId, string speakerId) : this(Database.DialogueDatabase.DialoguesStructuresDatas[dialoguesStructureId], speakerId)
        {
        }

        public DialogueStructureController(Datas.Dialogue.DialogueStructure.DialoguesStructureDatas dialoguesStructureDatas, string speakerId)
        {
            _dialoguesStructureDatas = dialoguesStructureDatas;
            _speakerId = speakerId;
            CreateConditionsCheckers();
        }

        public void LoadDoneDialogues(System.Collections.Generic.IEnumerable<string> doneDialogues)
        {
            _doneDialogues = doneDialogues.ToList();
        }

        public void LoadSoldItems(System.Collections.Generic.Dictionary<string, int> soldItems)
        {
            _soldItems = new System.Collections.Generic.Dictionary<string, int>(soldItems);
        }

        public string GetNextDialogueId()
        {
            // TODO: Record in this controller the dialogues already played, so that they can be passed as a parameter then for the "DialogueNeverDone" condition.

            foreach (System.Collections.Generic.KeyValuePair<string, IDialogueStructureConditionChecker[]> dialogue in _dialogueStructureConditionsCheckers)
            {
                if (dialogue.Value == null)
                {
                    // Null condition checker array means we don't have any condition needed.
                    return MarkDialogueAsDone(dialogue.Key);
                }

                bool validDialogue = true;

                for (int i = dialogue.Value.Length - 1; i >= 0; --i)
                {
                    if (!dialogue.Value[i].Check(this))
                    {
                        validDialogue = false;
                        break;
                    }
                }

                if (validDialogue)
                    return MarkDialogueAsDone(dialogue.Key);
            }

            // Should never happen.
            CProLogger.LogError(this, $"No valid dialogue has been found in dialogues structure {_dialoguesStructureDatas.Id}!");
            return string.Empty;
        }

        public bool IsDialogueAlreadyDone(string id)
        {
            UnityEngine.Assertions.Assert.IsTrue(
                _dialoguesStructureDatas.Dialogues.Select(o => o.DialogueId).Any(o => o == id),
                $"Checking if dialogue {id} has been done in a structure controller but no dialogue has such an Id in the structure.");

            return _doneDialogues.Contains(id);
        }

        public int GetItemSoldQuantity(string itemId)
        {
            return _soldItems.TryGetValue(itemId, out int quantity) ? quantity : 0;
        }
        
        public void MarkItemAsSold(string id, int quantity)
        {
            if (!_soldItems.ContainsKey(id))
                _soldItems.Add(id, quantity);
            else
                _soldItems[id] += quantity;
            
            Manager.DialoguesStructuresManager.RegisterSoldItemForSpeaker(_speakerId, id, quantity);
        }
        
        private string MarkDialogueAsDone(string id)
        {
            if (_doneDialogues.Contains(id))
                return id;

            _doneDialogues.Add(id);
            Manager.DialoguesStructuresManager.RegisterDialogueForSpeaker(_speakerId, id);
            
            return id;
        }
        
        private void CreateConditionsCheckers()
        {
            _dialogueStructureConditionsCheckers = new System.Collections.Generic.Dictionary<string, IDialogueStructureConditionChecker[]>();

            foreach (Datas.Dialogue.DialogueStructure.DialoguesStructureDialogueDatas dialogueDatas in _dialoguesStructureDatas.Dialogues)
            {
                if (dialogueDatas.ConditionsDatas == null)
                {
                    // Null condition checker array means we don't have any condition needed, so this is valid.
                    _dialogueStructureConditionsCheckers.Add(dialogueDatas.DialogueId, null);
                    continue;
                }

                IDialogueStructureConditionChecker[] conditionChecker = new IDialogueStructureConditionChecker[dialogueDatas.ConditionsDatas.Length];
                for (int i = 0; i < dialogueDatas.ConditionsDatas.Length; ++i)
                {
                    if (dialogueDatas.ConditionsDatas[i] is Datas.Dialogue.DialogueStructure.DialogueNeverDoneDialogueConditionDatas dialogueNeverDoneConditionDatas)
                        conditionChecker[i] = new DialogueNeverDoneDialogueStructureConditionChecker(dialogueNeverDoneConditionDatas);
                    else if (dialogueDatas.ConditionsDatas[i] is Datas.Dialogue.DialogueStructure.ItemUnsoldDialogueConditionDatas itemUnsoldConditionDatas)
                        conditionChecker[i] = new ItemUnsoldDialogueStructureConditionChecker(itemUnsoldConditionDatas);
                    else if (dialogueDatas.ConditionsDatas[i] is Datas.Dialogue.DialogueStructure.PlayerHasItemDialogueConditionDatas playerHasItemConditionDatas)
                        conditionChecker[i] = new PlayerHasItemDialogueStructureConditionChecker(playerHasItemConditionDatas);
                    else if (dialogueDatas.ConditionsDatas[i] is Datas.Dialogue.DialogueStructure.PlayerDoesntHaveItemDialogueConditionDatas playerDoesntHaveItemConditionDatas)
                        conditionChecker[i] = new PlayerDoesntHaveItemDialogueStructureConditionChecker(playerDoesntHaveItemConditionDatas);
                    else if (dialogueDatas.ConditionsDatas[i] is Datas.Dialogue.DialogueStructure.BoardDiscoveredDialogueConditionDatas boardDiscoveredConditionDatas)
                        conditionChecker[i] = new BoardDiscoveredDialogueStructureConditionChecker(boardDiscoveredConditionDatas);
                    else if (dialogueDatas.ConditionsDatas[i] is Datas.Dialogue.DialogueStructure.ZoneDiscoveredDialogueConditionDatas zoneDiscoveredConditionDatas)
                        conditionChecker[i] = new ZoneDiscoveredDialogueStructureConditionChecker(zoneDiscoveredConditionDatas);
                    else
                        CProLogger.LogError(this, $"Unhandled DialogueStructureConditionChecker type {dialogueDatas.ConditionsDatas[i].GetType().Name} to create a condition checker.");
                }

                _dialogueStructureConditionsCheckers.Add(dialogueDatas.DialogueId, conditionChecker);
            }
        }
    }
}