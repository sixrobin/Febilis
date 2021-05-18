namespace Templar.UI
{
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    public class KeyBindingButton : RSLib.Framework.GUI.EnhancedButton
    {
        public override void OnSelect(UnityEngine.EventSystems.BaseEventData eventData)
        {
            base.OnSelect(eventData);
            OnPointerEnter(null);
        }

        public override void OnDeselect(UnityEngine.EventSystems.BaseEventData eventData)
        {
            base.OnDeselect(eventData);
            OnPointerExit(null);
        }

        public void SetHorizontalNavigation(UnityEngine.UI.Selectable selectable)
        {
            UnityEngine.UI.Navigation nav = new UnityEngine.UI.Navigation()
            {
                mode = UnityEngine.UI.Navigation.Mode.Explicit,
                selectOnUp = navigation.selectOnUp,
                selectOnDown = navigation.selectOnDown,
                selectOnLeft = selectable,
                selectOnRight = selectable
            };

            navigation = nav;
        }

        public void SetUpNavigation(UnityEngine.UI.Selectable selectable)
        {
            UnityEngine.UI.Navigation nav = new UnityEngine.UI.Navigation()
            {
                mode = UnityEngine.UI.Navigation.Mode.Explicit,
                selectOnUp = selectable,
                selectOnDown = navigation.selectOnDown,
                selectOnLeft = navigation.selectOnLeft,
                selectOnRight = navigation.selectOnRight
            };

            navigation = nav;
        }

        public void SetDownNavigation(UnityEngine.UI.Selectable selectable)
        {
            UnityEngine.UI.Navigation nav = new UnityEngine.UI.Navigation()
            {
                mode = UnityEngine.UI.Navigation.Mode.Explicit,
                selectOnUp = navigation.selectOnUp,
                selectOnDown = selectable,
                selectOnLeft = navigation.selectOnLeft,
                selectOnRight = navigation.selectOnRight
            };

            navigation = nav;
        }
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