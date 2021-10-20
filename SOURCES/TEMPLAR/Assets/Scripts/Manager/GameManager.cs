namespace Templar.Manager
{
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    public class GameManager : RSLib.Framework.ConsoleProSingleton<GameManager>
    {
        [Header("REFS")]
        [SerializeField] private Unit.Player.PlayerController _playerCtrl = null;
        [SerializeField] private Templar.Camera.CameraController _cameraCtrl = null;
        [SerializeField] private Item.InventoryController _inventoryCtrl = null;
        [SerializeField] private UI.Inventory.InventoryView _inventoryView = null;

        [Header("SPAWN DEBUG")]
        [SerializeField] private Interaction.Checkpoint.OptionalCheckpointController _overrideCheckpoint = new Interaction.Checkpoint.OptionalCheckpointController();
        [SerializeField] private bool _fadeOnRespawn = true;
        [SerializeField] private bool _stayAtSceneWindowPos = false;

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

            SaveManager.Save();
        }

        private void CheckDuplicateCheckpointIds()
        {
            Interaction.Checkpoint.CheckpointController[] checkpoints = FindObjectsOfType<Interaction.Checkpoint.CheckpointController>();
            if (checkpoints.Length == 0)
                return;

            System.Collections.Generic.Dictionary<string, int> idCounters = new System.Collections.Generic.Dictionary<string, int>();

            for (int i = checkpoints.Length - 1; i >= 0; --i)
            {
                if (!idCounters.ContainsKey(checkpoints[i].Identifier.Id))
                    idCounters.Add(checkpoints[i].Identifier.Id, 1);
                else
                    idCounters[checkpoints[i].Identifier.Id]++;
            }

            foreach (System.Collections.Generic.KeyValuePair<string, int> idCounter in idCounters)
                if (idCounter.Value > 1)
                    LogError($"There are {idCounter.Value} checkpoints with the same Id \"{idCounter.Key}\" in the scene.");
        }

        private void SpawnPlayer()
        {
            if (!_stayAtSceneWindowPos && OptionalCheckpoint.Enabled)
                LogWarning($"Spawning player to an overridden checkpoint {OptionalCheckpoint.Value.transform.name}.");

            if (_stayAtSceneWindowPos)
                _playerCtrl.Init();
            else
                _playerCtrl.Init(OptionalCheckpoint.Enabled ? OptionalCheckpoint.Value : Interaction.Checkpoint.CheckpointController.CurrCheckpoint);
        }

        private System.Collections.IEnumerator SpawnPlayerCoroutine()
        {
            if (_fadeOnRespawn && CameraCtrl.GrayscaleRamp.enabled)
            {
                RampFadeManager.SetRampOffset(CameraCtrl.GrayscaleRamp, -1f);
                // [TODO] Hide player HUD.
            }

            yield return RSLib.Yield.SharedYields.WaitForEndOfFrame; // Wait for checkpoints initialization.

            SpawnPlayer();

            if (CameraCtrl.InitBoardBounds.Enabled && CameraCtrl.InitBoardBounds.Value != null)
            {
                CameraCtrl.SetBoardBounds(CameraCtrl.InitBoardBounds.Value);
                CameraCtrl.InitBoardBounds.Value.Board.OnBoardEntered();
            }
            else
            {
                Boards.Board initBoard = BoardsManager.DebugForceRefreshInitBoard(); // [TODO] We can call this here, but this is a debug unoptimized method right now.
                initBoard?.OnBoardEntered();
            }

            if (_fadeOnRespawn && CameraCtrl.GrayscaleRamp.enabled)
                RampFadeManager.Fade(CameraCtrl.GrayscaleRamp, "OutBase", (0.1f, 0f), (fadeIn) => _playerCtrl.AllowInputs(true));
            else
                _playerCtrl.AllowInputs(true);
        }

        protected override void Awake()
        {
            base.Awake();

#if UNITY_EDITOR
            CheckDuplicateCheckpointIds();
#endif

            if (_playerCtrl == null)
            {
                LogWarning("Reference to PlayerController is missing, trying to find it using FindObjectOfType.");
                _playerCtrl = FindObjectOfType<Unit.Player.PlayerController>();

                if (_playerCtrl == null)
                    LogError("No PlayerController seems to be in the scene.");
            }

            if (_cameraCtrl == null)
            {
                LogWarning("Reference to CameraController is missing, trying to find it using FindObjectOfType.");
                _cameraCtrl = FindObjectOfType<Templar.Camera.CameraController>();

                if (_cameraCtrl == null)
                    LogError("No CameraController seems to be in the scene.");
            }

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

            // Being in board transitions here means we're entering the scene from another gameplay scene, not from editor or a menu.
            if (!BoardsTransitionManager.IsInBoardTransition && _playerCtrl != null)
                StartCoroutine(SpawnPlayerCoroutine());
            else
                _playerCtrl.Init();
        }

        private void Start()
        {
            KillTrigger.ResetSharedTriggers();
            _checkpointListeners = RSLib.Helpers.FindInstancesOfType<ICheckpointListener>();

            // [TMP]
            RSLib.SceneReloader.BeforeReload += SaveManager.Save;

            if (!SaveManager.DisableLoading)
                SaveManager.TryLoad();
    
            SaveManager.Save();
        }

        private void OnDestroy()
        {
            // [TMP]
            RSLib.SceneReloader.BeforeReload -= SaveManager.Save;
        }

        [ContextMenu("Find All References")]
        private void DebugFindAllReferences()
        {
            _playerCtrl = FindObjectOfType<Unit.Player.PlayerController>();
            _cameraCtrl = FindObjectOfType<Templar.Camera.CameraController>();
            _inventoryCtrl = FindObjectOfType<Item.InventoryController>();
            _inventoryView = FindObjectOfType<UI.Inventory.InventoryView>();

            RSLib.EditorUtilities.SceneManagerUtilities.SetCurrentSceneDirty();
        }

        [ContextMenu("Find Missing References")]
        private void DebugFindMissingReferences()
        {
            _playerCtrl = _playerCtrl ?? FindObjectOfType<Unit.Player.PlayerController>();
            _cameraCtrl = _cameraCtrl ?? FindObjectOfType<Templar.Camera.CameraController>();
            _inventoryCtrl = _inventoryCtrl ?? FindObjectOfType<Item.InventoryController>();
            _inventoryView = _inventoryView ?? FindObjectOfType<UI.Inventory.InventoryView>();

            RSLib.EditorUtilities.SceneManagerUtilities.SetCurrentSceneDirty();
        }
    }
}