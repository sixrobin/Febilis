namespace Templar.Manager
{
    using Boards;
    using RSLib.Extensions;
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    public class BoardsManager : RSLib.Framework.ConsoleProSingleton<BoardsManager>
    {
        [System.Serializable]
        private class BoardsLinkPair
        {
            [SerializeField] private BoardsLink _first = null;
            [SerializeField] private BoardsLink _second = null;

            public BoardsLink First => _first;
            public BoardsLink Second => _second;
        }

        [SerializeField] private Board _initBoard = null; // [TODO] Auto detect using camera bounds to check if player is inside ?
        [SerializeField] private BoardsLinkPair[] _links = null;
        // [TODO] SceneLoaders[] <=> BoardsLinkPair that leads to another scene.

        [Header("TRANSITION VIEW")]
        [SerializeField] private Datas.RampFadeDatas _fadeInDatas = null;
        [SerializeField] private Datas.RampFadeDatas _fadeOutDatas = null;
        [SerializeField] private float _fadedInDur = 0.5f;
        [SerializeField] private float _downRespawnHeightOffset = 1f;

        [Header("DEBUG")]
        [SerializeField] private RSLib.DataColor _debugColor = null;
        [SerializeField] private RSLib.DataColor _respawnDebugColor = null;

        private static System.Collections.Generic.Dictionary<BoardsLink, BoardsLink> s_linksPairs = new System.Collections.Generic.Dictionary<BoardsLink, BoardsLink>();

        private static System.Collections.IEnumerator s_boardTransitionCoroutine;
        private static System.Collections.IEnumerator s_playerMovementCoroutine;

        public static bool IsInBoardTransition => s_boardTransitionCoroutine != null;

        public static RSLib.DataColor RespawnDebugColor => Instance._respawnDebugColor;

        public static void TriggerLink(BoardsLink link)
        {
            Instance.Log($"{link.transform.name} triggered.", link.gameObject);
            UnityEngine.Assertions.Assert.IsTrue(
                s_linksPairs.ContainsKey(link),
                $"Unknown {typeof(BoardsLink).Name} instance by {Instance.GetType().Name} on {link.transform.name}.");

            Instance.StartCoroutine(s_boardTransitionCoroutine = BoardTransitionCoroutine(link, s_linksPairs[link]));
        }

        private static System.Collections.IEnumerator BoardTransitionCoroutine(BoardsLink source, BoardsLink target)
        {
            source.gameObject.SetActive(false);
            target.gameObject.SetActive(false);

            GameManager.PlayerCtrl.AllowInputs(false);

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

                default:
                    Instance.LogError($"Invalid exit direction {source.ExitDir.ToString()} for board link {source.transform.name}.", source.gameObject);
                    yield break;
            }

            RampFadeManager.Fade(GameManager.CameraCtrl.GrayscaleRamp, Instance._fadeInDatas, (0f, 0f));
            yield return RSLib.Yield.SharedYields.WaitForEndOfFrame;
            yield return new WaitUntil(() => !RampFadeManager.IsFading);

            if (s_playerMovementCoroutine != null)
                Instance.StopCoroutine(s_playerMovementCoroutine);

            yield return new WaitForSeconds(Instance._fadedInDur);

            GameManager.PlayerCtrl.ResetVelocity();
            GameManager.PlayerCtrl.transform.position = target.OverrideRespawnPos != null ? target.OverrideRespawnPos.position : target.transform.position; // ?? operator does not seem to work.

            yield return null;
            GameManager.CameraCtrl.SetBoardBounds(target.OwnerBoard);
            GameManager.CameraCtrl.PositionInstantly();

            yield return null;
            RampFadeManager.Fade(GameManager.CameraCtrl.GrayscaleRamp, Instance._fadeOutDatas, (0f, 0f));

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

                default:
                    Instance.LogError($"Invalid enter direction {target.EnterDir.ToString()} for board link {target.transform.name}.", target.gameObject);
                    yield break;
            }

            yield return RSLib.Yield.SharedYields.WaitForEndOfFrame;
            yield return new WaitUntil(() => !RampFadeManager.IsFading);

            GameManager.PlayerCtrl.AllowInputs(true);

            source.gameObject.SetActive(true);
            target.gameObject.SetActive(true);

            s_boardTransitionCoroutine = null;
            s_playerMovementCoroutine = null;
        }

        protected override void Awake()
        {
            for (int i = _links.Length - 1; i >= 0; --i)
            {
                s_linksPairs.Add(_links[i].First, _links[i].Second);
                s_linksPairs.Add(_links[i].Second, _links[i].First);
            }

            if (_initBoard != null)
                GameManager.CameraCtrl.SetBoardBounds(_initBoard);
        }

        private void OnDrawGizmos()
        {
            if (_links == null || _links.Length == 0)
                return;

            Gizmos.color = _debugColor?.Color ?? Color.yellow;

            for (int i = _links.Length - 1; i >= 0; --i)
                if (_links[i].First != null && _links[i].Second != null)
                    Gizmos.DrawLine(_links[i].First.transform.position, _links[i].Second.transform.position);
        }

        public static void DebugAutoDetectInitBoard()
        {
            Board[] boards = FindObjectsOfType<Board>();
            Unit.Player.PlayerController playerCtrl = FindObjectOfType<Unit.Player.PlayerController>();

            for (int i = boards.Length - 1; i >= 0; --i)
            {
                if (boards[i].CameraBounds.bounds.Contains(playerCtrl.transform.position))
                {
                    Instance.Log($"Detected board {boards[i].transform.name} as the init board.", Instance.gameObject);
                    Instance._initBoard = boards[i];
                    return;
                }
            }

            Instance.LogWarning($"No board has been fonud as the init board.", Instance.gameObject);
        }

        public static void DebugForceRefreshBoard()
        {
            Board[] boards = FindObjectsOfType<Board>();
            Unit.Player.PlayerController playerCtrl = FindObjectOfType<Unit.Player.PlayerController>();

            for (int i = boards.Length - 1; i >= 0; --i)
                if (boards[i].CameraBounds.bounds.Contains(playerCtrl.transform.position))
                    Manager.GameManager.CameraCtrl.SetBoardBounds(boards[i]);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(BoardsManager))]
    public class BoardsManagerEditor : RSLib.EditorUtilities.ButtonProviderEditor<BoardsManager>
    {
        protected override void DrawButtons()
        {
            DrawButton("Auto Detect Init Board", BoardsManager.DebugAutoDetectInitBoard);
            DrawButton("Refresh Current Board", BoardsManager.DebugForceRefreshBoard);
        }
    }
#endif
}