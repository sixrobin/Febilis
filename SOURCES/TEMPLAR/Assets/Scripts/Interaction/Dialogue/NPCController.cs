namespace Templar.Interaction.Dialogue
{
    using Templar.SceneLoadingDatasStorage;
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
        [SerializeField] private string _dialogueStructureId = string.Empty;
        [SerializeField] private RSLib.Framework.OptionalTransform _playerDialoguePivot = new RSLib.Framework.OptionalTransform(null, false);

        [Header("GENERAL")]
        [SerializeField] private bool _lookAtPlayer = false;

        private DialogueStructure.DialogueStructureController _dialogueStructureController;
        private Transform _player;

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

        private void OnDialogueOver(UI.Dialogue.DialogueManager.DialogueOverEventArgs dialogueOverEventArgs)
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

            string dialogueToPlay = _dialogueStructureController.GetNextDialogueId();

            if (!string.IsNullOrEmpty(dialogueToPlay))
            {
                Manager.DialoguesStructuresManager.RegisterDialogueForSpeaker(SpeakerId, dialogueToPlay);
                UI.Dialogue.DialogueManager.PlayDialogue(dialogueToPlay, this);
            }
        }

        private void SetTriggerOnAnimators(string parameterId)
        {
            for (int i = _spriteAnimatorPairs.Length - 1; i >= 0; --i)
                _spriteAnimatorPairs[i].Animator.SetTrigger(parameterId);
        }

        private void Start()
        {
            UI.Dialogue.DialogueManager.Instance.DialogueOver += OnDialogueOver;

            _player = Manager.GameManager.PlayerCtrl.transform;
            _dialogueStructureController = new DialogueStructure.DialogueStructureController( _dialogueStructureId);

            if (Manager.DialoguesStructuresManager.TryGetDialoguesDoneBySpeaker(SpeakerId, out System.Collections.Generic.List<string> dialoguesDone))
                _dialogueStructureController.LoadDoneDialogues(dialoguesDone);
        }

        private void Update()
        {
            if (_lookAtPlayer)
                for (int i = _spriteAnimatorPairs.Length - 1; i >= 0; --i)
                    _spriteAnimatorPairs[i].SpriteRenderer.flipX = transform.position.x - _player.position.x > 0f;
        }

        private void OnDestroy()
        {
            if (UI.Dialogue.DialogueManager.Exists())
                UI.Dialogue.DialogueManager.Instance.DialogueOver -= OnDialogueOver;
        }
    }
}