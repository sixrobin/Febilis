namespace Templar.Interaction.Dialogue
{
    using UnityEngine;

    public class Speaker : Interactable, ISpeaker
    {
        [SerializeField] private string _speakerId = string.Empty;
        [SerializeField] private string _dialogueId = string.Empty;
        [SerializeField] private Transform _playerDialoguePos = null;

        public string SpeakerId => _speakerId;

        public Vector3 PlayerDialoguePos => _playerDialoguePos.position;
        public Vector3 SpeakerPos => transform.position;

        public override void Interact()
        {
            base.Interact();
            UI.Dialogue.DialogueManager.PlayDialogue(_dialogueId, this);
        }

        public void OnSentenceStart()
        {
        }

        public void OnSentenceStop()
        {
        }

        private void Awake()
        {
            UnityEngine.Assertions.Assert.IsTrue(
                Datas.Dialogue.DialogueDatabase.SpeakersDisplayDatas.ContainsKey(SpeakerId),
                $"Speaker Id {SpeakerId} isn't known by {Datas.Dialogue.DialogueDatabase.Instance.GetType().Name}.");
        }
    }
}