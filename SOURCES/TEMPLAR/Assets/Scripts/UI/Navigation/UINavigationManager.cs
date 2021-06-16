namespace Templar.UI.Navigation
{
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class UINavigationManager : RSLib.Framework.ConsoleProSingleton<UINavigationManager>
    {
        private const string INPUT_BACK = "UICancel";

        [SerializeField] private ConfirmationPopup _confirmationPopup = null;

#if UNITY_EDITOR
        [Header("DEBUG")]
        [SerializeField] private GameObject _currSelection = null;
#endif

        private static UIPanel _currOpenPanel;

        public static ConfirmationPopup ConfirmationPopup
        {
            get
            {
                if (Instance._confirmationPopup == null)
                {
                    Instance.LogWarning("ConfirmationPopup reference is missing, trying to get it dynamically...", Instance.gameObject);
                    Instance._confirmationPopup = FindObjectOfType<ConfirmationPopup>();

                    if (Instance._confirmationPopup == null)
                        Instance.LogError("No ConfirmationPopup instance has been found in the scene!", Instance.gameObject);
                }

                return Instance._confirmationPopup;
            }
        }

        public static GameObject CurrentlySelected => EventSystem.current.currentSelectedGameObject;

        public static void OpenAndSelect(UIPanel uiPanel)
        {
            UnityEngine.Assertions.Assert.IsNotNull(uiPanel, "Trying to open a null panel.");
            Instance.Log($"Opening {uiPanel.transform.name}.", uiPanel.gameObject);

            _currOpenPanel = uiPanel;
            _currOpenPanel.Open();
            Select(_currOpenPanel.FirstSelected);
        }

        public static void CloseCurrentPanel()
        {
            if (_currOpenPanel == null)
                return;

            Instance.Log($"Closing {_currOpenPanel.transform.name}.", _currOpenPanel.gameObject);
            _currOpenPanel.Close();
            _currOpenPanel = null;
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

            if (Input.GetButtonDown(INPUT_BACK))
                _currOpenPanel.OnBackButtonPressed();
        }

        private static void SetSelectedGameObject(GameObject selected)
        {
            Instance.Log($"Selecting {selected?.transform.name ?? "none"}.", selected?.gameObject ?? null);

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

        // [TODO] Editor button.
        [ContextMenu("Locate Confirmation Popup")]
        private void LocateConfirmationPopup()
        {
            _confirmationPopup = FindObjectOfType<ConfirmationPopup>();
#if UNITY_EDITOR
            RSLib.EditorUtilities.SceneManagerUtilities.SetCurrentSceneDirty();
#endif
        }
    }
}