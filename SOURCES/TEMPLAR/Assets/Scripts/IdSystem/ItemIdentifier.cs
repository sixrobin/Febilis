namespace Templar
{
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [CreateAssetMenu(fileName = "New Item Identifier", menuName = "Id System/Identifier - Item")]
    public class ItemIdentifier : Identifier
    {
        private const string ID_FORMAT = "Item_{0}";

        public override string Id => string.Format(ID_FORMAT, base.Id);
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ItemIdentifier))]
    public class ItemIdentifierEditor : IdentifierEditor
    {
    }
#endif
}