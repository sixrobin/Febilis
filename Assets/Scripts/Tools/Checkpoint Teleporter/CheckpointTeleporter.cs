﻿namespace Templar.Tools
{
    using RSLib.Extensions;
    using System.Linq;
    using UnityEngine;

    public class CheckpointTeleporter : RSLib.Framework.Singleton<CheckpointTeleporter>
    {
        [SerializeField] private Canvas _canvas = null;
        [SerializeField] private Unit.Player.PlayerController _playerCtrl = null;
        [SerializeField] private CheckpointTeleportPanel _teleportPanelTemplate = null;
        [SerializeField] private Transform _teleportPanelsContainer = null;
        [SerializeField] private UnityEngine.UI.Button _closeBtn = null;

        private Interaction.Checkpoint.CheckpointController[] _checkpoints;
        private CheckpointTeleportPanel[] _teleportPanels;

        public static bool IsOpen { get; private set; }

        private void OnTeleportButtonClicked(string checkpointId)
        {
            Interaction.Checkpoint.CheckpointController checkpoint = _checkpoints.Where(o => o.Identifier.Id == checkpointId).First();

            Log($"Teleporting player to checkpoint {checkpointId} (position {checkpoint.RespawnPos}).", true);

            _playerCtrl.transform.position = checkpoint.RespawnPos.AddY(Templar.Physics.RaycastsController.SKIN_WIDTH * 10f);
            _playerCtrl.CollisionsCtrl.Ground(_playerCtrl.transform);
            Manager.GameManager.CameraCtrl.PositionInstantly();

            if (Manager.BoardsManager.Exists())
                Manager.BoardsManager.DebugForceRefreshInitBoard();
        }

        private void TogglePanel()
        {
            IsOpen = !IsOpen;
            _canvas.enabled = IsOpen;

            if (IsOpen)
                UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(_canvas.GetComponent<RectTransform>());
        }

        protected override void Awake()
        {
            if (_playerCtrl == null)
                _playerCtrl = FindObjectOfType<Unit.Player.PlayerController>();

            _closeBtn.onClick.AddListener(TogglePanel);

            _checkpoints = FindObjectsOfType<Interaction.Checkpoint.CheckpointController>().OrderBy(o => o.transform.name).ToArray();
            _teleportPanels = new CheckpointTeleportPanel[_checkpoints.Length];

            for (int i = 0; i < _checkpoints.Length; ++i)
            {
                _teleportPanels[i] = Instantiate(_teleportPanelTemplate, _teleportPanelsContainer);
                _teleportPanels[i].Init(_checkpoints[i].Identifier.Id, OnTeleportButtonClicked);
            }

            RSLib.Debug.Console.DebugConsole.OverrideCommand("OpenCheckpointTeleporter", "Opens checkpoint teleporter panel.", TogglePanel);
            RSLib.Debug.Console.DebugConsole.OverrideCommand<string>("GoToCheckpoint", "Teleports player to a checkpoint.", OnTeleportButtonClicked);
        }

        private void OnDestroy()
        {
            _closeBtn.onClick.RemoveListener(TogglePanel);

            for (int i = _teleportPanels.Length - 1; i >= 0; --i)
                _teleportPanels[i].TeleportButtonClicked -= OnTeleportButtonClicked;
        }
    }
}