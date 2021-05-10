namespace Templar.Tools
{
    using RSLib.Extensions;
    using System.Linq;
    using UnityEngine;

    [DisallowMultipleComponent]
    public class CheckpointTeleporter : RSLib.Framework.Singleton<CheckpointTeleporter>
    {
        [SerializeField] private KeyCode _toggleKey = KeyCode.None;
        [SerializeField] private Canvas _canvas = null;
        [SerializeField] private Unit.Player.PlayerController _playerCtrl = null;
        [SerializeField] private CheckpointTeleportPanel _teleportPanelTemplate = null;
        [SerializeField] private Transform _teleportPanelsContainer = null;

        private Interaction.Checkpoint.CheckpointController[] _checkpoints;
        private CheckpointTeleportPanel[] _teleportPanels;

        public static bool IsOpen { get; private set; }

        private void ShowCheckpoints()
        {
            _checkpoints = FindObjectsOfType<Interaction.Checkpoint.CheckpointController>();
        }

        private void OnTeleportButtonClicked(string checkpointId)
        {
            _playerCtrl.transform.position = _checkpoints.Where(o => o.Id == checkpointId).First().RespawnPos.AddY(Templar.Physics.RaycastsController.SKIN_WIDTH * 10f);
            _playerCtrl.CollisionsCtrl.Ground(_playerCtrl.transform);
            _playerCtrl.CameraCtrl.PositionInstantly();
        }

        protected override void Awake()
        {
            if (_playerCtrl == null)
                _playerCtrl = FindObjectOfType<Unit.Player.PlayerController>();

            _checkpoints = FindObjectsOfType<Interaction.Checkpoint.CheckpointController>();
            _teleportPanels = new CheckpointTeleportPanel[_checkpoints.Length];

            for (int i = _checkpoints.Length - 1; i >= 0; --i)
            {
                _teleportPanels[i] = Instantiate(_teleportPanelTemplate, _teleportPanelsContainer);
                _teleportPanels[i].Init(_checkpoints[i].Id, OnTeleportButtonClicked);
            }

            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        }

        private void Update()
        {
            if (Input.GetKeyDown(_toggleKey))
            {
                IsOpen = !IsOpen;
                _canvas.enabled = IsOpen;
            }
        }

        private void OnDestroy()
        {
            for (int i = _teleportPanels.Length - 1; i >= 0; --i)
                _teleportPanels[i].TeleportButtonClicked -= OnTeleportButtonClicked;
        }
    }
}