namespace Templar.Manager
{
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class ScenesManager : RSLib.Framework.ConsoleProSingleton<ScenesManager>
    {
        [SerializeField] private RSLib.Framework.SceneField _mainMenuScene = null;
        [SerializeField] private RSLib.Framework.SceneField _levelScene = null;

        public static RSLib.Framework.SceneField MainMenuScene => Instance._mainMenuScene;
        public static RSLib.Framework.SceneField LevelScene => Instance._levelScene;

        public static string ActiveSceneName => SceneManager.GetActiveScene().name;
        public static int ActiveSceneBuildIndex => SceneManager.GetActiveScene().buildIndex;

        public static void LoadScene(RSLib.Framework.SceneField scene)
        {
            SceneManager.LoadScene(scene);
        }

        public static void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        public static void LoadScene(int sceneBuildIndex)
        {
            SceneManager.LoadScene(sceneBuildIndex);
        }
    }
}