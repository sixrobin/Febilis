namespace Templar.Boards
{
    using System.Linq;
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [DisallowMultipleComponent]
    public class BoardsLink : MonoBehaviour
    {
        [SerializeField] private ScreenDirection _exitDir = ScreenDirection.NONE;
        [SerializeField] private RSLib.Framework.OptionalTransform _overrideRespawnPos = new RSLib.Framework.OptionalTransform(null);
        [SerializeField] private RSLib.Framework.OptionalTransform _enterTeleportPos = new RSLib.Framework.OptionalTransform(null, false);
        [SerializeField] private RSLib.Framework.OptionalFloat _overrideExitFadedInDur = new RSLib.Framework.OptionalFloat(1f);
        [SerializeField] private RSLib.Framework.OptionalFloat _overrideFadeInDelayDur = new RSLib.Framework.OptionalFloat(0.5f, false);

        [Header("TEST")]
        [SerializeField] private Templar.Tools.OptionalBoardsLink _targetBoardsLink = new Templar.Tools.OptionalBoardsLink(null, true);
        [SerializeField] private Templar.Tools.OptionalScenesPassage _targetScenePassage = new Templar.Tools.OptionalScenesPassage(null, false);

        [Header("DEBUG")]
        [SerializeField] private SpriteRenderer _dbgVisualizer = null;

        public ScreenDirection ExitDir => _exitDir;
        public ScreenDirection EnterDir => _exitDir.Opposite();

        public Transform OverrideRespawnPos => _overrideRespawnPos.Enabled ? _overrideRespawnPos.Value : null;
        public Transform EnterTeleportPos => _enterTeleportPos.Enabled ? _enterTeleportPos.Value : null;

        public Templar.Tools.OptionalBoardsLink TargetBoardsLink => _targetBoardsLink;
        public Templar.Tools.OptionalScenesPassage TargetScenePassage => _targetScenePassage;

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

#if UNITY_EDITOR
    [CustomEditor(typeof(BoardsLink))]
    public class BoardsLinkEditor : RSLib.EditorUtilities.ButtonProviderEditor<BoardsLink>
    {
        public override void OnInspectorGUI()
        {
            if (Obj.TargetBoardsLink.Enabled && Obj.TargetScenePassage.Enabled)
                EditorGUILayout.HelpBox("Only one of the boards transitions references must be enabled.", MessageType.Error);
            else if (!Obj.TargetBoardsLink.Enabled && !Obj.TargetScenePassage.Enabled)
                EditorGUILayout.HelpBox("Exactly one of the boards transitions references must be enabled.", MessageType.Error);

            base.OnInspectorGUI();
        }

        protected override void DrawButtons()
        {
            // [TODO] Button to automatically set the opposite link.
        }
    }
#endif
}