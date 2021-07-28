#if UNITY_EDITOR
namespace Templar.Tools
{
    using UnityEngine;

    /// <summary>
    /// Abstract class used to help adding buttons to managers prefabs instances to find their references in the scenes.
    /// Child class still needs to specify the CustomEditor(typeof(T)) attribute.
    /// </summary>
    /// <typeparam name="T">Type of the object to draw a custom editor of, that must be an IManagerReferencesHandler.</typeparam>
    public abstract class ManagerReferencesHandlerEditor<T> : RSLib.EditorUtilities.ButtonProviderEditor<T> where T : Object
    {
        private const string FIND_ALL_REFS_BUTTON = "Find All References";
        private const string FIND_MISSING_REFS_BUTTON = "Find Missing References";

        protected override void DrawButtons()
        {
            DrawButton(FIND_ALL_REFS_BUTTON, () => ManagersReferencesFinder.FindAllManagerReferences(Obj as IManagerReferencesHandler));
            DrawButton(FIND_MISSING_REFS_BUTTON, () => ManagersReferencesFinder.FindMissingManagerReferences(Obj as IManagerReferencesHandler));
        }
    }
}
#endif