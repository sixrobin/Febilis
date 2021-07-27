namespace Templar.UI.Navigation
{
    using UnityEngine;
    using UnityEngine.EventSystems;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    public class UINavigationManager : RSLib.Framework.ConsoleProSingleton<UINavigationManager>
    {
        private const string INPUT_BACK = "UICancel";

        [SerializeField] private ConfirmationPopup _confirmationPopup = null;

#if UNITY_EDITOR
        [Header("DEBUG")]
        [SerializeField] private GameObject _currSelection = null;
#endif

        public static UIPanel CurrentlyOpenPanel { get; private set; }

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

            SetPanelAsCurrent(uiPanel);
            CurrentlyOpenPanel.Open();
            Select(CurrentlyOpenPanel.FirstSelected);
        }

        public static void CloseCurrentPanel()
        {
            if (CurrentlyOpenPanel == null)
                return;

            Instance.Log($"Closing {CurrentlyOpenPanel.transform.name}.", CurrentlyOpenPanel.gameObject);
            CurrentlyOpenPanel.Close();
            CurrentlyOpenPanel = null;
        }

        public static void Select(GameObject selected)
        {
            SetSelectedGameObject(selected);
        }

        public static void SetPanelAsCurrent(UIPanel uiPanel)
        {
            CurrentlyOpenPanel = uiPanel;
        }

        public static void NullifySelected()
        {
            SetSelectedGameObject(null);
        }

        private static void TryHandleBackInput()
        {
            if (CurrentlyOpenPanel == null)
                return;

            if (Input.GetButtonDown(INPUT_BACK))
                CurrentlyOpenPanel.OnBackButtonPressed();
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

        public void DebugFindAllReferences()
        {
            _confirmationPopup = FindObjectOfType<ConfirmationPopup>();

#if UNITY_EDITOR
            RSLib.EditorUtilities.SceneManagerUtilities.SetCurrentSceneDirty();
#endif
        }

        public void DebugFindMissingReferences()
        {
            _confirmationPopup = _confirmationPopup ?? FindObjectOfType<ConfirmationPopup>();

#if UNITY_EDITOR
            RSLib.EditorUtilities.SceneManagerUtilities.SetCurrentSceneDirty();
#endif
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(UINavigationManager))]
    public class UINavigationManagerEditor : RSLib.EditorUtilities.ButtonProviderEditor<UINavigationManager>
    {
        protected override void DrawButtons()
        {
            DrawButton("Find All References", Obj.DebugFindAllReferences);
            DrawButton("Find Missing References", Obj.DebugFindMissingReferences);
        }
    }
#endif
}