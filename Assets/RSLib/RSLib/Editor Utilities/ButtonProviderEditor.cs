#if UNITY_EDITOR
namespace RSLib.EditorUtilities
{
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Class used to help adding buttons in inspectors, for debug purpose.
    /// Child class still needs to specify the CustomEditor(typeof(T)) attribute.
    /// </summary>
    /// <typeparam name="T">Type of the object to draw a custom editor of.</typeparam>
    public abstract class ButtonProviderEditor<T> : Editor where T : Object
    {
        private const string EDITOR_UTILITIES_LABEL = "EDITOR UTILITIES";

        protected T Obj { get; private set; }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(15f);

            GUILayout.Label(EDITOR_UTILITIES_LABEL, EditorStyles.boldLabel);
            DrawButtons();
        }

        protected virtual void OnEnable()
        {
            Obj = (T)target;
        }

        protected abstract void DrawButtons();

        /// <summary>
        /// Can be used to draw a button with a System.Action callback in one line.
        /// </summary>
        /// <param name="label">Button text.</param>
        /// <param name="onClick">Callback on button clicked. Will throw an ArgumentNullException if null.</param>
        protected virtual void DrawButton(string label, System.Action onClick)
        {
            if (onClick == null)
                throw new System.ArgumentNullException(nameof(onClick), "Inspector button must have an onClick action.");

            if (GUILayout.Button(label))
                onClick();
        }
    }
}
#endif