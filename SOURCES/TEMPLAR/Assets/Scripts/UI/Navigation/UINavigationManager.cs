namespace Templar.UI.Navigation
{
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class UINavigationManager : RSLib.Framework.ConsoleProSingleton<UINavigationManager>
    {
#if UNITY_EDITOR
        [Header("DEBUG")]
        [SerializeField] private GameObject _currSelection = null;
#endif

        private static IUIPanel _currOpenPanel;

        public static void OpenAndSelect(IUIPanel uiPanel)
        {
            UnityEngine.Assertions.Assert.IsNotNull(uiPanel, "Trying to open a null panel.");

            _currOpenPanel = uiPanel;
            _currOpenPanel.Open();
            Select(_currOpenPanel.FirstSelected);
        }

        public static void CloseCurrentPanel()
        {
            if (_currOpenPanel == null)
                return;

            _currOpenPanel.Close();
            _currOpenPanel = null;

            NullifySelected();
        }

        public static void Select(GameObject selected)
        {
            SetSelectedGameObject(selected);
        }

        public static void NullifySelected()
        {
            SetSelectedGameObject(null);
        }

        private static void TryHandleBackInput()
        {
            if (_currOpenPanel == null)
                return;

            if (Input.GetButtonDown("UICancel")) // [TMP] Hardcoded input string.
                _currOpenPanel.OnBackButtonPressed();
        }

        private static void SetSelectedGameObject(GameObject selected)
        {
            if (EventSystem.current.alreadySelecting)
            {
                Instance.StartCoroutine(Instance.SetSelectedGameObjectDelayed(selected));
                return;
            }

            EventSystem.current.SetSelectedGameObject(selected);
        }

        private System.Collections.IEnumerator SetSelectedGameObjectDelayed(GameObject selected)
        {
            yield return RSLib.Yield.SharedYields.WaitForEndOfFrame;
            EventSystem.current.SetSelectedGameObject(selected);
        }

        private void Update()
        {
            TryHandleBackInput();

#if UNITY_EDITOR
            _currSelection = EventSystem.current.currentSelectedGameObject;
#endif
        }
    }
}