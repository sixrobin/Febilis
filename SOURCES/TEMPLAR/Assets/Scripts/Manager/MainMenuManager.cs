namespace Templar.Manager
{
    public class MainMenuManager : RSLib.Framework.SingletonConsolePro<MainMenuManager>
    {
        public static void NewGame()
        {
            if (SaveManager.EraseSave())
                Instance.Log("Erased save file to start a new game.");

            ScenesManager.LoadScene(ScenesManager.LevelScene);
        }

        public static void LoadSavedGame()
        {
            // TODO: Load scene that leads to save file actual progress.
            ScenesManager.LoadScene(ScenesManager.LevelScene);
        }

        public static void Quit()
        {
            RSLib.Helpers.QuitPlatformDependent();
        }
    }
}