namespace Templar.Interaction
{
    using UnityEngine;

    public class Speaker : Interactable
    {
        [SerializeField] private string _speakerId = string.Empty;
        [SerializeField] private string _dialogueId = string.Empty;

        public override void Interact()
        {
            base.Interact();

            UI.Dialogue.DialogueManager.PlayDialogue(_dialogueId);
        }

        private void Awake()
        {
            // [TODO] Check if speaker Id is known by DialogueDatabase. Else, throw error.   
        }
    }
}