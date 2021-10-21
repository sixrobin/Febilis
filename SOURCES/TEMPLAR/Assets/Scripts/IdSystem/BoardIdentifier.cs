namespace Templar.Flags
{
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [CreateAssetMenu(fileName = "New Board Identifier", menuName = "Id/Identifier - Board")]
    public class BoardIdentifier : Identifier
    {
        private const string ID_PREFIX = "Board";

        [Header("ZONE")]
        [SerializeField] private ZoneIdentifier _containingZone = null;
        [SerializeField] private bool _includeZoneId = true;

        public bool ZoneReferenceMissing => _includeZoneId && _containingZone == null;

        public override string Id
        {
            get
            {
                if (string.IsNullOrEmpty(BaseId)
                    && (!_includeZoneId || _containingZone == null)
                    && !UseNumbering)
                    return ID_ERROR;

                string id = ID_PREFIX;

                if (_includeZoneId && _containingZone != null)
                    id += "_" + _containingZone.BaseId;

                if (!string.IsNullOrEmpty(BaseId))
                    id += "_" + BaseId;

                if (UseNumbering)
                    id += "_" + Number;

                return id;
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(BoardIdentifier))]
    public class BoardIdentifierEditor : IdentifierEditor
    {
        public override void OnInspectorGUI()
        {
            if ((Obj as BoardIdentifier).ZoneReferenceMissing)
                EditorGUILayout.HelpBox("Zone reference is missing even though Zone Id is used to generate Board Id.", MessageType.Error);

            base.OnInspectorGUI();
        }
    }
#endif
}