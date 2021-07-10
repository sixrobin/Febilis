namespace Templar.Manager
{
    using System.Linq;
    using UnityEngine;

    public class GameManager : RSLib.Framework.ConsoleProSingleton<GameManager>
    {
        [SerializeField] private Interaction.Checkpoint.OptionalCheckpointController _overrideCheckpoint = new Interaction.Checkpoint.OptionalCheckpointController();
        [SerializeField] private Unit.Player.PlayerController _playerCtrl = null;
        [SerializeField] private Templar.Camera.CameraController _cameraCtrl = null;
        [SerializeField] private Item.InventoryController _inventoryCtrl = null;
        [SerializeField] private UI.Inventory.InventoryView _inventoryView = null;
        [SerializeField] private bool _fadeOnRespawn = false;

        private System.Collections.Generic.IEnumerable<ICheckpointListener> _checkpointListeners;

        public static Unit.Player.PlayerController PlayerCtrl => Instance._playerCtrl;
        public static Templar.Camera.CameraController CameraCtrl => Instance._cameraCtrl;
        public static Item.InventoryController InventoryCtrl => Instance._inventoryCtrl;
        public static UI.Inventory.InventoryView InventoryView => Instance._inventoryView;
        public static Interaction.Checkpoint.OptionalCheckpointController OptionalCheckpoint => Instance._overrideCheckpoint;

        public static void OnCheckpointInteracted(Interaction.Checkpoint.CheckpointController checkpoint)
        {
            foreach (ICheckpointListener listener in Instance._checkpointListeners)
                listener.OnCheckpointInteracted(checkpoint);
        }

        private void CheckDuplicateCheckpointIds()
        {
            Interaction.Checkpoint.CheckpointController[] checkpoints = FindObjectsOfType<Interaction.Checkpoint.CheckpointController>();
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
            if (OptionalCheckpoint.Enabled)
                LogWarning($"Spawning player to an overridden checkpoint {OptionalCheckpoint.Value.transform.name}.");

            _playerCtrl.Init(OptionalCheckpoint.Enabled ? OptionalCheckpoint.Value : Interaction.Checkpoint.CheckpointController.CurrCheckpoint);
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

            if (_cameraCtrl == null)
            {
                LogWarning("Reference to CameraController is missing, trying to find it using FindObjectOfType.");
                _cameraCtrl = FindObjectOfType<Templar.Camera.CameraController>();

                if (_cameraCtrl == null)
                    LogError("No CameraController seems to be in the scene.");
            }

            if (_fadeOnRespawn && Manager.GameManager.CameraCtrl.GrayscaleRamp.enabled)
            {
                RampFadeManager.SetRampOffset(Manager.GameManager.CameraCtrl.GrayscaleRamp, -1f);
                // [TODO] Hide player HUD.
            }

            yield return new WaitForEndOfFrame(); // Wait for checkpoints initialization.

            SpawnPlayer();

            if (_fadeOnRespawn && Manager.GameManager.CameraCtrl.GrayscaleRamp.enabled)
                RampFadeManager.Fade(Manager.GameManager.CameraCtrl.GrayscaleRamp, "OutBase", (0.1f, 0f), (fadeIn) => _playerCtrl.AllowInputs(true));
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

            if (_inventoryCtrl == null)
            {
                LogWarning("Reference to InventoryController is missing, trying to find it using FindObjectOfType.");
                _inventoryCtrl = FindObjectOfType<Item.InventoryController>();
                if (_inventoryCtrl == null)
                    LogError("No InventoryController seems to be in the scene.");
            }

            if (_inventoryView == null)
            {
                LogWarning("Reference to InventoryView is missing, trying to find it using FindObjectOfType.");
                _inventoryView = FindObjectOfType<UI.Inventory.InventoryView>();
                if (_inventoryView == null)
                    LogError("No InventoryView seems to be in the scene.");
            }
        }

        private void Start()
        {
            KillTrigger.ResetSharedTriggers();
            _checkpointListeners = FindObjectsOfType<MonoBehaviour>().OfType<ICheckpointListener>();

            // [TMP]
            RSLib.SceneReloader.BeforeReload += SaveManager.Save;
        }

        private void OnDestroy()
        {
            // [TMP]
            RSLib.SceneReloader.BeforeReload -= SaveManager.Save;
        }
    }
}