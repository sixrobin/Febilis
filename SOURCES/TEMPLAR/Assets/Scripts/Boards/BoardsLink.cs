namespace Templar.Boards
{
    using System.Linq;
    using UnityEngine;

    [DisallowMultipleComponent]
    public class BoardsLink : MonoBehaviour
    {
        [SerializeField] private ScreenDirection _exitDir = ScreenDirection.NONE;
        [SerializeField] private Transform _overrideRespawnPos = null;

        [Header("DEBUG")]
        [SerializeField] private SpriteRenderer _visualizer = null;

        public ScreenDirection ExitDir => _exitDir;
        public ScreenDirection EnterDir => _exitDir.Opposite();

        public Transform OverrideRespawnPos => _overrideRespawnPos;

        public Board OwnerBoard { get; set; }

        private static void DisplayVisualizers(bool state)
        {
            FindObjectsOfType<BoardsLink>().ToList().ForEach(o => o._visualizer.enabled = state);
        }

        private void Awake()
        {
            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command<bool>("VisualizeBoardsLinks", "Shows the board links hitboxes.", DisplayVisualizers));
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            UnityEngine.Assertions.Assert.IsFalse(
                _exitDir == ScreenDirection.NONE,
                $"Boards Link instance on {transform.name} exit direction has an invalid value {_exitDir.ToString()}.");

            if (other.GetComponent<Unit.Player.PlayerController>())
                Manager.BoardsManager.TriggerLink(this);
        }

        private void OnDrawGizmos()
        {
            if (OverrideRespawnPos == null)
                return;

            Gizmos.color = Manager.BoardsManager.RespawnDebugColor?.Color ?? Color.yellow;
            Gizmos.DrawLine(OverrideRespawnPos.position, transform.position);
        }
    }
}