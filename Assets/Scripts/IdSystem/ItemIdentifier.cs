namespace Templar.Flags
{
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [CreateAssetMenu(fileName = "New Item Identifier", menuName = "Id/Identifier - Item")]
    public class ItemIdentifier : Identifier
    {
        private const string ID_PREFIX = "Item";

        public override string Id
        {
            get
            {
                string id = ID_PREFIX;

                if (!string.IsNullOrEmpty(BaseId))
                    id += "_" + BaseId;

                if (UseNumbering)
                    id += "_" + Number;

                return id;
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ItemIdentifier))]
    public class ItemIdentifierEditor : IdentifierEditor
    {
    }
#endif
}