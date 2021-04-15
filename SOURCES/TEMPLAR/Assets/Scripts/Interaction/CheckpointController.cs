namespace Templar.Interaction
{
    using UnityEngine;

    public class CheckpointController : Interactable
    {
        [Header("REFS")]
        [SerializeField] private CheckpointView _checkpointView = null;
        [SerializeField] private GameObject _highlight = null;

        [Header("CHECKPOINT DATAS")]
        [SerializeField] private string _id = string.Empty;
        [SerializeField] private Vector2 _respawnOffset = Vector2.zero;

        [Header("DEBUG COLOR")]
        [SerializeField] private RSLib.DataColor _debugColor = null;

        private delegate void BeforeCheckpointChangeEventHandler(string currId, string nextId);
        private static BeforeCheckpointChangeEventHandler BeforeCheckpointChange;

        public static string CurrCheckpointId { get; private set; }
        public static CheckpointController CurrCheckpoint { get; private set; }

        public Vector3 RespawnPos => transform.position + (Vector3)_respawnOffset;

        public string Id => _id;

        // [TMP] We might want to keep this for some uses, but for now it's only for debug.
        public static void ForceRemoveCurrentCheckpoint()
        {
            CurrCheckpoint = null;
            CurrCheckpointId = null;
        }

        /// <summary>
        /// Used to set the current checkpoint Id from save file.
        /// Should only be called by a save manager of some sort.
        /// </summary>
        public static void LoadCurrentCheckpointId(string id)
        {
            // [TODO] Check if id is found in the scene to log a warning if not.
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
                CurrCheckpoint = this;
            }

            Manager.GameManager.PlayerCtrl.AllowInputs(false);
            _checkpointView.PlayInteractedAnimation(OnCheckpointViewEnabled);

            Manager.SaveManager.Save();
            Manager.GameManager.OnCheckpointInteracted(this);
        }

        private void OnBeforeCheckpointChange(string currId, string nextId)
        {
            UnityEngine.Assertions.Assert.IsFalse(currId == nextId,
                "Checkpoint change event has been called but current Id and next Id are the same.");

            // Turn off last checkpoint if it's in the scene.
            if (currId == Id)
                _checkpointView.PlayOffAnimation();
        }

        private void OnCheckpointViewEnabled()
        {
            // Logic that should happen on interaction, but delayed to fit the game view.

            Manager.GameManager.PlayerCtrl.HealthCtrl.HealFull();
            Manager.GameManager.PlayerCtrl.PlayerHealthCtrl.RestoreCells();

            Manager.GameManager.PlayerCtrl.AllowInputs(true);
        }

        private void Start()
        {
            BeforeCheckpointChange += OnBeforeCheckpointChange;

            if (CurrCheckpointId == Id)
            {
                CurrCheckpoint = this;
                _checkpointView.PlayOnAnimation();
            }
        }

        private void OnDestroy()
        {
            BeforeCheckpointChange -= OnBeforeCheckpointChange;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = _debugColor.Color;
            Gizmos.DrawWireSphere(RespawnPos, 0.2f);
        }
    }
}