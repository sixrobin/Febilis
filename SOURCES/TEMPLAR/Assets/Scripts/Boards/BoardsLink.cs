namespace Templar.Boards
{
    using System.Linq;
    using UnityEngine;

    [DisallowMultipleComponent]
    public class BoardsLink : MonoBehaviour
    {
        [SerializeField] private ScreenDirection _exitDir = ScreenDirection.NONE;
        [SerializeField] private RSLib.Framework.OptionalTransform _overrideRespawnPos = new RSLib.Framework.OptionalTransform(null);
        [SerializeField] private RSLib.Framework.OptionalTransform _enterTeleportPos = new RSLib.Framework.OptionalTransform(null, false);
        [SerializeField] private RSLib.Framework.OptionalFloat _overrideExitFadedInDur = new RSLib.Framework.OptionalFloat(1f);
        [SerializeField] private RSLib.Framework.OptionalFloat _overrideFadeInDelayDur = new RSLib.Framework.OptionalFloat(0.5f, false);

        [Header("DEBUG")]
        [SerializeField] private SpriteRenderer _dbgVisualizer = null;

        public ScreenDirection ExitDir => _exitDir;
        public ScreenDirection EnterDir => _exitDir.Opposite();

        public Transform OverrideRespawnPos => _overrideRespawnPos.Enabled ? _overrideRespawnPos.Value : null;
        public Transform EnterTeleportPos => _enterTeleportPos.Enabled ? _enterTeleportPos.Value : null;

        public bool OverrideExitFadedIn => _overrideExitFadedInDur.Enabled;
        public float OverrideExitFadedInDur => _overrideExitFadedInDur.Value;
        public bool OverrideFadedInDelay => _overrideFadeInDelayDur.Enabled;
        public float OverrideFadedInDelayDur => _overrideFadeInDelayDur.Value;

        public Board OwnerBoard { get; set; }

        public virtual void OnBoardsTransitionBegan()
        {
            gameObject.SetActive(false);
        }

        public virtual void OnBoardsTransitionOver()
        {
            gameObject.SetActive(true);
        }

        protected virtual void Trigger()
        {
            Manager.BoardsManager.TriggerLink(this);
        }

        private static void DisplayVisualizers(bool state)
        {
            FindObjectsOfType<BoardsLink>().Where(o => o._dbgVisualizer != null).ToList().ForEach(o => o._dbgVisualizer.enabled = state);
        }

        private void Awake()
        {
            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command<bool>("VisualizeBoardsLinks", "Shows the board links hitboxes.", DisplayVisualizers));
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            UnityEngine.Assertions.Assert.IsFalse(
                _exitDir == ScreenDirection.NONE,
                $"Boards Link instance on {transform.name} exit direction has an invalid value {_exitDir}.");

            if (other.GetComponent<Unit.Player.PlayerController>()) // [TODO] Remove GetComponent.
                Trigger();
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