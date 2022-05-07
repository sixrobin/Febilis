namespace Templar
{
    using UnityEngine;

    [DisallowMultipleComponent]
    public sealed class LocalizerDebugger : MonoBehaviour
    {
        private void Awake()
        {
            RSLib.Debug.Console.DebugConsole.OverrideCommand<string>("locKey", "Localizes a given key.", key => Localizer.Instance.Log(Localizer.Get(key), forceVerbose: true));
            RSLib.Debug.Console.DebugConsole.OverrideCommand<string>("locSetLanguage", "Set language.", Localizer.SetCurrentLanguage);
            RSLib.Debug.Console.DebugConsole.OverrideCommand<int>("locSetLanguageIndex", "Set language index.", Localizer.SetCurrentLanguage);
            
            RSLib.Debug.Console.DebugConsole.OverrideCommand("locShowLanguages", "Shows handled languages.",
                () =>
                {
                    for (int i = 0; i < Localizer.Instance.Languages.Length; ++i)
                    {
                        Localizer.Instance.Log(Localizer.Instance.Languages[i], forceVerbose: true);
                        RSLib.Debug.Console.DebugConsole.LogExternal(Localizer.Instance.Languages[i]);
                    }
                });
        }
    }
}