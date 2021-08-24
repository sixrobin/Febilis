namespace Templar.Interaction.Dialogue.DialogueStructure
{
    public abstract class DialogueStructureConditionChecker<T> : IDialogueStructureConditionChecker
        where T : Datas.Dialogue.DialogueStructure.DialoguesStructureDialogueConditionDatas
    {
        protected T _conditionDatas;

        public DialogueStructureConditionChecker(Datas.Dialogue.DialogueStructure.DialoguesStructureDialogueConditionDatas conditionDatas)
        {
            _conditionDatas = (T)conditionDatas;
        }

        public abstract bool Check(DialogueStructureController dialogueStructureController);
    }


    public class DialogueNeverDoneDialogueStructureConditionChecker
        : DialogueStructureConditionChecker<Datas.Dialogue.DialogueStructure.DialogueNeverDoneDialogueConditionDatas>
    {
        public DialogueNeverDoneDialogueStructureConditionChecker(Datas.Dialogue.DialogueStructure.DialoguesStructureDialogueConditionDatas conditionDatas) : base(conditionDatas)
        {
        }

        public override bool Check(DialogueStructureController dialogueStructureController)
        {
            return !dialogueStructureController.IsDialogueAlreadyDone(_conditionDatas.DialogueId); 
        }
    }


    public class PlayerHasItemDialogueStructureConditionChecker
        : DialogueStructureConditionChecker<Datas.Dialogue.DialogueStructure.PlayerHasItemDialogueConditionDatas>
    {
        public PlayerHasItemDialogueStructureConditionChecker(Datas.Dialogue.DialogueStructure.DialoguesStructureDialogueConditionDatas conditionDatas) : base(conditionDatas)
        {
        }

        public override bool Check(DialogueStructureController dialogueStructureController)
        {
            return Manager.GameManager.InventoryCtrl.GetItemQuantity(_conditionDatas.ItemId) >= _conditionDatas.MinQuantity;
        }
    }


    public class PlayerDoesntHaveItemDialogueStructureConditionChecker
        : DialogueStructureConditionChecker<Datas.Dialogue.DialogueStructure.PlayerDoesntHaveItemDialogueConditionDatas>
    {
        public PlayerDoesntHaveItemDialogueStructureConditionChecker(Datas.Dialogue.DialogueStructure.DialoguesStructureDialogueConditionDatas conditionDatas) : base(conditionDatas)
        {
        }

        public override bool Check(DialogueStructureController dialogueStructureController)
        {
            return Manager.GameManager.InventoryCtrl.GetItemQuantity(_conditionDatas.ItemId) == 0;
        }
    }
}