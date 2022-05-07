namespace Templar
{
    using UnityEngine;

    [DisallowMultipleComponent]
    public sealed class LocalizerDebugger : MonoBehaviour
    {
        private void Awake()
        {
            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command<string>("locKey", "Localizes a given key.",
                (key) => Localizer.Instance.Log(Localizer.Get(key), forceVerbose: true)));
            
            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command<string>("locSetLanguage", "Set language.",
                Localizer.SetCurrentLanguage));
            
            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command<int>("locSetLanguageIndex", "Set language index.",
                Localizer.SetCurrentLanguage));
        }
    }
}