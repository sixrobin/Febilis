namespace Templar.UI.Inventory
{
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [DisallowMultipleComponent]
    public class InventoryView : MonoBehaviour
    {
        [SerializeField] private InventorySlotView[] _slotsViews = null;

        public void LocateSlotsInParentChildren()
        {
            _slotsViews = transform.parent.GetComponentsInChildren<InventorySlotView>();

#if UNITY_EDITOR
            RSLib.EditorUtilities.SceneManagerUtilities.SetCurrentSceneDirty();
            RSLib.EditorUtilities.PrefabEditorUtilities.SetCurrentPrefabStageDirty();
#endif
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(InventoryView))]
    public class InventoryViewEditor : RSLib.EditorUtilities.ButtonProviderEditor<InventoryView>
    {
        protected override void DrawButtons()
        {
            DrawButton("Locate Slots in Parent Children", Obj.LocateSlotsInParentChildren);
        }
    }
#endif
}