﻿namespace Templar.Interaction.Dialogue
{
    using UnityEngine;

    public class Speaker : Interactable, INpcSpeaker
    {
        [SerializeField] private string _speakerId = string.Empty;
        [SerializeField] private string _dialogueId = string.Empty;
        [SerializeField] private Transform _playerDialoguePivot = null;
        [SerializeField] private GameObject _highlight = null;

        public string SpeakerId => _speakerId;

        public bool IsDialoguing { get; set; }

        public Transform PlayerDialoguePivot => _playerDialoguePivot;
        public Vector3 SpeakerPos => transform.position;
        
        private void OnDialogueOver(string dialogueId)
        {
            IsDialoguing = false;
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

        public void OnSentenceStartOrResume()
        {
        }

        public void OnSentenceStopOrPause()
        {
        }

        private void Start()
        {
            UI.Dialogue.DialogueManager.Instance.DialogueOver += OnDialogueOver;

            UnityEngine.Assertions.Assert.IsTrue(
                Datas.Dialogue.DialogueDatabase.SpeakersDisplayDatas.ContainsKey(SpeakerId),
                $"Speaker Id {SpeakerId} isn't known by {Datas.Dialogue.DialogueDatabase.Instance.GetType().Name}.");
        }

        private void OnDestroy()
        {
            if (UI.Dialogue.DialogueManager.Exists())
                UI.Dialogue.DialogueManager.Instance.DialogueOver -= OnDialogueOver;
        }
    }
}