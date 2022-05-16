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
        [SerializeField] private Templar.Tools.OptionalSceneLoader _targetScene = new Templar.Tools.OptionalSceneLoader(null, false);
        [SerializeField] private BoardBounds _containingBounds = null;

        [Header("PLATFORMS TO RESET")]
        [SerializeField] private Templar.Physics.MovingPlatform.MovingPlatformController.PlatformResetDatas[] _platformsToReset = null;

        [Header("TRANSITION VIEW")]
        [SerializeField] private ScreenDirection _exitDir = ScreenDirection.NONE;
        [SerializeField] private RSLib.Framework.OptionalTransform _overrideRespawnPos = new RSLib.Framework.OptionalTransform(null);
        [SerializeField] private RSLib.Framework.OptionalTransform _enterTeleportPos = new RSLib.Framework.OptionalTransform(null, false);
        [SerializeField] private RSLib.Framework.OptionalFloat _overrideExitFadedInDur = new RSLib.Framework.OptionalFloat(1f);
        [SerializeField] private RSLib.Framework.OptionalFloat _overrideFadeInDelayDur = new RSLib.Framework.OptionalFloat(0.5f, false);
        [SerializeField] private Templar.Tools.OptionalRampFadeDatas _overrideFadeDatas = new Templar.Tools.OptionalRampFadeDatas(null, false);

        [Header("DEBUG")]
        [SerializeField] private SpriteRenderer _dbgVisualizer = null;
        [SerializeField] private RSLib.DataColor _dbgColor = null;

        [System.Obsolete("Not technically obsolete, but should only be used in editor/debug classes. Use GetTarget method instead.")]
        public Templar.Tools.OptionalBoardsLink TargetBoardsLink => _targetBoardsLink;

        [System.Obsolete("Not technically obsolete, but should only be used in editor/debug classes. Use GetTarget method instead.")]
        public Templar.Tools.OptionalScenesPassage TargetScenePassage => _targetScenePassage;

        [System.Obsolete("Not technically obsolete, but should only be used in editor/debug classes. Use GetTarget method instead.")]
        public Templar.Tools.OptionalSceneLoader TargetScene => _targetScene;
        
        public ScreenDirection ExitDir => _exitDir;
        public ScreenDirection EnterDir => _exitDir.Opposite();

        public Transform OverrideRespawnPos => _overrideRespawnPos.Enabled ? _overrideRespawnPos.Value : null;
        public Transform EnterTeleportPos => _enterTeleportPos.Enabled ? _enterTeleportPos.Value : null;

        public bool OverrideExitFadedIn => _overrideExitFadedInDur.Enabled;
        public float OverrideExitFadedInDur => _overrideExitFadedInDur.Value;
        public bool OverrideFadedInDelay => _overrideFadeInDelayDur.Enabled;
        public float OverrideFadedInDelayDur => _overrideFadeInDelayDur.Value;
        public bool OverrideFadeDatas => _overrideFadeDatas.Enabled && OverrideFadeDatasValue != null;
        public Datas.RampFadeDatas OverrideFadeDatasValue => _overrideFadeDatas.Value;

        public Board Board { get; private set; }
        public BoardBounds BoardBounds => _containingBounds;

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

        public void SetBoard(Board board)
        {
            Board = board;
            BoardBounds.SetBoard(board);
        }

        public void OnBoardEntered()
        {
            Board.OnBoardEntered();

            for (int i = _platformsToReset.Length - 1; i >= 0; --i)
                _platformsToReset[i].ResetPlatform();
        }

        protected virtual void Trigger()
        {
            Manager.BoardsTransitionManager.TriggerLink(this);
        }

        public IBoardTransitionHandler GetTarget()
        {
            if (GetEnabledTransitionsCount() != 1)
            {
                CProLogger.LogError(this, $"Exactly one of the boards transitions references must be enabled within BoardsLink {transform.name}! Returning null.", gameObject);
                return null;
            }

            if (_targetBoardsLink.Enabled)
                return _targetBoardsLink.Value;
            if (_targetScenePassage.Enabled)
                return _targetScenePassage.Value;
            return _targetScene.Value;
        }

        public int GetEnabledTransitionsCount()
        {
            int enabledTransitions = 0;
            if (TargetBoardsLink.Enabled)
                enabledTransitions++;
            if (TargetScenePassage.Enabled)
                enabledTransitions++;
            if (TargetScene.Enabled)
                enabledTransitions++;

            return enabledTransitions;
        }
        
//        All those methods do not seem to work with prefab apply shit, even though they are called from ContextMenu...

//        [ContextMenu("Autoset Opposite Link (forced)")]
//        private void AutoSetOppositeBoardsLinkForced()
//        {
//            AutoSetOppositeBoardsLink(true);
//        }

//        [ContextMenu("Autoset Opposite Link (if null)")]
//        private void AutoSetOppositeBoardsLinkIfNull()
//        {
//            AutoSetOppositeBoardsLink(false);
//        }

//        private void AutoSetOppositeBoardsLink(bool force)
//        {
//            BoardsLink otherTargetLink = _targetBoardsLink.Value._targetBoardsLink.Value;

//            if (_targetBoardsLink.Value == null)
//            {
//                Debug.LogWarning("Cannot automatically set the opposite link while target BoardsLink is not referenced.");
//                return;
//            }

//            if (otherTargetLink != null)
//            {
//                if (otherTargetLink == this)
//                {
//                    Debug.Log($"Target BoardsLink of {otherTargetLink.name} is already set to {name}.");
//                    return;
//                }

//                if (force)
//                {
//                    Debug.LogWarning($"Automatically setting target passage of {_targetBoardsLink.Value.name} to {name}, overriding previous one {otherTargetLink.name}.");
//                    _targetBoardsLink.Value._targetBoardsLink = new Templar.Tools.OptionalBoardsLink(this, _targetBoardsLink.Enabled);
//                }
//            }
//            else
//            {
//                Debug.Log($"Automatically setting target link of {_targetBoardsLink.Value.name} to {name}.");
//                _targetBoardsLink.Value._targetBoardsLink = new Templar.Tools.OptionalBoardsLink(this, _targetBoardsLink.Enabled);
//            }

//#if UNITY_EDITOR
//            RSLib.EditorUtilities.SceneManagerUtilities.SetCurrentSceneDirty();
//#endif
//        }

        private void Awake()
        {
            RSLib.Debug.Console.DebugConsole.OverrideCommand<bool>("VisualizeBoardsLinks", "Shows the board links hitboxes.", DebugDisplayVisualizers);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            UnityEngine.Assertions.Assert.IsFalse(
                _exitDir == ScreenDirection.NONE,
                $"Boards Link instance on {transform.name} exit direction has an invalid value {_exitDir}.");

            if (other.TryGetComponent<Unit.Player.PlayerController>(out _))
                Trigger();
        }

        public void DebugDrawLineToTarget()
        {
#pragma warning disable CS0618
            if (!TargetBoardsLink.Enabled || TargetBoardsLink.Value == null)
                return;

            Gizmos.color = Manager.BoardsManager.DebugColor?.Color ?? RSLib.DataColor.Default;
            Gizmos.DrawLine(TargetBoardsLink.Value.transform.position, transform.position);
            Gizmos.DrawWireSphere(transform.position, 0.2f);
#pragma warning restore CS0618
        }

        private void OnDrawGizmos()
        {
            if (OverrideRespawnPos == null)
                return;

            Gizmos.color = Manager.BoardsManager.DebugColor?.Color ?? RSLib.DataColor.Default;
            Gizmos.DrawLine(OverrideRespawnPos.position, transform.position);
        }

        private void OnDrawGizmosSelected()
        {
            if (_platformsToReset != null)
            {
                for (int i = _platformsToReset.Length - 1; i >= 0; --i)
                {
                    if (_platformsToReset[i].Platform == null)
                        continue;

                    _platformsToReset[i].Platform.DrawWaypointsStartGizmos(
                        _platformsToReset[i].WaypointIndex,
                        _platformsToReset[i].Percentage,
                        _dbgColor?.Color ?? RSLib.DataColor.Default);
                }
            }
        }

        private static void DebugDisplayVisualizers(bool state)
        {
            FindObjectsOfType<BoardsLink>().Where(o => o._dbgVisualizer != null).ToList().ForEach(o => o._dbgVisualizer.enabled = state);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(BoardsLink))]
    public class BoardsLinkEditor : Editor
    {
        protected BoardsLink Obj { get; private set; }

        protected virtual void OnEnable()
        {
            Obj = (BoardsLink)target;
        }

        public override void OnInspectorGUI()
        {
            int enabledTransitions = Obj.GetEnabledTransitionsCount();
            
#pragma warning disable CS0618
            if (enabledTransitions > 1)
                EditorGUILayout.HelpBox("Only one of the boards transitions references must be enabled.", MessageType.Error);
            else if (enabledTransitions == 0)
                EditorGUILayout.HelpBox("Exactly one of the boards transitions references must be enabled.", MessageType.Error);

            if (Obj.TargetBoardsLink.Enabled && Obj.TargetBoardsLink.Value == null)
                EditorGUILayout.HelpBox("TargetBoardsLink is enabled but its value is missing.", MessageType.Warning);
            if (Obj.TargetScenePassage.Enabled && Obj.TargetScenePassage.Value == null)
                EditorGUILayout.HelpBox("TargetScenePassage is enabled but its value is missing.", MessageType.Warning);
            if (Obj.TargetScene.Enabled && Obj.TargetScene.Value == null)
                EditorGUILayout.HelpBox("TargetScene is enabled but its value is missing.", MessageType.Warning);
#pragma warning restore CS0618

            base.OnInspectorGUI();
        }
    }
#endif
}