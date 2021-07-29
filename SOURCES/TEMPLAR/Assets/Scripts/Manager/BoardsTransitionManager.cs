namespace Templar.Manager
{
    using RSLib.Extensions;
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    public class BoardsTransitionManager : RSLib.Framework.ConsoleProSingleton<BoardsTransitionManager>
    {
        [Header("INIT BOARD ON SCENE START")]
        // [TODO] Auto detect using camera bounds to check if player is inside ?
        [SerializeField] private Templar.Tools.OptionalBoard _initBoard = new Templar.Tools.OptionalBoard(null, false); 

        [Header("TRANSITION VIEW")]
        [SerializeField] private Datas.RampFadeDatas _fadeInDatas = null;
        [SerializeField] private Datas.RampFadeDatas _fadeOutDatas = null;
        [SerializeField, Min(0f)] private float _fadedInDur = 0.5f;
        [SerializeField] private float _downRespawnHeightOffset = 1f;

        [Header("DEBUG")]
        [SerializeField] private RSLib.DataColor _debugColor = null;
        [SerializeField] private RSLib.DataColor _respawnDebugColor = null;

        private static System.Collections.IEnumerator s_boardTransitionCoroutine;
        private static System.Collections.IEnumerator s_playerMovementCoroutine;

        public static bool IsInBoardTransition => s_boardTransitionCoroutine != null;

        public static RSLib.DataColor RespawnDebugColor => Instance._respawnDebugColor;

        public static void TriggerLink(Boards.BoardsLink link)
        {
            Instance.Log($"{link.transform.name} triggered.", link.gameObject);
            Instance.StartCoroutine(s_boardTransitionCoroutine = BoardTransitionCoroutine(link));
        }

        private static System.Collections.IEnumerator BoardTransitionCoroutine(Boards.BoardsLink source)
        {
            Boards.IBoardTransitionHandler target = source.GetTarget();
            Boards.BoardsLink targetBoardsLink = target as Boards.BoardsLink;
            Boards.ScenesPassage targetScenesPassage = target as Boards.ScenesPassage;

            source.OnBoardsTransitionBegan();
            target.OnBoardsTransitionBegan();

            GameManager.PlayerCtrl.AllowInputs(false);

            if (source.EnterTeleportPos != null)
            {
                GameManager.PlayerCtrl.transform.position = source.EnterTeleportPos.position;
                GameManager.CameraCtrl.ToggleFreeze(true);
            }

            switch (source.ExitDir)
            {
                case ScreenDirection.RIGHT:
                case ScreenDirection.LEFT:
                    Vector2 outDir = source.ExitDir.ConvertToVector2();
                    Instance.StartCoroutine(s_playerMovementCoroutine = GameManager.PlayerCtrl.MoveToDirection(outDir.x, 0.4f));
                    break;

                case ScreenDirection.UP:
                    Instance.StartCoroutine(s_playerMovementCoroutine = GameManager.PlayerCtrl.KeepUpwardMovement(0.4f));
                    break;

                case ScreenDirection.DOWN:
                    // Just let player fall.
                    break;

                case ScreenDirection.IN:
                    GameManager.PlayerCtrl.RollCtrl.Interrupt();
                    GameManager.PlayerCtrl.AttackCtrl.CancelAttack();
                    GameManager.PlayerCtrl.PlayerView.PlayTransitionInAnimation();
                    break;

                default:
                    Instance.LogError($"Unhandled exit direction {source.ExitDir} for board link {source.transform.name}.", source.gameObject);
                    yield break;
            }

            if (source.OverrideFadedInDelay)
                yield return RSLib.Yield.SharedYields.WaitForSeconds(source.OverrideFadedInDelayDur);

            yield return WaitForFadeIn(source);

            if (s_playerMovementCoroutine != null)
                Instance.StopCoroutine(s_playerMovementCoroutine);

            yield return targetBoardsLink
                ? TransitionOutToBoardsLink(source, targetBoardsLink)
                : TransitionOutToScenesPassage(source, targetScenesPassage);

            s_boardTransitionCoroutine = null;
            s_playerMovementCoroutine = null;

            if (source != null)
                source.OnBoardsTransitionOver();
            target.OnBoardsTransitionOver();
        }

        private static System.Collections.IEnumerator WaitForFadeIn(Boards.BoardsLink source)
        {
            RampFadeManager.Fade(GameManager.CameraCtrl.GrayscaleRamp, Instance._fadeInDatas, (0f, 0f));
            yield return RSLib.Yield.SharedYields.WaitForEndOfFrame;
            yield return new WaitUntil(() => !RampFadeManager.IsFading);
            yield return new WaitForSeconds(source.OverrideExitFadedIn ? source.OverrideExitFadedInDur : Instance._fadedInDur);
        }

        private static System.Collections.IEnumerator TransitionOutToBoardsLink(Boards.BoardsLink source, Boards.BoardsLink target)
        {
            RampFadeManager.Fade(GameManager.CameraCtrl.GrayscaleRamp, Instance._fadeOutDatas, (0f, 0f));

            GameManager.PlayerCtrl.ResetVelocity();
            GameManager.PlayerCtrl.transform.position = target.OverrideRespawnPos != null ? target.OverrideRespawnPos.position : target.transform.position; // ?? operator does not seem to work.

            yield return null;

            GameManager.CameraCtrl.ToggleFreeze(false);
            GameManager.CameraCtrl.SetBoardBounds(target.OwnerBoard);
            GameManager.CameraCtrl.PositionInstantly();

            yield return null;

            switch (target.EnterDir)
            {
                case ScreenDirection.RIGHT:
                case ScreenDirection.LEFT:
                {
                    // This is fucking ugly, but for now we need to wait a frame after we reset the velocity and then
                    // set the position again before groundind the character. Without waiting, we set the position, but the potential
                    // fall velocity will make the character fall under the ground, and the Groud method won't work properly.
                    yield return null;
                    GameManager.PlayerCtrl.transform.position = target.transform.position;
                    GameManager.PlayerCtrl.CollisionsCtrl.Ground(GameManager.PlayerCtrl.transform, true);
                    GameManager.PlayerCtrl.PlayerView.PlayIdleAnimation();

                    Vector2 inDir = target.EnterDir.ConvertToVector2();
                    Instance.StartCoroutine(s_playerMovementCoroutine = GameManager.PlayerCtrl.MoveToDirection(inDir.x, 0.4f));
                    break;
                }

                case ScreenDirection.UP:
                    UnityEngine.Assertions.Assert.IsNotNull(target.OverrideRespawnPos, $"BoardsLink with enter direction {target.EnterDir} must have an OverrideRespawnPos.");
                    break;

                case ScreenDirection.DOWN:
                    GameManager.PlayerCtrl.transform.AddPositionY(Instance._downRespawnHeightOffset);
                    break;

                case ScreenDirection.OUT:
                    GameManager.PlayerCtrl.PlayerView.PlayTransitionOutAnimation();
                    break;

                default:
                    Instance.LogError($"Unhandled enter direction {target.EnterDir} for board link {target.transform.name}.", target.gameObject);
                    yield break;
            }

            yield return RSLib.Yield.SharedYields.WaitForEndOfFrame;
            yield return new WaitUntil(() => !RampFadeManager.IsFading);

            GameManager.PlayerCtrl.AllowInputs(true);
        }

        private static System.Collections.IEnumerator TransitionOutToScenesPassage(Boards.BoardsLink source, Boards.ScenesPassage target)
        {
            // GetTarget() must be called here, because the source referenced will be missing after loading scene.
            Boards.ScenesPassage sourceScenesPassage = source.GetTarget() as Boards.ScenesPassage;

            UnityEngine.SceneManagement.SceneManager.LoadScene(target.TargetPassage.GetTargetScene());

            // [TMP] Generic coroutine. Also make sure this always works. And compare scene id instead of name.
            while (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != target.GetTargetScene().SceneName)
                yield return null;
            yield return null;

            Boards.BoardsLink targetBoardsLink = BoardsLinksManager.GetLinkRelatedToScenesPassage(sourceScenesPassage);
            yield return TransitionOutToBoardsLink(source, targetBoardsLink);
        }

        protected override void Awake()
        {
            base.Awake();

            if (_initBoard.Enabled)
                GameManager.CameraCtrl.SetBoardBounds(_initBoard.Value);
        }

        public static void DebugAutoDetectInitBoard()
        {
            Boards.Board[] boards = FindObjectsOfType<Boards.Board>();
            Unit.Player.PlayerController playerCtrl = FindObjectOfType<Unit.Player.PlayerController>();

            for (int i = boards.Length - 1; i >= 0; --i)
            {
                if (boards[i].CameraBounds.bounds.Contains(playerCtrl.transform.position))
                {
                    Instance.Log($"Detected board {boards[i].transform.name} as the init board.", Instance.gameObject);
                    Instance._initBoard = new Templar.Tools.OptionalBoard(boards[i], true);
                    return;
                }
            }

            Instance.LogWarning($"No board has been fonud as the init board.", Instance.gameObject);
        }

        public static void DebugForceRefreshBoard()
        {
            Boards.Board[] boards = FindObjectsOfType<Boards.Board>();
            Unit.Player.PlayerController playerCtrl = FindObjectOfType<Unit.Player.PlayerController>();

            for (int i = boards.Length - 1; i >= 0; --i)
                if (boards[i].CameraBounds.bounds.Contains(playerCtrl.transform.position))
                    Manager.GameManager.CameraCtrl.SetBoardBounds(boards[i]);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(BoardsTransitionManager))]
    public class BoardsManagerEditor : RSLib.EditorUtilities.ButtonProviderEditor<BoardsTransitionManager>
    {
        protected override void DrawButtons()
        {
            DrawButton("Auto Detect Init Board", BoardsTransitionManager.DebugAutoDetectInitBoard);
            DrawButton("Refresh Current Board", BoardsTransitionManager.DebugForceRefreshBoard);
        }
    }
#endif
}