namespace Templar.Flags
{
    using RSLib.Extensions;
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [CreateAssetMenu(fileName = "New Identifier", menuName = "Id/Identifier", order = -1)]
    public class Identifier : ScriptableObject, IIdentifier
    {
        protected const string ID_ERROR = "NA";

        [SerializeField] private RSLib.Framework.DisabledString _preview = new RSLib.Framework.DisabledString(string.Empty);

        [Header("BASE")]
        [SerializeField] private string _id = string.Empty;
        [SerializeField] private RSLib.Framework.OptionalInt _number = new RSLib.Framework.OptionalInt(0, true);

        public string BaseId => _id;
        public string Number => _number.Value.AddLeading0(4);
        public bool UseNumbering => _number.Enabled;

        public virtual string Id
        {
            get
            {
                string id = BaseId;
                if (UseNumbering)
                    id += "_" + Number;

                return id;
            }
        }

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