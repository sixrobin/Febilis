﻿namespace Templar.UI.Navigation
{
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class UINavigationManager : RSLib.Framework.SingletonConsolePro<UINavigationManager>
    {
        private const string INPUT_BACK = "UICancel";

        [SerializeField] private ConfirmationPopup _confirmationPopup = null;
        [SerializeField] private MessagePopup _messagePopup = null;

#if UNITY_EDITOR
        [Header("DEBUG")]
        [SerializeField] private RSLib.Framework.DisabledGameObject _currSelection = new RSLib.Framework.DisabledGameObject(null);
        [SerializeField] private RSLib.Framework.DisabledGameObject _currentlyOpenPanel = new RSLib.Framework.DisabledGameObject(null);
#endif

        public static UIPanel CurrentlyOpenPanel { get; private set; }

        public static ConfirmationPopup ConfirmationPopup
        {
            get
            {
                if (Instance._confirmationPopup == null)
                {
                    Instance.LogWarning($"{typeof(ConfirmationPopup).Name} reference is missing, trying to get it dynamically...", Instance.gameObject);
                    Instance._confirmationPopup = FindObjectOfType<ConfirmationPopup>();

                    if (Instance._confirmationPopup == null)
                        Instance.LogError($"No {typeof(ConfirmationPopup).Name} instance has been found in the scene!", Instance.gameObject);
                }

                return Instance._confirmationPopup;
            }
        }

        public static MessagePopup MessagePopup
        {
            get
            {
                if (Instance._messagePopup == null)
                {
                    Instance.LogWarning($"{typeof(MessagePopup).Name} reference is missing, trying to get it dynamically...", Instance.gameObject);
                    Instance._messagePopup = FindObjectOfType<MessagePopup>();

                    if (Instance._messagePopup == null)
                        Instance.LogError($"No {typeof(MessagePopup).Name} instance has been found in the scene!", Instance.gameObject);
                }

                return Instance._messagePopup;
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

        private static void HandleBackInput()
        {
            if (CurrentlyOpenPanel != null && Input.GetButtonDown(INPUT_BACK))
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
            HandleBackInput();

#if UNITY_EDITOR
            _currSelection = new RSLib.Framework.DisabledGameObject(EventSystem.current.currentSelectedGameObject);
            _currentlyOpenPanel = new RSLib.Framework.DisabledGameObject(CurrentlyOpenPanel != null ? CurrentlyOpenPanel.gameObject : null);
#endif
        }

        [ContextMenu("Find All References")]
        private void DebugFindAllReferences()
        {
            _confirmationPopup = FindObjectOfType<ConfirmationPopup>();
            _messagePopup = FindObjectOfType<MessagePopup>();

            RSLib.EditorUtilities.SceneManagerUtilities.SetCurrentSceneDirty();
        }

        [ContextMenu("Find Missing References")]
        private void DebugFindMissingReferences()
        {
            _confirmationPopup = _confirmationPopup ?? FindObjectOfType<ConfirmationPopup>();
            _messagePopup = _messagePopup ?? FindObjectOfType<MessagePopup>();

            RSLib.EditorUtilities.SceneManagerUtilities.SetCurrentSceneDirty();
        }
    }
}