namespace Templar.MainMenu
{
    using UnityEngine;

    public class MainMenuManager : RSLib.Framework.ConsoleProSingleton<MainMenuManager>
    {
        public static void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}