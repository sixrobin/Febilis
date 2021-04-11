namespace Templar.Manager
{
    using System;
    using System.Linq;
    using UnityEngine;

    public class GameManager : RSLib.Framework.Singleton<GameManager>
    {
        [Tooltip("Checkpoint to use on game start. Can be null to use default behaviour.")]
        [SerializeField] private Interaction.CheckpointController _overrideCheckpoint = null;

        [SerializeField] private Unit.Player.PlayerController _playerCtrl = null;
        [SerializeField] private bool _fadeOnRespawn = false;

        private System.Collections.Generic.IEnumerable<ICheckpointListener> _checkpointListeners;

        public static Unit.Player.PlayerController PlayerCtrl => Instance._playerCtrl;
        public static Interaction.CheckpointController OverrideCheckpoint => Instance._overrideCheckpoint;

        public static void OnCheckpointInteracted(Interaction.CheckpointController checkpoint)
        {
            foreach (ICheckpointListener listener in Instance._checkpointListeners)
                listener.OnCheckpointInteracted(checkpoint);
        }

        private void CheckDuplicateCheckpointIds()
        {
            Interaction.CheckpointController[] checkpoints = FindObjectsOfType<Interaction.CheckpointController>();
            if (checkpoints.Length == 0)
                return;

            System.Collections.Generic.Dictionary<string, int> idCounters = new System.Collections.Generic.Dictionary<string, int>();

            for (int i = checkpoints.Length - 1; i >= 0; --i)
            {
                if (!idCounters.ContainsKey(checkpoints[i].Id))
                    idCounters.Add(checkpoints[i].Id, 0);

                idCounters[checkpoints[i].Id]++;
            }

            foreach (System.Collections.Generic.KeyValuePair<string, int> idCounter in idCounters)
                if (idCounter.Value > 1)
                    LogError($"There are {idCounter.Value} checkpoints with the same Id \"{idCounter.Key}\" in the scene.");
        }

        private void SpawnPlayer()
        {
            if (OverrideCheckpoint != null)
                LogWarning($"Spawning player to an overridden checkpoint {OverrideCheckpoint.transform.name}.");

            _playerCtrl.Init(OverrideCheckpoint ?? Interaction.CheckpointController.CurrCheckpoint);
        }

        private System.Collections.IEnumerator SpawnPlayerCoroutine()
        {
            if (_playerCtrl == null)
            {
                LogWarning("Reference to PlayerController is missing, trying to find it using FindObjectOfType.");
                _playerCtrl = FindObjectOfType<Unit.Player.PlayerController>();

                if (_playerCtrl == null)
                {
                    LogError("No PlayerController seems to be in the scene.");
                    yield break;
                }
            }

            if (_fadeOnRespawn && _playerCtrl.CameraCtrl.GrayscaleRamp.enabled)
            {
                RampFadeManager.SetRampOffset(_playerCtrl.CameraCtrl.GrayscaleRamp, -1f);
                // [TODO] Hide player HUD.
            }

            yield return new WaitForEndOfFrame(); // Wait for checkpoints initialization.

            SpawnPlayer();

            if (_fadeOnRespawn && _playerCtrl.CameraCtrl.GrayscaleRamp.enabled)
                RampFadeManager.Fade(_playerCtrl.CameraCtrl.GrayscaleRamp, "InBase", (0.1f, 0f), () => _playerCtrl.AllowInputs(true));
            else
                _playerCtrl.AllowInputs(true);
        }

        protected override void Awake()
        {
            base.Awake();

#if UNITY_EDITOR
            CheckDuplicateCheckpointIds();
#endif

            StartCoroutine(SpawnPlayerCoroutine());
        }

        private void Start()
        {
            KillTrigger.ResetSharedTriggers();
            _checkpointListeners = FindObjectsOfType<MonoBehaviour>().OfType<ICheckpointListener>();

            // [TMP]
            RSLib.SceneReloader.BeforeReload += SaveManager.Save;
        }

        private void Update()
        {
            // [TMP]
            if (Input.GetKeyDown(KeyCode.F2))
            {
                SaveManager.EraseSave();
                Interaction.CheckpointController.ForceRemoveCurrentCheckpoint();
            }
        }

        private void OnDestroy()
        {
            // [TMP]
            RSLib.SceneReloader.BeforeReload -= SaveManager.Save;
        }
    }
}