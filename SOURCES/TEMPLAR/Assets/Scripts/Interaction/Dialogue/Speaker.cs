namespace Templar.Interaction.Dialogue
{
    using UnityEngine;

    public class Speaker : Interactable, INPCSpeaker
    {
        [SerializeField] private RSLib.Framework.OptionalString _speakerId = new RSLib.Framework.OptionalString(string.Empty, false);
        [SerializeField] private string _dialogueId = string.Empty;
        [SerializeField] private RSLib.Framework.OptionalTransform _playerDialoguePivot = new RSLib.Framework.OptionalTransform(null, false);
        [SerializeField] protected GameObject _highlight = null;
        [SerializeField] protected Collider2D _collider = null;

        public string SpeakerId => _speakerId.Enabled ? _speakerId.Value : string.Empty;

        public bool IsDialoguing { get; set; }

        public Transform PlayerDialoguePivot => _playerDialoguePivot.Enabled ? _playerDialoguePivot.Value : null;
        public Vector3 SpeakerPos => transform.position;
        
        void ISpeaker.OnSentenceStart()
        {
        }

        void ISpeaker.OnSentenceEnd()
        {
        }

        private void OnDialogueOver(UI.Dialogue.DialogueManager.DialogueOverEventArgs dialogueOverEventArgs)
        {
            IsDialoguing = false;

            // If no dialogue pivot has been set, the player will not unfocus the speaker since it will be on it.
            // We can then assume the highlight should stay visible.
            if (PlayerDialoguePivot != null)
                _highlight.SetActive(false);
        }

        public override void Focus()
        {
            base.Focus();
            _highlight.SetActive(true);
        }

        public override void Unfocus()
        {
            base.Unfocus();

            if (!IsDialoguing)
                _highlight.SetActive(false);
        }

        public override void Interact()
        {
            base.Interact();

            IsDialoguing = true;
            UI.Dialogue.DialogueManager.PlayDialogue(_dialogueId, this);
        }

        private void Start()
        {
            UI.Dialogue.DialogueManager.Instance.DialogueOver += OnDialogueOver;
        }

        private void OnDestroy()
        {
            if (UI.Dialogue.DialogueManager.Exists())
                UI.Dialogue.DialogueManager.Instance.DialogueOver -= OnDialogueOver;
        }
    }
}