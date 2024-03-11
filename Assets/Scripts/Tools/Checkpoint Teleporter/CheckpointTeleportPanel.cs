namespace Templar.Tools
{
    using UnityEngine;

    public class CheckpointTeleportPanel : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI _checkpointIdText = null;
        [SerializeField] private UnityEngine.UI.Button _teleportButton = null;

        public string CheckpointId { get; private set; }

        public delegate void TeleportButtonClickedEventHandler(string checkpointId);
        public event TeleportButtonClickedEventHandler TeleportButtonClicked;

        public void Init(string checkpointId, TeleportButtonClickedEventHandler onTeleportButtonClicked)
        {
            CheckpointId = checkpointId;

            transform.name = transform.name.Replace("(Clone)", $" {CheckpointId}");
            _checkpointIdText.text = CheckpointId;

            TeleportButtonClicked += onTeleportButtonClicked;

            Show();
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void Awake()
        {
            _teleportButton.onClick.AddListener(() => TeleportButtonClicked?.Invoke(CheckpointId));
        }

        private void OnDestroy()
        {
            _teleportButton.onClick.RemoveAllListeners();
        }
    }
}