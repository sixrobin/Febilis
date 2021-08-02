namespace Templar.Boards
{
    using System.Linq;
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [DisallowMultipleComponent]
    public class BoardsLink : MonoBehaviour, IBoardTransitionHandler
    {
        [Header("TRANSITION REFS")]
        [SerializeField] private Templar.Tools.OptionalBoardsLink _targetBoardsLink = new Templar.Tools.OptionalBoardsLink(null, true);
        [SerializeField] private Templar.Tools.OptionalScenesPassage _targetScenePassage = new Templar.Tools.OptionalScenesPassage(null, false);

        [Header("TRANSITION VIEW")]
        [SerializeField] private ScreenDirection _exitDir = ScreenDirection.NONE;
        [SerializeField] private RSLib.Framework.OptionalTransform _overrideRespawnPos = new RSLib.Framework.OptionalTransform(null);
        [SerializeField] private RSLib.Framework.OptionalTransform _enterTeleportPos = new RSLib.Framework.OptionalTransform(null, false);
        [SerializeField] private RSLib.Framework.OptionalFloat _overrideExitFadedInDur = new RSLib.Framework.OptionalFloat(1f);
        [SerializeField] private RSLib.Framework.OptionalFloat _overrideFadeInDelayDur = new RSLib.Framework.OptionalFloat(0.5f, false);

        [Header("DEBUG")]
        [SerializeField] private SpriteRenderer _dbgVisualizer = null;

        [System.Obsolete("Not technically obsolete, but should only be used in editor classes. Use GetTarget method instead.")]
        public Templar.Tools.OptionalBoardsLink TargetBoardsLink => _targetBoardsLink;

        [System.Obsolete("Not technically obsolete, but should only be used in editor classes. Use GetTarget method instead.")]
        public Templar.Tools.OptionalScenesPassage TargetScenePassage => _targetScenePassage;

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

        public bool CompareTargetPassage(ScenesPassage passage)
        {
            return GetTarget() is ScenesPassage scenesPassage && scenesPassage.TargetPassage == passage;
        }

        protected virtual void Trigger()
        {
            Manager.BoardsTransitionManager.TriggerLink(this);
        }

        public IBoardTransitionHandler GetTarget()
        {
            if (_targetBoardsLink.Enabled == _targetScenePassage.Enabled)
            {
                CProLogger.LogError(this, $"Exactly one of the boards transitions references must be enabled within BoardsLink {transform.name}! Returning null.", gameObject);
                return null;
            }

            return _targetBoardsLink.Enabled
                ? (IBoardTransitionHandler)_targetBoardsLink.Value
                : (IBoardTransitionHandler)_targetScenePassage.Value;
        }

        public void AutoSetOppositeBoardsLink(bool force)
        {
            BoardsLink otherTargetLink = _targetBoardsLink.Value._targetBoardsLink.Value;

            if (_targetBoardsLink.Value == null)
            {
                Debug.LogWarning("Cannot automatically set the opposite link while target BoardsLink is not referenced.");
                return;
            }

            if (otherTargetLink != null)
            {
                if (otherTargetLink == this)
                {
                    Debug.Log($"Target BoardsLink of {otherTargetLink.name} is already set to {name}.");
                    return;
                }

                if (force)
                {
                    Debug.LogWarning($"Automatically setting target passage of {_targetBoardsLink.Value.name} to {name}, overriding previous one {otherTargetLink.name}.");
                    _targetBoardsLink.Value._targetBoardsLink = new Templar.Tools.OptionalBoardsLink(this, _targetBoardsLink.Enabled);
                }
            }
            else
            {
                Debug.Log($"Automatically setting target link of {_targetBoardsLink.Value.name} to {name}.");
                _targetBoardsLink.Value._targetBoardsLink = new Templar.Tools.OptionalBoardsLink(this, _targetBoardsLink.Enabled);
            }

#if UNITY_EDITOR
            RSLib.EditorUtilities.SceneManagerUtilities.SetCurrentSceneDirty();
#endif
        }

        private void Awake()
        {
            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command<bool>("VisualizeBoardsLinks", "Shows the board links hitboxes.", DebugDisplayVisualizers));
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

            Gizmos.color = Manager.BoardsManager.DebugColor?.Color ?? RSLib.DataColor.Default;
            Gizmos.DrawLine(OverrideRespawnPos.position, transform.position);
        }

        private static void DebugDisplayVisualizers(bool state)
        {
            FindObjectsOfType<BoardsLink>().Where(o => o._dbgVisualizer != null).ToList().ForEach(o => o._dbgVisualizer.enabled = state);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(BoardsLink))]
    public class BoardsLinkEditor : RSLib.EditorUtilities.ButtonProviderEditor<BoardsLink>
    {
        public override void OnInspectorGUI()
        {
#pragma warning disable CS0618
            if (Obj.TargetBoardsLink.Enabled && Obj.TargetScenePassage.Enabled)
                EditorGUILayout.HelpBox("Only one of the boards transitions references must be enabled.", MessageType.Error);
            else if (!Obj.TargetBoardsLink.Enabled && !Obj.TargetScenePassage.Enabled)
                EditorGUILayout.HelpBox("Exactly one of the boards transitions references must be enabled.", MessageType.Error);

            if (Obj.TargetBoardsLink.Enabled && Obj.TargetBoardsLink.Value == null)
                EditorGUILayout.HelpBox("TargetBoardsLink is enabled but its value is missing.", MessageType.Warning);
            if (Obj.TargetScenePassage.Enabled && Obj.TargetScenePassage.Value == null)
                EditorGUILayout.HelpBox("TargetScenePassage is enabled but its value is missing.", MessageType.Warning);
#pragma warning restore CS0618

            base.OnInspectorGUI();
        }

        protected override void DrawButtons()
        {
            DrawButton("Autoset Opposite BoardsLink (if null)", () => Obj.AutoSetOppositeBoardsLink(false));
            DrawButton("Autoset Opposite BoardsLink (forced)", () => Obj.AutoSetOppositeBoardsLink(true));
        }
    }
#endif
}