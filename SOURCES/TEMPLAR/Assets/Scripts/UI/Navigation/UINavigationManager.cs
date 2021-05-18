namespace Templar.UI.Navigation
{
    using UnityEngine;
    using UnityEngine.EventSystems;

    [DisallowMultipleComponent]
    public class UINavigationManager : RSLib.Framework.ConsoleProSingleton<UINavigationManager>
    {
#if UNITY_EDITOR
        [Header("DEBUG")]
        [SerializeField] private GameObject _currSelection = null;
#endif

        public static void Select(GameObject selected)
        {
            if (EventSystem.current.alreadySelecting)
            {
                Instance.StartCoroutine(Instance.SelectDelayed(selected));
                return;
            }

            EventSystem.current.SetSelectedGameObject(selected);
        }

        private System.Collections.IEnumerator SelectDelayed(GameObject selected)
        {
            yield return RSLib.Yield.SharedYields.WaitForEndOfFrame;
            EventSystem.current.SetSelectedGameObject(selected);
        }

#if UNITY_EDITOR
        private void Update()
        {
            _currSelection = EventSystem.current.currentSelectedGameObject;
        }
#endif
    }
}