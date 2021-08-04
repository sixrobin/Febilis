namespace Templar.UI.Inventory.ContextMenu
{
    using RSLib.Extensions;
    using System.Linq;
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [DisallowMultipleComponent]
    public class ItemContextMenu : UIPanel
    {
        [SerializeField] private ItemContextMenuAction[] _contextActions = null;
        [SerializeField] private Vector2 _posOffset = Vector2.zero;

        public override GameObject FirstSelected => GetFirstAllowedAction()?.gameObject ?? null;

        public InventorySlot Slot { get; private set; }

        private ItemContextMenuAction GetFirstAllowedAction()
        {
            for (int i = 0; i < _contextActions.Length; ++i)
                if (_contextActions[i].ActionAllowed)
                    return _contextActions[i];

            return null;
        }

        public override void Open()
        {
            UnityEngine.Assertions.Assert.IsNotNull(Slot, "Inventory slot must be referenced before opening Item context menu.");

            base.Open();
            
            CProLogger.Log(this, $"Opening Item context menu for item {Slot.Item.Id}.");

            for (int i = _contextActions.Length - 1; i >= 0; --i)
                _contextActions[i].Init(Slot);

            ItemContextMenuAction[] allowedActions = _contextActions.Where(o => o.ActionAllowed).ToArray();
            UnityEngine.UI.Selectable actionButton;
            for (int i = 0; i < allowedActions.Length; ++i)
            {
                actionButton = allowedActions[i].Button;
                actionButton.SetMode(UnityEngine.UI.Navigation.Mode.Explicit);
                actionButton.SetSelectOnUp(i == 0 ? allowedActions.Last().Button : allowedActions[i - 1].Button);
                actionButton.SetSelectOnDown(i == allowedActions.Length - 1 ? allowedActions[0].Button : allowedActions[i + 1].Button);
            }

            SetPosition();
        }

        public override void Close()
        {
            base.Close();

            // Immediatly close inventory.
            Navigation.UINavigationManager.SetPanelAsCurrent(Manager.GameManager.InventoryView);
            Navigation.UINavigationManager.CloseCurrentPanel();
            Navigation.UINavigationManager.NullifySelected();

            Slot.DisplaySelector(false);
            Slot = null;
        }

        public override void OnBackButtonPressed()
        {
            base.OnBackButtonPressed();

            Navigation.UINavigationManager.SetPanelAsCurrent(Manager.GameManager.InventoryView);
            Navigation.UINavigationManager.Select(Slot.gameObject);

            Slot = null;
        }

        public void SetSlotContext(InventorySlot slot)
        {
            Slot = slot;
        }

        public void CloseAtEndOfFrame()
        {
            StartCoroutine(CloseAtEndOfFrameCoroutine());
        }

        private void SetPosition()
        {
            RectTransform.anchoredPosition = Slot.RectTransform.anchoredPosition + _posOffset;
        }

        private System.Collections.IEnumerator CloseAtEndOfFrameCoroutine()
        {
            yield return RSLib.Yield.SharedYields.WaitForEndOfFrame;
            Close();
        }

        private void Awake()
        {
            for (int i = _contextActions.Length - 1; i >= 0; --i)
                _contextActions[i].SetContextMenu(this);
        }

        public void LocateActionsInChildren()
        {
            _contextActions = GetComponentsInChildren<ItemContextMenuAction>();
            RSLib.EditorUtilities.PrefabEditorUtilities.SetCurrentPrefabStageDirty();
            RSLib.EditorUtilities.SceneManagerUtilities.SetCurrentSceneDirty();
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ItemContextMenu))]
    public class ItemContextMenuEditor : RSLib.EditorUtilities.ButtonProviderEditor<ItemContextMenu>
    {
        protected override void DrawButtons()
        {
            DrawButton("Locate Actions in Children", Obj.LocateActionsInChildren);
        }
    }
#endif
}