namespace Templar
{
    using RSLib.Extensions;
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [CreateAssetMenu(fileName = "New Identifier", menuName = "Id System/Identifier", order = -1)]
    public class Identifier : ScriptableObject, IIdentifier
    {
        private const string ID_FORMAT = "{0}_{1}";

        [SerializeField] private RSLib.Framework.DisabledString _preview = new RSLib.Framework.DisabledString(string.Empty);

        [Space(15f)]
        [SerializeField] private string _id = string.Empty;
        [SerializeField, Min(0)] private int _number = 0;

        public virtual string Id => string.Format(ID_FORMAT, _id, _number.AddLeading0(4));

        public void CopyIdToClipboard()
        {
            _preview.Value.CopyToClipboard();
        }

        protected virtual void OnValidate()
        {
            _preview = new RSLib.Framework.DisabledString(Id);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Identifier))]
    public class IdentifierEditor : RSLib.EditorUtilities.ButtonProviderEditor<Identifier>
    {
        protected override void DrawButtons()
        {
            DrawButton("Copy ID to clipboard", Obj.CopyIdToClipboard);
        }
    }
#endif
}