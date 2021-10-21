namespace Templar.Flags
{
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [CreateAssetMenu(fileName = "New Zone Identifier", menuName = "Id/Identifier - Zone")]
    public class ZoneIdentifier : Identifier
    {
        private const string ID_PREFIX = "Zone";

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
    [CustomEditor(typeof(ZoneIdentifier))]
    public class ZoneIdentifierEditor : IdentifierEditor
    {
    }
#endif
}