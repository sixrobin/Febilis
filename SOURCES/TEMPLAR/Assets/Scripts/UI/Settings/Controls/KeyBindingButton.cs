namespace Templar.UI.Settings.Controls
{
#if UNITY_EDITOR
    using UnityEditor;
#endif

    public class KeyBindingButton : RSLib.Framework.GUI.EnhancedButton
    {
        ///// <summary>Mimics the behaviour of OnPointerEnter event specified in the Button component when Selectable is selected by joystick navigation.</summary>
        ///// <param name="eventData">Navigation event data.</param>
        //public override void OnSelect(UnityEngine.EventSystems.BaseEventData eventData)
        //{
        //    base.OnSelect(eventData);
        //    OnPointerEnter(null);
        //}

        ///// <summary>Mimics the behaviour of OnPointerExit event specified in the Button component when Selectable is selected by joystick navigation.</summary>
        ///// <param name="eventData">Navigation event data.</param>
        //public override void OnDeselect(UnityEngine.EventSystems.BaseEventData eventData)
        //{
        //    base.OnDeselect(eventData);
        //    OnPointerExit(null);
        //}
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(KeyBindingButton)), CanEditMultipleObjects]
    public class KeyBindingButtonEditor : RSLib.Framework.GUI.EnhancedButtonEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
#endif
}