namespace Templar.Interaction.Dialogue.DialogueStructure
{
    using System.Linq;

    public class DialogueStructureController
    {
        private Datas.Dialogue.DialogueStructure.DialoguesStructureDatas _dialoguesStructureDatas;
        private System.Collections.Generic.Dictionary<string, IDialogueStructureConditionChecker[]> _dialogueStructureConditionsCheckers;

        private System.Collections.Generic.List<string> _doneDialogues = new System.Collections.Generic.List<string>();

        public DialogueStructureController(string dialoguesStructureId)
            : this(Database.DialogueDatabase.DialoguesStructuresDatas[dialoguesStructureId]) { }

        public DialogueStructureController(Datas.Dialogue.DialogueStructure.DialoguesStructureDatas dialoguesStructureDatas)
        {
            _dialoguesStructureDatas = dialoguesStructureDatas;

            CreateConditionsCheckers();
        }

        public System.Collections.Generic.List<string> GetDoneDialoguesCopy()
        {
            return new System.Collections.Generic.List<string>(_doneDialogues);
        }

        public void LoadDoneDialogues(System.Collections.Generic.IEnumerable<string> doneDialogues)
        {
            _doneDialogues = doneDialogues.ToList();
        }

        public string GetNextDialogueId()
        {
            // [TODO] Record in this controller the dialogues already played, so that they can be passed as a parameter
            // then for the "DialogueNeverDone" condition.

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
                _dialoguesStructureDatas.Dialogues.Select(o => o.DialogueId).Where(o => o == id).Count() > 0,
                $"Checking if dialogue {id} has been done in a structure controller but no dialogue has such an Id in the structure.");

            return _doneDialogues.Contains(id);
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
                    {
                        conditionChecker[i] = new DialogueNeverDoneDialogueStructureConditionChecker(dialogueNeverDoneConditionDatas);
                    }
                    else if (dialogueDatas.ConditionsDatas[i] is Datas.Dialogue.DialogueStructure.PlayerHasItemDialogueConditionDatas playerHasItemConditionDatas)
                    {
                        conditionChecker[i] = new PlayerHasItemDialogueStructureConditionChecker(playerHasItemConditionDatas);
                    }
                    else if (dialogueDatas.ConditionsDatas[i] is Datas.Dialogue.DialogueStructure.PlayerDoesntHaveItemDialogueConditionDatas playerDoesntHaveItemConditionDatas)
                    {
                        conditionChecker[i] = new PlayerDoesntHaveItemDialogueStructureConditionChecker(playerDoesntHaveItemConditionDatas);
                    }
                    else
                    {
                        CProLogger.LogError(this, $"Unhandled DialogueStructureConditionChecker type {dialogueDatas.ConditionsDatas[i].GetType().Name} to create a condition checker.");
                        continue;
                    }
                }

                _dialogueStructureConditionsCheckers.Add(dialogueDatas.DialogueId, conditionChecker);
            }
        }
    
        private string MarkDialogueAsDone(string id)
        {
            if (_doneDialogues.Contains(id))
                return id;

            _doneDialogues.Add(id);
            return id;
        }
    }
}