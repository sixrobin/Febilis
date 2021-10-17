namespace Templar
{
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [CreateAssetMenu(fileName = "New Chest Identifier", menuName = "Id System/Identifier - Chest")]
    public class ChestIdentifier : Identifier
    {
        private const string ID_FORMAT = "Chest_{0}";

        public override string Id => string.Format(ID_FORMAT, base.Id);
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ChestIdentifier))]
    public class ChestIdentifierEditor : IdentifierEditor
    {
    }
#endif
}