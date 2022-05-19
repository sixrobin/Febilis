namespace Templar.Manager
{
    using RSLib.Extensions;
    using UnityEngine;

    public class BoardsTransitionManager : RSLib.Framework.ConsoleProSingleton<BoardsTransitionManager>
    {
        [Header("TRANSITION VIEW")]
        [SerializeField] private Datas.RampFadeDatas _fadeInDatas = null;
        [SerializeField] private Datas.RampFadeDatas _fadeOutDatas = null;
        [SerializeField, Min(0f)] private float _fadedInDur = 0.5f;
        [SerializeField] private float _downRespawnHeightOffset = 1f;

        private static System.Collections.IEnumerator s_boardTransitionCoroutine;
        private static System.Collections.IEnumerator s_playerMovementCoroutine;

        public delegate void BoardsTransitionEventHandler();
        public event BoardsTransitionEventHandler BoardsTransitionTriggered;
        public event BoardsTransitionEventHandler BoardsTransitionOver;

        public static bool IsInBoardTransition => s_boardTransitionCoroutine != null;

        public static void TriggerLink(Boards.BoardsLink link)
        {
            Instance.Log($"{link.transform.name} triggered.", link.gameObject);
            Instance.StartCoroutine(s_boardTransitionCoroutine = TransitionInCoroutine(link));
        }

        private static System.Collections.IEnumerator TransitionInCoroutine(Boards.BoardsLink source)
        {
            Instance.BoardsTransitionTriggered?.Invoke();

            Boards.IBoardTransitionHandler target = source.GetTarget();
            
            Boards.BoardsLink targetBoardsLink = target as Boards.BoardsLink;
            Boards.ScenesPassage targetScenesPassage = target as Boards.ScenesPassage;
            Boards.SceneLoader targetScene = target as Boards.SceneLoader;

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

            yield return WaitForFadeInCoroutine(source);

            if (s_playerMovementCoroutine != null)
                Instance.StopCoroutine(s_playerMovementCoroutine);

            if (targetScene != null)
            {
                s_boardTransitionCoroutine = null;
                s_playerMovementCoroutine = null;
                
                ScenesManager.LoadScene(targetScene.GetTargetScene());
                yield break;
            }
            
            yield return targetBoardsLink
                ? TransitionOutToBoardsLinkCoroutine(source, targetBoardsLink)
                : TransitionOutToScenesPassageCoroutine(source, targetScenesPassage);

            s_boardTransitionCoroutine = null;
            s_playerMovementCoroutine = null;

            if (source != null)
                source.OnBoardsTransitionOver();
            
            target.OnBoardsTransitionOver();
        }

        private static System.Collections.IEnumerator WaitForFadeInCoroutine(Boards.BoardsLink source)
        {
            Templar.Datas.RampFadeDatas fadeData = source.OverrideFadeDatas ? source.OverrideFadeDatasValue : Instance._fadeInDatas;
            RampFadeManager.Fade(GameManager.CameraCtrl.GrayscaleRamp, fadeData, (0f, 0f));
            
            yield return RSLib.Yield.SharedYields.WaitForEndOfFrame;
            yield return new WaitUntil(() => !RampFadeManager.IsFading);
            yield return new WaitForSeconds(source.OverrideExitFadedIn ? source.OverrideExitFadedInDur : Instance._fadedInDur);
        }

        private static System.Collections.IEnumerator TransitionOutToBoardsLinkCoroutine(Boards.BoardsLink source, Boards.BoardsLink target)
        {
            source.OnBoardExit();
            target.OnBoardEntered();

            GameManager.CameraCtrl.SetBackgroundColor(target.Board.BackgroundColor);
            RampFadeManager.Fade(GameManager.CameraCtrl.GrayscaleRamp, Instance._fadeOutDatas, (0f, 0f));

            GameManager.PlayerCtrl.ResetVelocity();
            GameManager.PlayerCtrl.transform.position = target.OverrideRespawnPos != null ? target.OverrideRespawnPos.position : target.transform.position; // ?? operator does not seem to work.

            yield return null;

            GameManager.CameraCtrl.ToggleFreeze(false);
            GameManager.CameraCtrl.SetBoardBounds(target.BoardBounds);
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

            Instance.BoardsTransitionOver?.Invoke();
        }

        private static System.Collections.IEnumerator TransitionOutToScenesPassageCoroutine(Boards.BoardsLink source, Boards.ScenesPassage target)
        {
            SceneLoadingDatasStorageManager.StoreDatas();

            // Source's target must be get here, because the source ref will be missing after scene load.
            Boards.ScenesPassage sourceScenesPassage = source.GetTarget() as Boards.ScenesPassage;

            string sceneName = target.TargetPassage.GetTargetScene().SceneName;
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
            yield return RSLib.Yield.SharedYields.WaitForSceneLoad(sceneName);

            SceneLoadingDatasStorageManager.TryLoadDatas();

            yield return TransitionOutToBoardsLinkCoroutine(source, BoardsManager.GetLinkRelatedToScenesPassage(sourceScenesPassage));
        }
    }
}