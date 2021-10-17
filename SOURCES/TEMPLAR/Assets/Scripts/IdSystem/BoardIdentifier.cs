namespace Templar
{
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [CreateAssetMenu(fileName = "New Board Identifier", menuName = "Id System/Identifier - Board")]
    public class BoardIdentifier : Identifier
    {
        private const string ID_FORMAT = "Board_{0}";

        public override string Id => string.Format(ID_FORMAT, base.Id);
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(BoardIdentifier))]
    public class BoardIdentifierEditor : IdentifierEditor
    {
    }
#endif
}