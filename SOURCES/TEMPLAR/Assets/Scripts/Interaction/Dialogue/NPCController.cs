namespace Templar.Interaction.Dialogue
{
    using UnityEngine;

    public class NPCController : Interactable, INPCSpeaker
    {
        private const string IDLE = "Idle";
        private const string DIALOGUE_IDLE = "DialogueIdle";
        private const string DIALOGUE_TALK = "DialogueTalk";

        [Header("REFS")]
        [SerializeField] private RSLib.SpriteRendererAnimatorPair[] _spriteAnimatorPairs = null;
        [SerializeField] private SpriteRenderer _highlight = null;

        [Header("DIALOGUE SETTINGS")]
        [SerializeField] private string _speakerId = string.Empty;
        [SerializeField] private string _initDialogueId = string.Empty; // [TODO] "DialogueBranchingDatas".
        [SerializeField] private RSLib.Framework.OptionalTransform _playerDialoguePivot = new RSLib.Framework.OptionalTransform(null, false);

        public string SpeakerId => _speakerId;

        public bool IsDialoguing { get; set; }

        public Transform PlayerDialoguePivot => _playerDialoguePivot.Enabled ? _playerDialoguePivot.Value : null;
        public Vector3 SpeakerPos => transform.position;

        void ISpeaker.OnSentenceStart()
        {
            SetTriggerOnAnimators(DIALOGUE_TALK);
        }

        void ISpeaker.OnSentenceEnd()
        {
            SetTriggerOnAnimators(DIALOGUE_IDLE);
        }

        private void OnDialogueOver(Datas.Dialogue.DialogueDatas dialogueDatas)
        {
            SetTriggerOnAnimators(IDLE);

            IsDialoguing = false;
            _highlight.enabled = false;
        }

        public override void Focus()
        {
            base.Focus();
            _highlight.enabled = true;
        }

        public override void Unfocus()
        {
            base.Unfocus();

            if (!IsDialoguing)
                _highlight.enabled = false;
        }

        public override void Interact()
        {
            base.Interact();

            IsDialoguing = true;
            UI.Dialogue.DialogueManager.PlayDialogue(_initDialogueId, this);
        }

        private void SetTriggerOnAnimators(string parameterId)
        {
            for (int i = _spriteAnimatorPairs.Length - 1; i >= 0; --i)
                _spriteAnimatorPairs[i].Animator.SetTrigger(parameterId);
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