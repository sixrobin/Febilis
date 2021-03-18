﻿namespace Templar.Interaction
{
    using UnityEngine;

    public class CheckpointController : Interactable
    {
        [Header("REFS")]
        [SerializeField] private SpriteRenderer _checkpointView = null;
        [SerializeField] private Sprite _enabledSprite = null;
        [SerializeField] private GameObject _highlight = null;

        [Header("CHECKPOINT DATAS")]
        [SerializeField] private string _id = string.Empty;

        private Sprite _baseSprite;

        private delegate void BeforeCheckpointChangeEventHandler(string currId, string nextId);
        private static BeforeCheckpointChangeEventHandler BeforeCheckpointChange;

        public static string CurrCheckpointId { get; private set; }

        public string Id => _id;

        /// <summary>
        /// Used to set the current checkpoint Id from save file.
        /// Should only be called by a save manager of some sort.
        /// </summary>
        public static void LoadCurrentCheckpointId(string id)
        {
            CurrCheckpointId = id;
        }

        public override void Focus()
        {
            base.Focus();
            _highlight.SetActive(true);
        }

        public override void Unfocus()
        {
            base.Unfocus();
            _highlight.SetActive(false);
        }

        public override void Interact()
        {
            base.Interact();

            if (CurrCheckpointId != Id)
            {
                BeforeCheckpointChange(CurrCheckpointId, Id);
                CurrCheckpointId = Id;
            }

            Manager.SaveManager.Save();

            // [TODO] VFX to show something happened even if it is the current checkpoint.
            _checkpointView.sprite = _enabledSprite;
        }

        private void OnBeforeCheckpointChange(string currId, string nextId)
        {
            UnityEngine.Assertions.Assert.IsFalse(currId == nextId,
                "Checkpoint change event has been called but current Id and next Id are the same.");

            // Turn off last checkpoint if it's in the scene.
            if (currId == Id)
                _checkpointView.sprite = _baseSprite;
        }

        private void Start()
        {
            BeforeCheckpointChange += OnBeforeCheckpointChange;
            _baseSprite = _checkpointView.sprite;

            if (CurrCheckpointId == Id)
                _checkpointView.sprite = _enabledSprite;
        }

        private void OnDestroy()
        {
            BeforeCheckpointChange -= OnBeforeCheckpointChange;
        }
    }
}