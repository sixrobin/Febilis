namespace RSLib
{
    using UnityEngine;

    /// <summary>
    /// Used only to be attached on a MonoBehaviour to call the Quit method from a UnityEvent.
    /// </summary>
    public sealed class ApplicationQuit : MonoBehaviour
    {
        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            UnityEngine.Application.Quit();
#endif
        }
    }
}