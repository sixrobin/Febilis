namespace Templar.Tools
{
    using RSLib.Extensions;
    using System.Linq;
    using UnityEngine;

    [DisallowMultipleComponent]
    public class CheckpointTeleporter : MonoBehaviour
    {
        [SerializeField] private Unit.Player.PlayerController _playerCtrl = null;
        [SerializeField] private CheckpointTeleportPanel _teleportPanelTemplate = null;
        [SerializeField] private Transform _teleportPanelsContainer = null;

        private Interaction.Checkpoint.CheckpointController[] _checkpoints;
        private CheckpointTeleportPanel[] _teleportPanels;

        private void ShowCheckpoints()
        {
            _checkpoints = FindObjectsOfType<Interaction.Checkpoint.CheckpointController>();
        }

        private void OnTeleportButtonClicked(string checkpointId)
        {
            _playerCtrl.transform.position = _checkpoints.Where(o => o.Id == checkpointId).First().RespawnPos.AddY(Templar.Physics.RaycastsController.SKIN_WIDTH * 10f);
        }

        private void Awake()
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

        private void OnDestroy()
        {
            for (int i = _teleportPanels.Length - 1; i >= 0; --i)
                _teleportPanels[i].TeleportButtonClicked -= OnTeleportButtonClicked;
        }

        [ContextMenu("Get Teleport Panels in Children")]
        private void GetTeleportPanelsInChildren()
        {
            _teleportPanels = GetComponentsInChildren<CheckpointTeleportPanel>();
        }
    }
}